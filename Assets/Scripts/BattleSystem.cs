using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    // Audio
    public AudioSource attackSound;
    public AudioSource enemyAttackSound;
    public AudioSource backgroundMusic;
    public AudioSource specialAttackSound;
    public AudioSource playerDeathSound;
    public AudioSource receiveEssenceSound;
    public AudioSource receiveSoulSound;
    public AudioSource worthyHeroSound;
    public AudioSource reviveSound;

    // Animations
    private Animator playerAnimator;
    private Animator enemyAnimator;

    // Track whether actions have been performed during each turn
    private bool hasPlayerActed = false;
    private bool hasEnemyActed = false;

    // Damage range for player and enemy
    public int playerMinDamage = 50;
    public int playerMaxDamage = 100;
    public int enemyMinDamage = 20;
    public int enemyMaxDamage = 40;

    // Track the number of player attacks
    private int playerAttackCount = 0;

    // Track rewards received
    private bool receivedEssence = false;
    private bool receivedSoul = false;

    private PauseManager pauseManager;

    void Start()
    {
        state = BattleState.START;
        pauseManager = FindObjectOfType<PauseManager>();
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseManager != null)
            {
                Debug.Log("Pausing game from BattleSystem");
                pauseManager.PauseGame();
            }
        }
    }

    IEnumerator SetupBattle()
    {
        // Check if background music is already playing
        if (!backgroundMusic.isPlaying)
        {
            // Start the background music
            backgroundMusic.Play();
        }

        // Destroy existing player and enemy GameObjects if they exist
        foreach (Transform child in playerBattleStation)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in enemyBattleStation)
        {
            Destroy(child.gameObject);
        }

        // Instantiate player and enemy units
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        playerAnimator = playerGO.GetComponent<Animator>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyAnimator = enemyGO.GetComponent<Animator>();

        // Set up HUDs right at the start
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        // Set player to BattleIdle state
        playerAnimator.SetBool("InBattle", true);

        // Display the initial dialogue message
        dialogueText.text = "" + enemyUnit.unitName + " will test your worth.";
        yield return new WaitForSeconds(4f);

        // Transition to player's turn
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        if (hasPlayerActed)
        {
            // Player has already performed an action this turn, so return without executing attack
            yield break;
        }

        // Set the flag to indicate that the player has acted
        hasPlayerActed = true;
        playerAttackCount++;

        int damage = 0;
        bool specialAttack = false;

        // Check if this is within the first three attacks and if the special attack triggers
        if (playerAttackCount <= 3 && Random.value < 0.05f)
        {
            damage = 500;
            specialAttack = true;
        }
        else
        {
            // Generate random damage within the specified range
            damage = Random.Range(playerMinDamage, playerMaxDamage);
        }

        // Play attack sound and trigger appropriate attack animation
        if (specialAttack)
        {
            specialAttackSound.Play();
            playerAnimator.SetTrigger("SpecialAttack");
            dialogueText.text = "You've delivered a killing blow!";
        }
        else
        {
            attackSound.Play();
            playerAnimator.SetTrigger("Attack");
            dialogueText.text = "You dealt " + damage + " damage to " + enemyUnit.unitName + "!";
        }

        yield return new WaitForSeconds(2f); // Adjust to match the duration of the animation

        bool isDead = enemyUnit.TakeDamage(damage);
        enemyHUD.SetHP(enemyUnit.currentHP);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerHeal()
    {
        if (hasPlayerActed)
        {
            // Player has already performed an action this turn, so return without executing heal
            yield break;
        }

        // Set the flag to indicate that the player has acted
        hasPlayerActed = true;

        int healAmount = 150;
        playerUnit.Heal(healAmount);
        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You healed " + healAmount + " HP!";
        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        if (hasEnemyActed)
        {
            // Enemy has already performed an action this turn, so return without executing its turn
            yield break;
        }

        // Set the flag to indicate that the enemy has acted
        hasEnemyActed = true;

        dialogueText.text = enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(2f);

        // Play enemy attack sound and trigger attack animation
        enemyAttackSound.Play();
        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(2f); // Adjust to match the duration of the animation

        // Generate random damage within the specified range
        int damage = Random.Range(enemyMinDamage, enemyMaxDamage);

        bool isDead = playerUnit.TakeDamage(damage);
        playerHUD.SetHP(playerUnit.currentHP);

        dialogueText.text = enemyUnit.unitName + " dealt " + damage + " damage to you!";
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

        // Reset the flags indicating actions after player's turn
        hasPlayerActed = false;
        hasEnemyActed = false;
    }

    void EndBattle()
    {
        backgroundMusic.Stop();
        StartCoroutine(HandleBattleEnd());
    }

    IEnumerator HandleBattleEnd()
    {
        if (state == BattleState.WON)
        {
            if (playerAttackCount <= 3 && !receivedEssence)
            {
                dialogueText.text = "If I was mortal, I'd be gone. You've shown your strength today, hero.";
                yield return new WaitForSeconds(2.5f);
                dialogueText.text = "";
                yield return new WaitForSeconds(0.5f); // Short pause to clear the text
                dialogueText.text = ColorizeText("Please, take my essence.");
                yield return new WaitForSeconds(2.2f);
                receiveEssenceSound.Play(); // Play essence sound effect
                dialogueText.text = "";
                yield return new WaitForSeconds(0.5f); // Short pause to clear the text
                dialogueText.text = "<color=#00008B>Yœsh has received Deslegch's Essence</color>";
                receivedEssence = true;
                yield return new WaitForSeconds(2.2f);
            }
            else if (playerAttackCount >= 6 && !receivedSoul)
            {
                dialogueText.text = "You've shown your resilience today, hero.";
                yield return new WaitForSeconds(2.5f);
                dialogueText.text = "";
                yield return new WaitForSeconds(0.5f); // Short pause to clear the text
                dialogueText.text = ColorizeText("Please, take my soul.");
                yield return new WaitForSeconds(2.2f);
                receiveSoulSound.Play(); // Play soul sound effect
                dialogueText.text = "";
                yield return new WaitForSeconds(0.5f); // Short pause to clear the text
                dialogueText.text = "<color=#006400>Yœsh has received Deslegch's Soul</color>";
                receivedSoul = true;
                yield return new WaitForSeconds(2.2f);
            }

            if (receivedEssence && receivedSoul)
            {
                dialogueText.text = "You are worthy, hero. Good luck on your journey.";
                worthyHeroSound.Play(); // Play "worthy hero" sound effect
                yield return new WaitForSeconds(2.5f);
                // Add your code to reset progress and go to StartMenu here
                ResetGameProgress();
            }
            else if (receivedEssence)
            {
                dialogueText.text = ColorizeText("You show potential, but your best has yet to come.");
                yield return new WaitForSeconds(2.5f);
                dialogueText.text = "";
                yield return new WaitForSeconds(0.5f); // Short pause to clear the text
                dialogueText.text = ColorizeText("Try again and you could be rewarded with my Soul.");
                yield return new WaitForSeconds(2.5f);
            }
            else if (receivedSoul)
            {
                dialogueText.text = ColorizeText("You show potential, but your best has yet to come.");
                yield return new WaitForSeconds(2.5f);
                dialogueText.text = "";
                yield return new WaitForSeconds(0.5f); // Short pause to clear the text
                dialogueText.text = ColorizeText("Try again and you could be rewarded with my Essence.");
                yield return new WaitForSeconds(2.5f);
            }
            else
            {
                dialogueText.text = ColorizeText("You show potential, but your best has yet to come.");
                yield return new WaitForSeconds(2.5f);
                dialogueText.text = "";
                yield return new WaitForSeconds(0.5f); // Short pause to clear the text
                dialogueText.text = ColorizeText("Try again and you could be rewarded with my Essence or Soul.");
                yield return new WaitForSeconds(2.5f);
            }
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You must try again, hero.";
            yield return new WaitForSeconds(2.5f);

            // Trigger player death animation and play death sound
            playerDeathSound.Play();
            playerAnimator.SetTrigger("Death");

            yield return new WaitForSeconds(2.5f);

            dialogueText.text = "I will revive you and we will start again.";
            yield return new WaitForSeconds(2.5f);

            // Play revive sound effect
            reviveSound.Play();

            // Return player to idle animation
            playerAnimator.SetTrigger("Revive");
        }

        // Reset the battle if player hasn't received both essence and soul
        if (!(receivedEssence && receivedSoul))
        {
            StartCoroutine(ResetBattle());
        }
        else
        {
            // Add your code to reset progress and go to StartMenu here
            ResetGameProgress();
        }
    }

    IEnumerator ResetBattle()
    {
        yield return new WaitForSeconds(3f);
        state = BattleState.START;
        playerAttackCount = 0;
        StartCoroutine(SetupBattle());
    }

    void ResetGameProgress()
    {
        // Reset progress-related variables
        receivedEssence = false;
        receivedSoul = false;
        playerAttackCount = 0;
        hasPlayerActed = false;
        hasEnemyActed = false;

        // Additional progress reset logic as needed
        // ...

        // Clear PlayerPrefs (or any other persistent data storage you are using)
        PlayerPrefs.DeleteAll();
        // Alternatively, if you are using a different method of data storage, reset that data here.

        // Load the StartMenu scene
        SceneManager.LoadScene("StartMenu");
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
        state = BattleState.PLAYERTURN; // Ensure the state is set correctly
        hasPlayerActed = false; // Reset player action flag
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }

    string ColorizeText(string text)
    {
        return text.Replace("essence", "<color=#00008B>essence</color>")
                   .Replace("Essence", "<color=#00008B>Essence</color>")
                   .Replace("soul", "<color=#006400>soul</color>")
                   .Replace("Soul", "<color=#006400>Soul</color>")
                   .Replace("Deslegch's Essence", "<color=#00008B>Deslegch's Essence</color>")
                   .Replace("Deslegch's Soul", "<color=#006400>Deslegch's Soul</color>");
    }
}