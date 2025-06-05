using System.Collections.Generic;
using __SCRIPTS.Plugins.AstarPathfindingProject.Core;
using __SCRIPTS.Plugins.AstarPathfindingProject.Generators;
using UnityEngine;
using VInspector;

namespace __SCRIPTS
{
    public class GraphNodePositioner : MonoBehaviour
    {
        [Header("References")]
        public Transform player;
        public List<Transform> potentialCenters = new List<Transform>();
    
        [Header("Settings")]
        public string centerObjectsTag = "GraphCenter"; // Optional: find centers by tag
        public float checkInterval = 0.5f; // How often to check for closest node
        public bool useGraphCaching = true; // Toggle for enabling/disabling caching
        
        [Header("Debug")]
        public bool logCacheInfo = true;
    
        private AstarPath pathfinder;
        private GridGraph grid;
        private Transform currentCenter;
        private float timeSinceCheck;
        
        // Dictionary to keep track of which centers we've already scanned
        private Dictionary<int, bool> scannedCenters = new Dictionary<int, bool>();
        
        public void StartGraphPositioning()
        {
            if (player == null)
                player = Players.I.AllJoinedPlayers[0].SpawnedPlayerGO.transform;
            if (player == null)
            {
                Debug.LogError("No player found in scene!");
                enabled = false;
                return;
            }
            
            // Find AstarPath
            pathfinder = AstarPath.active;
            if (pathfinder == null)
            {
                Debug.LogError("No AstarPath found in the scene!");
                enabled = false;
                return;
            }

            // Get the active grid graph without changing any settings
            if (pathfinder.data.graphs.Length > 0)
            {
                foreach (NavGraph graph in pathfinder.data.graphs)
                {
                    if (graph is GridGraph)
                    {
                        grid = graph as GridGraph;
                        break;
                    }
                }
            }

            if (grid == null)
            {
                Debug.LogError("No GridGraph found in scene!");
                enabled = false;
                return;
            }

            FindCenters();
            UpdateGraphCenter();
        }

        [Button()]
        public void FindCenters()
        {
            potentialCenters.Clear();
            GameObject[] taggedCenters = GameObject.FindGameObjectsWithTag(centerObjectsTag);
            foreach (GameObject go in taggedCenters)
            {
                potentialCenters.Add(go.transform);
            }
        }

        private void Update()
        {
            timeSinceCheck += Time.deltaTime;
        
            if (timeSinceCheck >= checkInterval)
            {
                timeSinceCheck = 0f;
                UpdateGraphCenter();
            }
        }
    
        private void UpdateGraphCenter()
        {
            if (potentialCenters.Count == 0 || player == null)
                return;
            
            // Find closest center to player
            Transform closestCenter = FindClosestCenter();
            if (closestCenter == null) return;
            
            // Only update if the closest center has changed
            if (closestCenter != currentCenter)
            {
                // Update the current center reference
                currentCenter = closestCenter;
                int centerId = currentCenter.GetInstanceID();
                
                // Check if we've already scanned this center
                if (useGraphCaching && scannedCenters.TryGetValue(centerId, out bool hasScanned) && hasScanned)
                {
                    // This center has already been scanned, we can just update the center position 
                    // without doing a full scan
                    grid.center = currentCenter.position;
                    
                    // We don't need to call Scan() again, the graph is already updated for this center
                    if (logCacheInfo)
                    {
                        Debug.Log("Using cached graph for center: " + currentCenter.name);
                    }
                }
                else
                {
                    // First time visiting this center, do a full scan
                    grid.center = currentCenter.position;
                    pathfinder.Scan();
                    
                    // Mark this center as scanned
                    if (useGraphCaching)
                    {
                        scannedCenters[centerId] = true;
                        if (logCacheInfo)
                        {
                            Debug.Log("Scanned and cached graph for center: " + currentCenter.name);
                        }
                    }
                }
            }
        }
    
        private Transform FindClosestCenter()
        {
            if (potentialCenters.Count == 0) return null;
            Transform closest = potentialCenters[0];
            if (closest == null) return null;
            float closestDist = Vector3.Distance(player.position, closest.position);
        
            for (int i = 1; i < potentialCenters.Count; i++)
            {
                if (potentialCenters[i] == null)
                    continue;
                
                float dist = Vector3.Distance(player.position, potentialCenters[i].position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = potentialCenters[i];
                }
            }
        
            return closest;
        }
        
        // Clear cache on disable
        private void OnDisable()
        {
            scannedCenters.Clear();
        }
        
        // Method to manually clear the cache if needed
        [Button()]
        public void ClearCache()
        {
            scannedCenters.Clear();
            Debug.Log("Graph cache cleared");
        }
    }
}
