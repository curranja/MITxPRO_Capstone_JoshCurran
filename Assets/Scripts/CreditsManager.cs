using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public GameObject creditsPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (creditsPanel.activeSelf)
            {
                creditsPanel.SetActive(false);
            }
        }
    }
}