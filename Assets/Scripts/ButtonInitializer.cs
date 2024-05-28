using UnityEngine;
using UnityEngine.UI;

public class ButtonInitializer : MonoBehaviour
{
    [System.Serializable]
    public class ButtonSoundPair
    {
        public Button targetButton; // The button to assign the event to
        public AudioSource audioSource; // AudioSource to play the sound
        public AudioClip soundClip; // Sound clip to play
        public string sceneName; // The name of the scene to load
    }

    public ButtonSoundPair[] buttonSoundPairs; // Array to hold multiple button-sound pairs

    void Start()
    {
        // Find the SceneLoaderObject in the scene
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader != null)
        {
            foreach (var pair in buttonSoundPairs)
            {
                if (pair.targetButton != null && !string.IsNullOrEmpty(pair.sceneName))
                {
                    // Clear any existing onClick listeners
                    pair.targetButton.onClick.RemoveAllListeners();

                    // Add the PlaySoundAndLoadScene method to the button's onClick event
                    pair.targetButton.onClick.AddListener(() =>
                    {
                        if (pair.audioSource != null && pair.soundClip != null)
                        {
                            pair.audioSource.PlayOneShot(pair.soundClip);
                        }
                        sceneLoader.PlaySoundAndLoadScene(pair.sceneName);
                    });
                }
            }
        }
        else
        {
            Debug.LogError("SceneLoaderObject not found in the scene.");
        }
    }
}