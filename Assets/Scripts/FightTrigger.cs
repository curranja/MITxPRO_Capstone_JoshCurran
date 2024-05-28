using UnityEngine;

public class FightTrigger : MonoBehaviour
{
    public Canvas bossTextCanvas; // Reference to the BossText canvas

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider entering the trigger is the player and if the trigger hasn't been activated yet
        if (other.CompareTag("Player") && !triggered)
        {
            // Trigger the events if not already triggered
            triggered = true;

            // Enable the canvas
            bossTextCanvas.gameObject.SetActive(true);
        }
    }
}