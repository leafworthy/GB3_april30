using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ColliderOverlapDetector : MonoBehaviour
{
    [Header("Target Colliders"), SerializeField]
    private Collider2D targetCollider1;
    [SerializeField] private Collider2D targetCollider2;

    [Header("Detection Settings"), SerializeField]
    private bool checkContinuously = true;
    [SerializeField] private float checkInterval = 0.1f;

    [SerializeField, Tooltip("Use bounds overlap instead of IsTouching (more reliable)")]
    private bool useBoundsOverlap = false;

    [SerializeField, Tooltip("Use OverlapCollider instead of IsTouching (most reliable)")]
    private bool useOverlapCollider = true;

    [Header("Events")]
    public UnityEvent OnBothOverlapping;
    public UnityEvent OnStoppedOverlapping;

    private Collider2D mainCharacterCollider => mainChar.GetComponent<Collider2D>();
    public GameObject mainChar;
    private bool wasOverlappingBoth;
    private float lastCheckTime;

    private void Start()
    {
        if (mainCharacterCollider == null)
        {
            Debug.LogError("ColliderOverlapDetector: No Collider2D found on " + mainChar?.name);
            enabled = false;
            return;
        }

        // Validate target colliders
        if (targetCollider1 == null || targetCollider2 == null)
            Debug.LogWarning("ColliderOverlapDetector: Target colliders not assigned on " + gameObject.name);
    }

    private void Update()
    {
        if (!checkContinuously) return;

        // Check at specified intervals to optimize performance
        if (Time.time - lastCheckTime >= checkInterval)
        {
            CheckOverlap();
            lastCheckTime = Time.time;
        }
    }

    /// <summary>
    /// Manually check if this collider is overlapping with both target colliders
    /// </summary>
    [Button("Check Overlap Now")]
    public bool CheckOverlap()
    {
        if (mainCharacterCollider == null || targetCollider1 == null || targetCollider2 == null)
            return false;

        bool overlapping1, overlapping2;

        if (useOverlapCollider)
        {
            // Method 1: Use Physics2D.OverlapCollider (most reliable)
            overlapping1 = IsOverlappingUsingOverlapCollider(mainCharacterCollider, targetCollider1);
            overlapping2 = IsOverlappingUsingOverlapCollider(mainCharacterCollider, targetCollider2);
        }
        else if (useBoundsOverlap)
        {
            // Method 2: Use bounds overlap
            overlapping1 = mainCharacterCollider.bounds.Intersects(targetCollider1.bounds);
            overlapping2 = mainCharacterCollider.bounds.Intersects(targetCollider2.bounds);
        }
        else
        {
            // Method 3: Use IsTouching (original method)
            overlapping1 = mainCharacterCollider.IsTouching(targetCollider1);
            overlapping2 = mainCharacterCollider.IsTouching(targetCollider2);
        }

        var bothOverlapping = overlapping1 && overlapping2;

        // Trigger events based on state changes
        if (bothOverlapping && !wasOverlappingBoth)
        {
            OnBothOverlapping?.Invoke();
            Debug.Log($"{mainChar.name} started overlapping both colliders!");
        }
        else if (!bothOverlapping && wasOverlappingBoth)
        {
            OnStoppedOverlapping?.Invoke();
            Debug.Log($"{mainChar.name} stopped overlapping both colliders!");
        }

        Debug.Log($"{mainChar.name} overlapping1: {overlapping1}, overlapping2: {overlapping2}, both: {bothOverlapping}");

        wasOverlappingBoth = bothOverlapping;
        return bothOverlapping;
    }

    /// <summary>
    /// Check overlap using Physics2D.OverlapCollider - most reliable method
    /// </summary>
    private bool IsOverlappingUsingOverlapCollider(Collider2D source, Collider2D target)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter();

        Collider2D[] results = new Collider2D[10]; // Buffer for results
        int count = source.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] == target)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Alternative method using Physics2D.OverlapArea
    /// </summary>
    [Button("Test OverlapArea Method")]
    public bool CheckOverlapUsingArea()
    {
        if (mainCharacterCollider == null || targetCollider1 == null || targetCollider2 == null)
            return false;

        var bounds = mainCharacterCollider.bounds;

        // Check if target colliders are within the main character's bounds
        var collider1InArea = Physics2D.OverlapArea(bounds.min, bounds.max, 1 << targetCollider1.gameObject.layer);
        var collider2InArea = Physics2D.OverlapArea(bounds.min, bounds.max, 1 << targetCollider2.gameObject.layer);

        bool overlapping1 = collider1InArea == targetCollider1;
        bool overlapping2 = collider2InArea == targetCollider2;

        Debug.Log($"OverlapArea method - overlapping1: {overlapping1}, overlapping2: {overlapping2}");

        return overlapping1 && overlapping2;
    }

    /// <summary>
    /// Check if overlapping with first target collider only
    /// </summary>
    public bool IsOverlappingFirst()
    {
        if (mainCharacterCollider == null || targetCollider1 == null) return false;

        if (useOverlapCollider)
            return IsOverlappingUsingOverlapCollider(mainCharacterCollider, targetCollider1);
        else if (useBoundsOverlap)
            return mainCharacterCollider.bounds.Intersects(targetCollider1.bounds);
        else
            return mainCharacterCollider.IsTouching(targetCollider1);
    }

    /// <summary>
    /// Check if overlapping with second target collider only
    /// </summary>
    public bool IsOverlappingSecond()
    {
        if (mainCharacterCollider == null || targetCollider2 == null) return false;

        if (useOverlapCollider)
            return IsOverlappingUsingOverlapCollider(mainCharacterCollider, targetCollider2);
        else if (useBoundsOverlap)
            return mainCharacterCollider.bounds.Intersects(targetCollider2.bounds);
        else
            return mainCharacterCollider.IsTouching(targetCollider2);
    }

    /// <summary>
    /// Set new target colliders at runtime
    /// </summary>
    public void SetTargetColliders(Collider2D collider1, Collider2D collider2)
    {
        targetCollider1 = collider1;
        targetCollider2 = collider2;
    }

    [Button("Debug Collider Info")]
    private void DebugColliderInfo()
    {
        Debug.Log($"Main Character Collider: {mainCharacterCollider?.name} - Enabled: {mainCharacterCollider?.enabled} - IsTrigger: {mainCharacterCollider?.isTrigger}");
        Debug.Log($"Target Collider 1: {targetCollider1?.name} - Enabled: {targetCollider1?.enabled} - IsTrigger: {targetCollider1?.isTrigger}");
        Debug.Log($"Target Collider 2: {targetCollider2?.name} - Enabled: {targetCollider2?.enabled} - IsTrigger: {targetCollider2?.isTrigger}");

        if (mainCharacterCollider != null && targetCollider1 != null)
        {
            var distance1 = Vector2.Distance(mainCharacterCollider.bounds.center, targetCollider1.bounds.center);
            Debug.Log($"Distance to Target 1: {distance1}");
        }

        if (mainCharacterCollider != null && targetCollider2 != null)
        {
            var distance2 = Vector2.Distance(mainCharacterCollider.bounds.center, targetCollider2.bounds.center);
            Debug.Log($"Distance to Target 2: {distance2}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visual debugging - draw lines to target colliders
        if (targetCollider1 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetCollider1.transform.position);

            // Draw the bounds of target collider 1
            Gizmos.color = Color.red * 0.3f;
            Gizmos.DrawCube(targetCollider1.bounds.center, targetCollider1.bounds.size);
        }

        if (targetCollider2 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, targetCollider2.transform.position);

            // Draw the bounds of target collider 2
            Gizmos.color = Color.blue * 0.3f;
            Gizmos.DrawCube(targetCollider2.bounds.center, targetCollider2.bounds.size);
        }

        // Draw main character collider bounds
        if (mainCharacterCollider != null)
        {
            Gizmos.color = Color.green * 0.3f;
            Gizmos.DrawCube(mainCharacterCollider.bounds.center, mainCharacterCollider.bounds.size);
        }
    }
}
