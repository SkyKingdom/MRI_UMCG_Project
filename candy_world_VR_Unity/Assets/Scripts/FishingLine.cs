using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public Transform startPoint; // Reference to the starting point GameObject
    public Transform endPoint;   // Reference to the ending point GameObject
    public float curveHeight = 1.0f; // Height of the curve

    private LineRenderer lineRenderer;

    void Start()
    {
        // Ensure the LineRenderer component is attached to the current GameObject
        lineRenderer = GetComponent<LineRenderer>();

        // Check if both start and end points are provided
        if (startPoint != null && endPoint != null && lineRenderer != null)
        {
            // Set the initial positions of the LineRenderer
            lineRenderer.positionCount = 3; // Three points for the curve
            Vector3 start = startPoint.position;
            Vector3 end = endPoint.position;
            Vector3 middle = (start + end) / 2.0f;
            middle += Vector3.up * curveHeight; // Apply curve height to the middle point

            // Set the positions
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, middle);
            lineRenderer.SetPosition(2, end);
        }
        else
        {
            Debug.LogError("Please assign both start and end points, and ensure LineRenderer component is attached.");
        }
    }

    void Update()
    {
        // Update the line positions every frame in case the GameObjects move
        if (startPoint != null && endPoint != null && lineRenderer != null)
        {
            Vector3 start = startPoint.position;
            Vector3 end = endPoint.position;
            Vector3 middle = (start + end) / 2.0f;
            middle += Vector3.up * curveHeight; // Apply curve height to the middle point

            // Set the positions
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, middle);
            lineRenderer.SetPosition(2, end);
        }
    }
}