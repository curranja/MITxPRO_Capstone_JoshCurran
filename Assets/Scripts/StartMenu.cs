using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public AudioClip clickSound;
    private static AudioSource audioSource;

    void Start()
    {
        // Find the StartMenuClickSound GameObject or create one if it doesn't exist
        GameObject startMenuClickSoundGO = GameObject.Find("StartMenuClickSound");
        if (startMenuClickSoundGO == null)
        {
            startMenuClickSoundGO = new GameObject("StartMenuClickSound");
            audioSource = startMenuClickSoundGO.AddComponent<AudioSource>();
            DontDestroyOnLoad(startMenuClickSoundGO);
        }
        else
        {
            audioSource = startMenuClickSoundGO.GetComponent<AudioSource>();
        }

        // Attach button click listeners
        Button startButton = GetComponent<Button>();
        startButton.onClick.AddListener(StartGame);
        startButton.onClick.AddListener(PlayClickSound);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("InstructionSlide");
    }

    void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void OpenOptions()
    {
        Debug.Log("Options button clicked");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit button clicked");
    }
}