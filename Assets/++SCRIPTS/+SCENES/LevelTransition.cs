using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class manages level transitions and spawn points
[Serializable]
public class SpawnPointData
{
    public string id;
    public GameScene.Type sourceScene;
    public GameScene.Type destinationScene;
    public string connectedId; // ID of connected spawn point in destination scene
    public Vector2 position;
    public SpawnPointType pointType = SpawnPointType.Both;
    public int capacity = 4;
}

public class LevelTransition : Singleton<LevelTransition>
{
    // List of all spawn points across scenes
    [SerializeField] private List<SpawnPointData> spawnPoints = new List<SpawnPointData>();
    
    // Track the current level and spawn point
    private GameScene.Type currentScene;
    private string lastTransitionId;
    private string lastConnectedId;
    
    // Track if this is a fresh game start or a continuation between levels
    private bool isFirstLevelLoad = true;
    
    // Dictionary for faster lookup
    private Dictionary<string, SpawnPointData> spawnPointsDict = new Dictionary<string, SpawnPointData>();
    private Dictionary<GameScene.Type, List<SpawnPointData>> spawnPointsByScene = 
        new Dictionary<GameScene.Type, List<SpawnPointData>>();
    
    protected override void Awake()
    {
        base.Awake();
        
        // Only use DontDestroyOnLoad in play mode
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        
            // Initialize dictionaries
            foreach (var spawnPoint in spawnPoints)
            {
                if (!string.IsNullOrEmpty(spawnPoint.id))
                {
                    // Store by ID for direct lookup
                    spawnPointsDict[spawnPoint.id] = spawnPoint;
                    
                    // Store by scene for finding available spawn points
                    if (!spawnPointsByScene.ContainsKey(spawnPoint.sourceScene))
                    {
                        spawnPointsByScene[spawnPoint.sourceScene] = new List<SpawnPointData>();
                    }
                    spawnPointsByScene[spawnPoint.sourceScene].Add(spawnPoint);
                }
            }
        }
    }
    
    // Called when a level is loaded
    public void SetCurrentScene(GameScene.Type sceneType)
    {
        currentScene = sceneType;
        Debug.Log($"LevelTransition: Current scene set to {sceneType}");
    }
    
    // Called when transitioning between levels
    public void SetTransitionId(string transitionId)
    {
        lastTransitionId = transitionId;
        isFirstLevelLoad = false;
        
        // Store the connected ID for the destination scene
        if (spawnPointsDict.TryGetValue(transitionId, out SpawnPointData spawnData))
        {
            lastConnectedId = spawnData.connectedId;
            Debug.Log($"LevelTransition: Transition set from {transitionId} to {spawnData.connectedId} in {spawnData.destinationScene}");
        }
    }
    
    // Get spawn positions for multiple players in the current scene
    public List<Vector2> GetSpawnPositions(GameScene.Type currentSceneType, int playerCount)
    {
        // Default positions in case we don't find suitable spawn points
        List<Vector2> positions = new List<Vector2>();
        for (int i = 0; i < playerCount; i++)
        {
            positions.Add(Vector2.zero);
        }
        
        // If this is the first level load, use default spawn points
        if (isFirstLevelLoad)
        {
            return positions;
        }
        
        // Try to find the connected spawn point in the destination scene
        if (!string.IsNullOrEmpty(lastConnectedId))
        {
            SpawnPointData destinationPoint = null;
            
            // Look for exact spawn point by ID
            foreach (var point in spawnPoints)
            {
                if (point.id == lastConnectedId && 
                    point.sourceScene == currentSceneType && 
                    (point.pointType == SpawnPointType.Entry || point.pointType == SpawnPointType.Both))
                {
                    destinationPoint = point;
                    break;
                }
            }
            
            if (destinationPoint != null)
            {
                // Calculate spawn positions around this point
                return CalculatePositionsAroundPoint(destinationPoint.position, playerCount, destinationPoint.capacity);
            }
        }
        
        // If connected ID not found, look for any entry points in this scene
        if (spawnPointsByScene.TryGetValue(currentSceneType, out List<SpawnPointData> scenePoints))
        {
            var entryPoints = scenePoints
                .Where(p => p.pointType == SpawnPointType.Entry || p.pointType == SpawnPointType.Both)
                .ToList();
                
            if (entryPoints.Count > 0)
            {
                // Use the first available entry point
                return CalculatePositionsAroundPoint(
                    entryPoints[0].position, 
                    playerCount, 
                    entryPoints[0].capacity);
            }
        }
        
        // Return default positions if no suitable spawn points found
        return positions;
    }
    
    // Helper method to calculate positions around a point
    private List<Vector2> CalculatePositionsAroundPoint(Vector2 center, int count, int capacity)
    {
        List<Vector2> positions = new List<Vector2>();
        
        // Calculate positions in a small circle/cluster around this point
        int actualCount = Mathf.Min(count, capacity);
        float radius = 1.0f; // Base radius for positioning
        
        for (int i = 0; i < actualCount; i++)
        {
            if (i == 0)
            {
                // First player goes at the exact position
                positions.Add(center);
            }
            else
            {
                // Calculate positions in a circle
                float angle = i * (360f / actualCount) * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
                positions.Add(center + offset);
            }
        }
        
        // Fill remaining positions with center position if more players than capacity
        for (int i = positions.Count; i < count; i++)
        {
            positions.Add(center);
        }
        
        return positions;
    }
    
    // For backward compatibility
    public Vector2 GetSpawnPosition(GameScene.Type currentSceneType)
    {
        return GetSpawnPositions(currentSceneType, 1)[0];
    }
    
    // Find all entry points in a scene
    public List<SpawnPointData> GetEntryPoints(GameScene.Type sceneType)
    {
        if (spawnPointsByScene.TryGetValue(sceneType, out List<SpawnPointData> scenePoints))
        {
            return scenePoints
                .Where(p => p.pointType == SpawnPointType.Entry || p.pointType == SpawnPointType.Both)
                .ToList();
        }
        return new List<SpawnPointData>();
    }
    
    // Find all exit points in a scene
    public List<SpawnPointData> GetExitPoints(GameScene.Type sceneType)
    {
        if (spawnPointsByScene.TryGetValue(sceneType, out List<SpawnPointData> scenePoints))
        {
            return scenePoints
                .Where(p => p.pointType == SpawnPointType.Exit || p.pointType == SpawnPointType.Both)
                .ToList();
        }
        return new List<SpawnPointData>();
    }
    
    // Check if this is a new game or continuing between levels
    public bool IsFirstLoad()
    {
        return isFirstLevelLoad;
    }
    
    // Reset for new game
    public void ResetTransitions()
    {
        isFirstLevelLoad = true;
        lastTransitionId = string.Empty;
        lastConnectedId = string.Empty;
    }
    
    // Register a new spawn point (for editor use)
    public void RegisterSpawnPoint(SpawnPointData newSpawnPoint)
    {
        // Check if ID already exists
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i].id == newSpawnPoint.id && 
                spawnPoints[i].sourceScene == newSpawnPoint.sourceScene)
            {
                // Update existing spawn point
                spawnPoints[i] = newSpawnPoint;
                spawnPointsDict[newSpawnPoint.id] = newSpawnPoint;
                
                // Update scene dictionary
                if (spawnPointsByScene.TryGetValue(newSpawnPoint.sourceScene, out List<SpawnPointData> scenePoints))
                {
                    for (int j = 0; j < scenePoints.Count; j++)
                    {
                        if (scenePoints[j].id == newSpawnPoint.id)
                        {
                            scenePoints[j] = newSpawnPoint;
                            break;
                        }
                    }
                }
                
                return;
            }
        }
        
        // Add new spawn point
        spawnPoints.Add(newSpawnPoint);
        spawnPointsDict[newSpawnPoint.id] = newSpawnPoint;
        
        // Add to scene dictionary
        if (!spawnPointsByScene.ContainsKey(newSpawnPoint.sourceScene))
        {
            spawnPointsByScene[newSpawnPoint.sourceScene] = new List<SpawnPointData>();
        }
        spawnPointsByScene[newSpawnPoint.sourceScene].Add(newSpawnPoint);
    }
    
    // Get last connected spawn point ID
    public string GetLastConnectedId()
    {
        return lastConnectedId;
    }
}