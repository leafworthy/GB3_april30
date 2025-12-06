using UnityEngine;
using System.Collections.Generic;

public class LineComparisonVisualizer : MonoBehaviour
{
    [Header("Line Sources")]
    public List<Transform> lineStarts;   // index i = start of line i
    public List<Transform> lineEnds;     // index i = end of line i

    [Header("Gizmo Settings")]
    public Color lineColor = Color.white;
    public Color lessColor = Color.blue;
    public Color equalColor = Color.green;
    public Color greaterColor = Color.red;

    public float neighborSearchRadius = 5f;
    public bool drawNeighbors = true;

    private void OnDrawGizmos()
    {
        if (lineStarts == null || lineEnds == null || lineStarts.Count == 0)
            return;

        int count = Mathf.Min(lineStarts.Count, lineEnds.Count);

        // 1. Always draw the line segments themselves
        Gizmos.color = lineColor;
        for (int i = 0; i < count; i++)
        {
            Vector3 a = GetLineStart(i);
            Vector3 b = GetLineEnd(i);
            Gizmos.DrawLine(a, b);
        }

        if (!drawNeighbors)
            return;

        // 2. Draw comparisons only to nearest neighbors
        for (int i = 0; i < count; i++)
        {
            int neighbor = FindClosestLine(i, count);
            if (neighbor < 0) continue;

            Vector3 a1 = GetLineStart(i);
            Vector3 a2 = GetLineEnd(i);

            Vector3 b1 = GetLineStart(neighbor);
            Vector3 b2 = GetLineEnd(neighbor);

            // Your actual comparison function
            int comp = CompareLineAndLine(i, neighbor);

            Gizmos.color = comp < 0 ? lessColor :
                           comp > 0 ? greaterColor :
                                      equalColor;

            // Draw a small "comparison connector" between midpoints
            Vector3 midA = (a1 + a2) * 0.5f;
            Vector3 midB = (b1 + b2) * 0.5f;
            Gizmos.DrawLine(midA, midB);
        }
    }

    private int FindClosestLine(int i, int count)
    {
        Vector3 aMid = (GetLineStart(i) + GetLineEnd(i)) * 0.5f;

        float best = neighborSearchRadius * neighborSearchRadius;
        int bestIndex = -1;

        for (int j = 0; j < count; j++)
        {
            if (j == i) continue;

            Vector3 bMid = (GetLineStart(j) + GetLineEnd(j)) * 0.5f;
            float d = (bMid - aMid).sqrMagnitude;

            if (d < best)
            {
                best = d;
                bestIndex = j;
            }
        }

        return bestIndex;
    }

    // --------------------------------------------
    // HOOK THESE INTO YOUR REAL DATA
    // --------------------------------------------
    private Vector3 GetLineStart(int i) => lineStarts[i].position;
    private Vector3 GetLineEnd(int i) => lineEnds[i].position;

    // Replace with your geometry comparison
    private int CompareLineAndLine(int i, int j)
    {
        // TEMP placeholder — replace with your real comparison call
        // Return <0 if line i < line j, etc.
        return 0;
    }
}
