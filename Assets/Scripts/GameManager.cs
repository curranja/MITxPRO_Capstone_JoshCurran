using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make sure this GameObject persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to reset game progress
    public void ResetGame()
    {
        // Reset game progress here
        // For example:
        // PlayerPrefs.DeleteAll(); // if you're using PlayerPrefs
        // Reset any static variables or game state data
        Debug.Log("Game progress has been reset.");
    }
}