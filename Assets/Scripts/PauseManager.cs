using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public AudioSource backgroundMusic;
    public AudioSource resumeSound;
    public AudioSource quitSound;
    private static bool isPaused = false;
    private static bool isPauseMenuLoaded = false;

    private EventSystem battleSceneEventSystem;
    private EventSystem pauseMenuEventSystem;

    void Awake()
    {
        // Ensure only one instance of PauseManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Find the Event System in the Battle Scene
        battleSceneEventSystem = FindObjectOfType<EventSystem>();
        if (battleSceneEventSystem != null)
        {
            Debug.Log("Battle Scene Event System found: " + battleSceneEventSystem.gameObject.name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Debug.Log("Resuming game...");
                StartCoroutine(ResumeGame());
            }
            else
            {
                Debug.Log("Pausing game...");
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused || isPauseMenuLoaded) return; // Prevent multiple calls
        Debug.Log("Loading pause menu...");
        StartCoroutine(LoadPauseMenu());
    }

    private IEnumerator LoadPauseMenu()
    {
        isPauseMenuLoaded = true; // Set this flag immediately to prevent multiple calls
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PauseMenuScene", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("Pause menu loaded");
        Time.timeScale = 0f;
        isPaused = true;
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        // Disable the battle scene Event System and enable the pause menu Event System
        if (battleSceneEventSystem != null)
        {
            battleSceneEventSystem.gameObject.SetActive(false);
            Debug.Log("Battle Scene Event System disabled");
        }
        pauseMenuEventSystem = FindPauseMenuEventSystem();
        if (pauseMenuEventSystem != null)
        {
            pauseMenuEventSystem.gameObject.SetActive(true);
            Debug.Log("Pause Menu Event System enabled");
        }
    }

    public IEnumerator ResumeGame()
    {
        if (!isPaused || !isPauseMenuLoaded) yield break; // Prevent multiple calls

        Debug.Log("Unloading pause menu...");
        // Unload the pause menu scene if it is loaded
        if (IsSceneLoaded("PauseMenuScene"))
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("PauseMenuScene");
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
            Debug.Log("Pause menu unloaded. Resuming game...");
        }
        else
        {
            Debug.LogWarning("Pause menu scene was not loaded.");
        }

        // Enable the battle scene Event System and disable the pause menu Event System
        if (battleSceneEventSystem != null)
        {
            battleSceneEventSystem.gameObject.SetActive(true);
            Debug.Log("Battle Scene Event System enabled");
        }
        if (pauseMenuEventSystem != null)
        {
            pauseMenuEventSystem.gameObject.SetActive(false);
            Debug.Log("Pause Menu Event System disabled");
        }

        // Resume game time
        Time.timeScale = 1f;
        isPaused = false;
        isPauseMenuLoaded = false;
        if (backgroundMusic != null)
        {
            if (!backgroundMusic.enabled)
            {
                backgroundMusic.enabled = true;
            }
            backgroundMusic.UnPause();
        }
        if (resumeSound != null)
        {
            if (!resumeSound.enabled)
            {
                resumeSound.enabled = true;
            }
            resumeSound.Play();
        }
    }

    public void ResumeGameFromButton()
    {
        StartCoroutine(ResumeGame());
    }

    public void QuitGame()
    {
        // Call the ResetGame method to clear progress
        GameManager.instance.ResetGame();

        Time.timeScale = 1f;
        if (quitSound != null)
        {
            if (!quitSound.enabled)
            {
                quitSound.enabled = true;
            }
            quitSound.Play();
        }
        DeactivatePauseMenuEventSystems();
        SceneManager.LoadScene("StartMenu");
    }

    private void DeactivatePauseMenuEventSystems()
    {
        if (pauseMenuEventSystem != null)
        {
            Debug.Log("Deactivating PauseMenu EventSystem on " + pauseMenuEventSystem.gameObject.name);
            pauseMenuEventSystem.gameObject.SetActive(false);
        }
    }

    private EventSystem FindPauseMenuEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        foreach (EventSystem es in eventSystems)
        {
            if (es != battleSceneEventSystem)
            {
                return es;
            }
        }
        return null;
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}