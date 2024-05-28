using UnityEngine;
using UnityEngine.UI;

public class PauseMenuTest : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public AudioSource backgroundMusic;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel.activeSelf)
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
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        backgroundMusic.UnPause();
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        backgroundMusic.Pause();
    }
}