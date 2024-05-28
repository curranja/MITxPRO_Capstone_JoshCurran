using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private EventSystem pauseMenuEventSystem;

    void Awake()
    {
        pauseMenuEventSystem = GetComponent<EventSystem>();
    }

    void OnEnable()
    {
        DisableExtraEventSystems();
    }

    void DisableExtraEventSystems()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        foreach (EventSystem es in eventSystems)
        {
            if (es != pauseMenuEventSystem)
            {
                Debug.Log("Disabling extra EventSystem on " + es.gameObject.name);
                es.gameObject.SetActive(false);
            }
        }
    }
}