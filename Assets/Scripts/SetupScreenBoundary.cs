using UnityEngine;

public class SetupScreenBoundary : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();

        Vector2[] edgePoints = new Vector2[5];
        edgePoints[0] = new Vector2(bottomLeft.x, bottomLeft.y);
        edgePoints[1] = new Vector2(bottomRight.x, bottomRight.y);
        edgePoints[2] = new Vector2(topRight.x, topRight.y);
        edgePoints[3] = new Vector2(topLeft.x, topLeft.y);
        edgePoints[4] = new Vector2(bottomLeft.x, bottomLeft.y); // Closing the loop

        edgeCollider.points = edgePoints;
    }
}