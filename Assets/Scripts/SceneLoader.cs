using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    private AudioSource audioSource; // Reference to the AudioSource component

    private void Awake()
    {
        // Ensure this object persists across scene loads
        DontDestroyOnLoad(gameObject);

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundAndLoadScene(string sceneName)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            // Play the sound clip
            audioSource.Play();
        }

        // Load the scene after a short delay to allow the sound to play
        StartCoroutine(LoadSceneAfterSound(sceneName));
    }

    private IEnumerator LoadSceneAfterSound(string sceneName)
    {
        // Wait for the duration of the sound clip
        yield return new WaitForSeconds(audioSource.clip.length);

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }
}