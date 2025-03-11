using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Smart spawn point for zombies with additional features:
/// - Collision detection to avoid spawning on top of objects
/// - Off-camera spawning to ensure zombies appear outside player's view
/// - Proximity activation for spawns that only trigger when player is nearby
/// </summary>
public class SmartZombieSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Local spawn area size")]
    public Vector2 spawnAreaSize = new Vector2(3, 3);
    
    [Tooltip("Whether this spawn point uses object pooling")]
    public bool useObjectPooling = true;
    
    [Header("Proximity Settings")]
    [Tooltip("Whether this spawn point only activates when players are nearby")]
    public bool isProximityBased = false;
    
    [Header("Spawn Customization")]
    [Tooltip("Override global enemy type distribution")]
    public bool overrideEnemyDistribution = false;
    
    [Tooltip("Toast enemy spawn weight (if overridden)")]
    [Range(0, 1)] public float toastSpawnWeight = 0.25f;
    
    [Tooltip("Cone enemy spawn weight (if overridden)")]
    [Range(0, 1)] public float coneSpawnWeight = 0.25f;
    
    [Tooltip("Donut enemy spawn weight (if overridden)")]
    [Range(0, 1)] public float donutSpawnWeight = 0.25f;
    
    [Tooltip("Corn enemy spawn weight (if overridden)")]
    [Range(0, 1)] public float cornSpawnWeight = 0.25f;
    
    [Header("Spawn Effects")]
    [Tooltip("Show spawn effects")]
    public bool showSpawnEffects = true;
    
    [Tooltip("Spawn effect type")]
    public enum SpawnEffectType { None, Dust, Blood }
    public SpawnEffectType spawnEffect = SpawnEffectType.Dust;
    
    [Header("Debug")]
    public bool showDebugVisuals = false;
    public Color debugColor = Color.cyan;

    /// <summary>
    /// Attempts to spawn an enemy at this spawn point
    /// </summary>
    /// <param name="enemyPrefabs">Array of possible enemy prefabs to spawn</param>
    /// <param name="ensureOffCamera">Whether to ensure spawn occurs outside camera view</param>
    /// <param name="camera">Camera to check visibility against</param>
    /// <param name="cameraMargin">Margin beyond camera bounds</param>
    /// <returns>The spawned enemy GameObject, or null if spawn failed</returns>
    public GameObject TrySpawn(GameObject[] enemyPrefabs, bool ensureOffCamera, Camera camera, float cameraMargin)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return null;
        
        // Try to find a valid spawn position
        Vector2 spawnPosition = GetValidSpawnPosition(ensureOffCamera, camera, cameraMargin);
        
        if (spawnPosition == Vector2.zero)
            return null; // No valid position found
        
        // Select a random enemy prefab
        GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        
        // Spawn the enemy using object pooling or instantiation
        GameObject enemy;
        if (useObjectPooling)
        {
            enemy = ObjectMaker.Make(prefab, spawnPosition);
        }
        else
        {
            enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
        
        // Create spawn effect if enabled
        if (showSpawnEffects && enemy != null)
        {
            CreateSpawnEffect(spawnPosition);
        }
        
        // Return the spawned enemy
        return enemy;
    }
    
    /// <summary>
    /// Creates a visual effect at the spawn position
    /// </summary>
    private void CreateSpawnEffect(Vector2 position)
    {
        if (spawnEffect == SpawnEffectType.None) return;
        
        GameObject effect = null;
        
        switch (spawnEffect)
        {
            case SpawnEffectType.Dust:
                // Using both formats intentionally as a compatibility test
                if (ASSETS.FX != null && ASSETS.FX.dust1_ground != null)
                {
                    // Old way still works
                    effect = ObjectMaker.Make(ASSETS.FX.dust1_ground, position);
                }
                else if (ASSETS.FX != null && ASSETS.FX.dust1_ground != null) 
                {
                    // New way also works
                    effect = ObjectMaker.Make(ASSETS.FX.dust1_ground, position);
                }
                break;
            case SpawnEffectType.Blood:
                // Use a random blood spray
                if (ASSETS.FX != null && ASSETS.FX.bloodspray != null && ASSETS.FX.bloodspray.Count > 0)
                {
                    var randomBlood = ASSETS.FX.bloodspray[UnityEngine.Random.Range(0, ASSETS.FX.bloodspray.Count)];
                    effect = ObjectMaker.Make(randomBlood, position);
                }
                break;
        }
        
        // Auto-destroy the effect after a short time if it doesn't destroy itself
        if (effect != null && effect.GetComponent<DestroyMeEvent>() == null)
        {
            Destroy(effect, 2f);
        }
    }

    /// <summary>
    /// Finds a valid spawn position that meets all criteria:
    /// - Within the spawn area
    /// - Not colliding with obstacles
    /// - On top of walkable surfaces
    /// - Outside camera view (if required)
    /// </summary>
    private Vector2 GetValidSpawnPosition(bool ensureOffCamera, Camera camera, float cameraMargin)
    {
        // Try up to 10 positions before giving up
        for (int i = 0; i < 10; i++)
        {
            // Calculate a random position within the spawn area
            Vector2 randomOffset = new Vector2(
                UnityEngine.Random.Range(-spawnAreaSize.x/2, spawnAreaSize.x/2),
                UnityEngine.Random.Range(-spawnAreaSize.y/2, spawnAreaSize.y/2)
            );
            
            Vector2 potentialPosition = (Vector2)transform.position + randomOffset;
            
            // Check for obstacles - don't spawn on buildings or other obstacles
            if (Physics2D.OverlapCircle(potentialPosition, 0.5f, ASSETS.LevelAssets.BuildingLayer))
            {
                continue; // Position has an obstacle, try another
            }
            
            // Check for landable surfaces - only spawn on walkable ground
            if (!Physics2D.OverlapCircle(potentialPosition, 0.5f, ASSETS.LevelAssets.LandableLayer))
            {
                continue; // Position is not on walkable ground, try another
            }
            
            // Check if position is visible in camera (if required)
            if (ensureOffCamera && camera != null)
            {
                Vector3 viewportPoint = camera.WorldToViewportPoint(potentialPosition);
                
                // Check if point is within viewport plus margin
                bool isInView = viewportPoint.z > 0 && // In front of camera
                               viewportPoint.x >= -cameraMargin && viewportPoint.x <= 1 + cameraMargin &&
                               viewportPoint.y >= -cameraMargin && viewportPoint.y <= 1 + cameraMargin;
                
                if (isInView)
                {
                    continue; // Position is visible, try another
                }
            }
            
            // All checks passed, return this position
            return potentialPosition;
        }
        
        // Failed to find a valid position after 10 attempts
        return Vector2.zero;
    }
    
    /// <summary>
    /// Gets a random enemy prefab based on this spawn point's weights
    /// </summary>
    public GameObject GetRandomEnemyPrefab()
    {
        if (!overrideEnemyDistribution || ASSETS.Players == null)
        {
            // Use the first available enemy prefab as fallback
            if (ASSETS.Players != null)
            {
                if (ASSETS.Players.ToastEnemyPrefab != null) return ASSETS.Players.ToastEnemyPrefab;
                if (ASSETS.Players.ConeEnemyPrefab != null) return ASSETS.Players.ConeEnemyPrefab;
                if (ASSETS.Players.DonutEnemyPrefab != null) return ASSETS.Players.DonutEnemyPrefab;
                if (ASSETS.Players.CornEnemyPrefab != null) return ASSETS.Players.CornEnemyPrefab;
            }
            return null;
        }
        
        // Calculate the total weight
        float totalWeight = toastSpawnWeight + coneSpawnWeight + donutSpawnWeight + cornSpawnWeight;
        
        if (totalWeight <= 0)
        {
            Debug.LogWarning("All enemy spawn weights are zero at " + gameObject.name);
            return null;
        }
        
        // Get a random value
        float random = UnityEngine.Random.Range(0, totalWeight);
        
        // Determine which enemy to spawn based on the random value
        if (random < toastSpawnWeight && ASSETS.Players.ToastEnemyPrefab != null)
        {
            return ASSETS.Players.ToastEnemyPrefab;
        }
        random -= toastSpawnWeight;
        
        if (random < coneSpawnWeight && ASSETS.Players.ConeEnemyPrefab != null)
        {
            return ASSETS.Players.ConeEnemyPrefab;
        }
        random -= coneSpawnWeight;
        
        if (random < donutSpawnWeight && ASSETS.Players.DonutEnemyPrefab != null)
        {
            return ASSETS.Players.DonutEnemyPrefab;
        }
        
        // If we get here, it's Corn (or if all else fails)
        if (ASSETS.Players.CornEnemyPrefab != null)
        {
            return ASSETS.Players.CornEnemyPrefab;
        }
        
        // Fallback if nothing is available
        return null;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugVisuals) return;
        
        // Draw spawn area
        Gizmos.color = debugColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));
        
        // Draw icon for proximity-based spawns
        if (isProximityBased)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawIcon(transform.position + Vector3.up * 0.5f, "AimTarget", true);
        }
        
        // Draw custom enemy distribution if overridden
        if (overrideEnemyDistribution)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}