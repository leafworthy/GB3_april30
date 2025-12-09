using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class PathMover : MonoBehaviour
{
	PolygonCollider2D _polygonCollider;
	PolygonCollider2D polygonCollider => _polygonCollider ??= GetComponent<PolygonCollider2D>();

	List<Vector2> originalPathPoints = new();

    public GameObject puller;
    public Vector2 pullerOriginalPosition;

    [SerializeField] int pathIndex = 1; // Most common case is path 0
    [SerializeField] bool updateInRealTime = true;

    void Start()
    {
        if (puller == null) return;
        CapturePointPositions(); // Automatically capture on start
    }

    void Update()
    {
        if (updateInRealTime && puller != null && originalPathPoints.Count > 0)
        {
            UpdatePath();
        }
    }

    [Button]
    public void CapturePointPositions()
    {
        if (puller == null || polygonCollider == null) return;

        pullerOriginalPosition = puller.transform.position;
        originalPathPoints.Clear();

        // Get the path points in local space
        var tempPoints = new List<Vector2>();
        polygonCollider.GetPath(pathIndex, tempPoints);

        // Store the original local space points
        originalPathPoints.AddRange(tempPoints);

        Debug.Log($"Captured {originalPathPoints.Count} path points for path index {pathIndex}");
    }

    [Button]
    public void UpdatePath()
    {
        if (puller == null || polygonCollider == null || originalPathPoints.Count == 0) return;

        // Calculate the difference in world space
        var currentPullerPos = (Vector2)puller.transform.position;
        var worldDiff = currentPullerPos - pullerOriginalPosition;

        // Convert world space difference to local space of the polygon collider
        Vector2 localDiff = polygonCollider.transform.InverseTransformVector(worldDiff);

        // Apply the local space difference to the original points
        var newPath = new Vector2[originalPathPoints.Count];
        for (var i = 0; i < originalPathPoints.Count; i++)
        {
            newPath[i] = originalPathPoints[i] + localDiff;
        }

        polygonCollider.SetPath(pathIndex, newPath);
    }

    [Button]
    public void ResetPath()
    {
        if (polygonCollider == null || originalPathPoints.Count == 0) return;

        // Reset to original positions
        polygonCollider.SetPath(pathIndex, originalPathPoints.ToArray());

        // Reset puller position
        if (puller != null)
        {
            puller.transform.position = pullerOriginalPosition;
        }
    }

    // Helper method to visualize the path points in the scene view
    void OnDrawGizmos()
    {
        if (polygonCollider == null || originalPathPoints.Count == 0) return;

        Gizmos.color = Color.yellow;
        var tempPoints = new List<Vector2>();
        polygonCollider.GetPath(pathIndex, tempPoints);

        for (int i = 0; i < tempPoints.Count; i++)
        {
            var worldPoint = polygonCollider.transform.TransformPoint(tempPoints[i]);
            Gizmos.DrawWireSphere(worldPoint, 0.1f);

            // Draw lines between consecutive points
            if (i < tempPoints.Count - 1)
            {
                var nextWorldPoint = polygonCollider.transform.TransformPoint(tempPoints[i + 1]);
                Gizmos.DrawLine(worldPoint, nextWorldPoint);
            }
        }

        // Draw puller position
        if (puller != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puller.transform.position, 0.2f);
        }
    }
}
