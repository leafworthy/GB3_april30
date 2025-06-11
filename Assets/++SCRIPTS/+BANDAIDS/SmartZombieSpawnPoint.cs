using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// Simplified spawn point that works with the new SmartZombieSpawningSystem.
    /// Provides basic collision detection and distance validation.
    /// </summary>
    public class SmartZombieSpawnPoint : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Vector2 spawnAreaSize = new Vector2(3, 3);
        [SerializeField] private float minDistanceFromPlayer = 5f;
        [SerializeField] private float maxDistanceFromPlayer = 30f;
        [SerializeField] private bool isActive = true;
        [SerializeField] private int maxSpawnAttempts = 10;
        
        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.red;
        
        // Events for the spawning system to track
        public System.Action OnZombieSpawned;
        public System.Action OnZombieDied;
        
        public bool CanSpawn()
        {
            if (!isActive)
                return false;
                
            // Check if any player is within valid distance range
            if (Players.I != null && Players.I.AllJoinedPlayers.Count > 0)
            {
                bool hasValidPlayer = false;
                foreach (var player in Players.I.AllJoinedPlayers)
                {
                    if (player != null)
                    {
                        float distance = Vector3.Distance(transform.position, player.transform.position);
                        if (distance >= minDistanceFromPlayer && distance <= maxDistanceFromPlayer)
                        {
                            hasValidPlayer = true;
                            break;
                        }
                    }
                }
                
                if (!hasValidPlayer)
                    return false;
            }
            
            return true;
        }
        
        public bool TrySpawn()
        {
            if (!CanSpawn())
                return false;
            
            Vector2 spawnPosition = GetValidSpawnPosition();
            if (spawnPosition == Vector2.zero)
                return false;
                
            // Notify that a zombie was spawned at this point
            OnZombieSpawned?.Invoke();
            return true;
        }
        
        private Vector2 GetValidSpawnPosition()
        {
            // Try up to maxSpawnAttempts positions before giving up
            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                Vector2 potentialPosition = (Vector2)transform.position + new Vector2(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
                );
                
                // Check for obstacles - don't spawn on buildings or other obstacles
                if (ASSETS.LevelAssets?.BuildingLayer != null && 
                    Physics2D.OverlapCircle(potentialPosition, 0.5f, ASSETS.LevelAssets.BuildingLayer))
                {
                    continue; // Position has an obstacle, try another
                }
                
                // All checks passed, return this position
                return potentialPosition;
            }
            
            // Failed to find a valid position after all attempts
            return Vector2.zero;
        }
        
        public void SetActive(bool active)
        {
            isActive = active;
        }
        
        public bool IsActive()
        {
            return isActive;
        }
        
        // Called when a zombie dies that was spawned from this point
        public void NotifyZombieDied()
        {
            OnZombieDied?.Invoke();
        }
        
        void OnDrawGizmos()
        {
            if (!showGizmos)
                return;
                
            // Draw spawn point
            Gizmos.color = isActive ? gizmoColor : Color.gray;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0));
            
            // Draw distance ranges
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.2f);
            Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);
            Gizmos.DrawWireSphere(transform.position, maxDistanceFromPlayer);
        }
        
        void OnDrawGizmosSelected()
        {
            if (!showGizmos)
                return;
                
            // Highlight when selected
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));
            
            // Draw distance ranges more prominently when selected
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxDistanceFromPlayer);
        }
    }
}