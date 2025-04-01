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
    
        private AstarPath pathfinder;
        private GridGraph grid;
        private Transform currentCenter;
        private float timeSinceCheck;
    
     

       

        public void StartGraphPositioning()
        {
            if (player == null)
                player = Players.AllJoinedPlayers[0].SpawnedPlayerGO.transform;
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
          

            // Initial center update

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
                currentCenter = closestCenter;
                Debug.Log("changing closest", currentCenter);
                
                // Move grid center to this position
                grid.center = closestCenter.position;
                Debug.DrawLine(player.transform.position, closestCenter.position, Color.green);
                // Update the graph
                pathfinder.Scan();
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
    }
}