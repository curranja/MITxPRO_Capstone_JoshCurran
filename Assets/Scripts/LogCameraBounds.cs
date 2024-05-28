using UnityEngine;

public class LogCameraBounds : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        UnityEngine.Debug.Log("Bottom Left: " + bottomLeft);
        UnityEngine.Debug.Log("Bottom Right: " + bottomRight);
        UnityEngine.Debug.Log("Top Left: " + topLeft);
        UnityEngine.Debug.Log("Top Right: " + topRight);
    }
}