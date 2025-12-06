using UnityEngine;
using System.Collections.Generic;

public static class ColliderOverlapUtility
{
    /// <summary>
    /// Checks if a collider is overlapping with specific other colliders.
    /// </summary>
    /// <param name="checkCollider">The collider to check overlaps for</param>
    /// <param name="targetColliders">The colliders to check against</param>
    /// <param name="foundColliders">Optional: outputs which specific colliders were found</param>
    /// <param name="maxResults">Maximum number of overlaps to check (default: 25)</param>
    /// <returns>True if overlapping with ALL target colliders</returns>
    public static bool IsOverlappingAll(Collider2D checkCollider, Collider2D[] targetColliders,
        out List<Collider2D> foundColliders, int maxResults = 25)
    {
        foundColliders = new List<Collider2D>();

        if (checkCollider == null || targetColliders == null || targetColliders.Length == 0)
            return false;

        // Setup filter to check all collisions
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Collider2D[] results = new Collider2D[maxResults];
        int count = checkCollider.Overlap(filter, results);

        // Check which target colliders are overlapping
        HashSet<Collider2D> targetSet = new HashSet<Collider2D>(targetColliders);
        for (int i = 0; i < count; i++)
        {
            if (targetSet.Contains(results[i]))
            {
                foundColliders.Add(results[i]);
            }
        }

        // Return true only if ALL targets were found
        return foundColliders.Count == targetColliders.Length;
    }

    /// <summary>
    /// Checks if a collider is overlapping with ANY of the specified colliders.
    /// </summary>
    public static bool IsOverlappingAny(Collider2D checkCollider, Collider2D[] targetColliders,
        out List<Collider2D> foundColliders, int maxResults = 25)
    {
        foundColliders = new List<Collider2D>();

        if (checkCollider == null || targetColliders == null || targetColliders.Length == 0)
            return false;

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Collider2D[] results = new Collider2D[maxResults];
        int count = checkCollider.Overlap(filter, results);

        HashSet<Collider2D> targetSet = new HashSet<Collider2D>(targetColliders);
        for (int i = 0; i < count; i++)
        {
            if (targetSet.Contains(results[i]))
            {
                foundColliders.Add(results[i]);
            }
        }

        return foundColliders.Count > 0;
    }

    /// <summary>
    /// Simple version that just returns true/false without details.
    /// </summary>
    public static bool IsOverlappingAll(Collider2D checkCollider, params Collider2D[] targetColliders)
    {
        List<Collider2D> dummy;
        return IsOverlappingAll(checkCollider, targetColliders, out dummy);
    }

    /// <summary>
    /// Gets all colliders overlapping with the given collider.
    /// </summary>
    public static List<Collider2D> GetAllOverlapping(Collider2D checkCollider, int maxResults = 25)
    {
        List<Collider2D> overlapping = new List<Collider2D>();

        if (checkCollider == null)
            return overlapping;

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Collider2D[] results = new Collider2D[maxResults];
        int count = checkCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i] != checkCollider)
                overlapping.Add(results[i]);
        }

        return overlapping;
    }

    /// <summary>
    /// Gets all colliders overlapping with a specific layer mask.
    /// </summary>
    public static List<Collider2D> GetAllOverlappingOnLayer(Collider2D checkCollider, LayerMask layerMask, int maxResults = 25)
    {
        List<Collider2D> overlapping = new List<Collider2D>();

        if (checkCollider == null)
            return overlapping;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(layerMask);
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[maxResults];
        int count = checkCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i] != checkCollider)
                overlapping.Add(results[i]);
        }

        return overlapping;
    }
}

// USAGE EXAMPLES:
/*

// Example 1: Check if overlapping both A and B
public class YourClass : MonoBehaviour
{
    [SerializeField] private Collider2D otherA;
    [SerializeField] private Collider2D otherB;

    void CheckOverlap()
    {
        Collider2D myCollider = GetComponent<Collider2D>();

        // Simple check
        if (ColliderOverlapUtility.IsOverlappingAll(myCollider, otherA, otherB))
        {
            Debug.Log("Overlapping both!");
        }

        // Get details about which were found
        List<Collider2D> found;
        if (ColliderOverlapUtility.IsOverlappingAll(myCollider, new[] { otherA, otherB }, out found))
        {
            Debug.Log($"Found {found.Count} colliders");
        }
    }
}

// Example 2: Check if overlapping ANY of the targets
void CheckAny()
{
    Collider2D myCollider = GetComponent<Collider2D>();
    List<Collider2D> found;

    if (ColliderOverlapUtility.IsOverlappingAny(myCollider, new[] { otherA, otherB }, out found))
    {
        Debug.Log($"Overlapping {found.Count} of the targets");
        foreach (var col in found)
        {
            Debug.Log($"Found: {col.gameObject.name}");
        }
    }
}

// Example 3: Get all overlapping colliders
void GetAll()
{
    Collider2D myCollider = GetComponent<Collider2D>();
    var all = ColliderOverlapUtility.GetAllOverlapping(myCollider);

    Debug.Log($"Overlapping {all.Count} colliders total");
}

// Example 4: Get overlapping colliders on specific layer
void GetByLayer()
{
    Collider2D myCollider = GetComponent<Collider2D>();
    LayerMask enemyLayer = LayerMask.GetMask("Enemy");
    var enemies = ColliderOverlapUtility.GetAllOverlappingOnLayer(myCollider, enemyLayer);

    Debug.Log($"Overlapping {enemies.Count} enemies");
}

// Example 5: Your original use case with continuous checking
public class OverlapTest : MonoBehaviour
{
    [SerializeField] private Collider2D otherA;
    [SerializeField] private Collider2D otherB;
    public GameObject mainPlayerGO;
    public bool checkContinuously = true;
    public float checkInterval = 0.5f;

    private float lastCheckTime;

    private void Update()
    {
        if (!checkContinuously) return;

        if (Time.time - lastCheckTime >= checkInterval)
        {
            CheckOverlap();
            lastCheckTime = Time.time;
        }
    }

    private void CheckOverlap()
    {
        mainPlayerGO = Services.playerManager?.mainPlayer?.SpawnedPlayerGO;
        if (mainPlayerGO == null)
        {
            Debug.Log("can't find player");
            return;
        }

        Collider2D myCollider = mainPlayerGO.GetComponent<Collider2D>();
        List<Collider2D> found;

        bool overlappingAll = ColliderOverlapUtility.IsOverlappingAll(
            myCollider,
            new[] { otherA, otherB },
            out found
        );

        Debug.Log($"foundA: {found.Contains(otherA)}, foundB: {found.Contains(otherB)}");

        if (overlappingAll)
        {
            Debug.Log("Overlapping both colliders!");
        }
    }
}

*/
