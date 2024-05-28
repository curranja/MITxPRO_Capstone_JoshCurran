using UnityEngine;

public class PauseMenuButtons : MonoBehaviour
{
    private PauseManager pauseManager;

    void Start()
    {
        pauseManager = FindObjectOfType<PauseManager>();
    }

    public void OnResumeButton()
    {
        Debug.Log("Resume button clicked");
        if (pauseManager != null)
        {
            pauseManager.ResumeGameFromButton(); // Ensure we call a method to handle this
        }
    }

    public void OnQuitButton()
    {
        Debug.Log("Quit button clicked");
        if (pauseManager != null)
        {
            pauseManager.QuitGame();
        }
    }
}