using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
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

        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        Debug.Log("Playing click sound");
        audioSource.PlayOneShot(clickSound);
    }
}