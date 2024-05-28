using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public AudioSource backgroundMusic;
    public AudioSource resumeSound;
    public AudioSource quitSound;

    void Start()
    {
        Debug.Log("PauseMenu Start: Setting pauseMenuUI to inactive");
        pauseMenuUI.SetActive(false); // Ensure the pause menu is hidden at the start
        LogUIState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed");
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Debug.Log("Resuming game");
        resumeSound.Play();
        pauseMenuUI.SetActive(false);
        Debug.Log("pauseMenuUI is now inactive: " + !pauseMenuUI.activeSelf);
        Time.timeScale = 1f;
        GameIsPaused = false;
        backgroundMusic.UnPause();
        Debug.Log("Game resumed: pauseMenuUI inactive, Time.timeScale = 1, GameIsPaused = " + GameIsPaused);
        LogUIState();
    }

    public void Pause()
    {
        Debug.Log("Pausing game");
        pauseMenuUI.SetActive(true);
        Debug.Log("pauseMenuUI is now active: " + pauseMenuUI.activeSelf);
        Time.timeScale = 0f;
        GameIsPaused = true;
        backgroundMusic.Pause();
        Debug.Log("Game paused: pauseMenuUI active, Time.timeScale = 0, GameIsPaused = " + GameIsPaused);
        LogUIState();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        quitSound.Play();
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    private void LogUIState()
    {
        Debug.Log("pauseMenuUI.activeSelf: " + pauseMenuUI.activeSelf);
        CanvasGroup canvasGroup = pauseMenuUI.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Debug.Log("CanvasGroup Alpha: " + canvasGroup.alpha);
        }
        foreach (Transform child in pauseMenuUI.transform)
        {
            Debug.Log(child.name + " active: " + child.gameObject.activeSelf);
        }
        CanvasRenderer[] renderers = pauseMenuUI.GetComponentsInChildren<CanvasRenderer>();
        foreach (var renderer in renderers)
        {
            Debug.Log(renderer.gameObject.name + " alpha: " + renderer.GetAlpha());
        }
        Image img = pauseMenuUI.GetComponent<Image>();
        if (img != null)
        {
            Debug.Log("PauseMenuPanel Image color: " + img.color);
        }
    }
}