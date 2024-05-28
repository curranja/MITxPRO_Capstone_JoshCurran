using UnityEngine;

public class SceneLoaderInitializer : MonoBehaviour
{
    private static bool isInitialized = false;

    void Awake()
    {
        if (!isInitialized)
        {
            DontDestroyOnLoad(gameObject);
            isInitialized = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}