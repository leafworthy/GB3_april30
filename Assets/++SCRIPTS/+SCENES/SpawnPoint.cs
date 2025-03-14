using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines whether a spawn point is an entry, exit or both
/// </summary>
public enum SpawnPointType
{
    Entry,    // Where players appear when entering this scene
    Exit,     // Where players exit to go to another scene
    Both      // Functions as both entry and exit
}

/// <summary>
/// Defines a spawn point in a scene, used for level transitions and player positioning
/// </summary>
[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [Tooltip("Unique identifier for this spawn point")]
    public string id;
    
    [Tooltip("Scene this spawn point is in")]
    [SerializeField] private SceneDefinition currentScene;
    
    [Tooltip("Defines if this is an entry point, exit point, or both")]
    public SpawnPointType pointType = SpawnPointType.Both;
    
    [Tooltip("Scene this spawn point leads to (for Exit points)")]
    [SerializeField] private SceneDefinition destinationScene;
    
    [Tooltip("Connected spawn point ID in the destination scene (for Exit points)")]
    public string connectedSpawnPointId;
    
    [Tooltip("Capacity - how many players can use this spawn point simultaneously")]
    public int capacity = 4;
    
    [Header("Visualization")]
    public Color gizmoColor = new Color(0, 0.5f, 1f, 0.5f); // Blue for default
    public float gizmoSize = 1.0f;
    
    // Reference to the definition this spawn point is based on (used in editor)
    [HideInInspector]
    public SpawnPointDefinition definition;
    
    // Properties for scene access
    public SceneDefinition CurrentScene => 
        currentScene != null && currentScene.IsValid() ? 
            currentScene : 
            GetCurrentSceneDefinition();
    
    public SceneDefinition DestinationScene => 
        destinationScene != null && destinationScene.IsValid() ? 
            destinationScene : 
            null;
    
    // Cache nearby SpawnPoints to visualize connections in editor
    private List<SpawnPoint> cachedConnections = new List<SpawnPoint>();
    
    /// <summary>
    /// Set the current scene for this spawn point
    /// </summary>
    public void SetCurrentScene(SceneDefinition scene)
    {
        if (scene != null)
        {
            currentScene = scene;
        }
    }
    
    /// <summary>
    /// Set the destination scene for this spawn point
    /// </summary>
    public void SetDestinationScene(SceneDefinition scene)
    {
        if (scene != null)
        {
            destinationScene = scene;
        }
    }
    
    /// <summary>
    /// Get the current scene definition (from active scene if not set)
    /// </summary>
    private SceneDefinition GetCurrentSceneDefinition()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return ASSETS.GetSceneByName(sceneName);
    }
    
    void OnDrawGizmos()
    {
        // Choose color based on spawn point type
        Color displayColor = gizmoColor;
        if (definition != null)
        {
            // If we have a definition, use its color
            displayColor = definition.gizmoColor;
        }
        else
        {
            // Default colors based on type if no custom color
            switch (pointType)
            {
                case SpawnPointType.Entry:
                    displayColor = new Color(0, 1, 0, 0.5f); // Green for entry
                    break;
                case SpawnPointType.Exit:
                    displayColor = new Color(1, 0.5f, 0, 0.5f); // Orange for exit
                    break;
                case SpawnPointType.Both:
                    displayColor = new Color(0, 0.5f, 1, 0.5f); // Blue for both
                    break;
            }
        }
        
        Gizmos.color = displayColor;
        
        // Draw a visual representation of the spawn point
        Gizmos.DrawSphere(transform.position, gizmoSize);
        
        // Draw spawn point ID and additional info as text
        #if UNITY_EDITOR
        string labelText = id;
        if (definition != null && !string.IsNullOrEmpty(definition.displayName))
        {
            labelText = definition.displayName;
        }
        
        if (pointType == SpawnPointType.Exit || pointType == SpawnPointType.Both)
        {
            string destSceneName = 
                destinationScene != null && destinationScene.IsValid() ? 
                destinationScene.DisplayName : 
                "Unknown";
                
            labelText += " → " + destSceneName;
            if (!string.IsNullOrEmpty(connectedSpawnPointId))
            {
                labelText += " (" + connectedSpawnPointId + ")";
            }
        }
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * gizmoSize, labelText);
        
        // Draw connections to other spawn points
        DrawConnections();
        #endif
    }
    
    #if UNITY_EDITOR
    private void DrawConnections()
    {
        // Don't draw in play mode for performance
        if (Application.isPlaying)
            return;
        
        // Handle connections within the same scene
        if (cachedConnections.Count == 0)
        {
            // Find all spawn points in the scene
            RefreshConnectionCache();
        }
        
        // Draw connections for exit points
        if (pointType == SpawnPointType.Exit || pointType == SpawnPointType.Both)
        {
            var destination = DestinationScene;
            if (destination == null)
                return;
                
            foreach (var otherPoint in cachedConnections)
            {
                var otherScene = otherPoint.CurrentScene;
                if (otherScene == null) 
                    continue;
                    
                // Check if scenes match
                bool scenesMatch = destination.Equals(otherScene);
                if (otherPoint != this && scenesMatch && otherPoint.id == connectedSpawnPointId)
                {
                    // Connected point found in editor, draw connection
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, otherPoint.transform.position);
                    
                    // Draw arrow indicator in the middle
                    Vector3 midPoint = (transform.position + otherPoint.transform.position) / 2;
                    Gizmos.DrawSphere(midPoint, 0.3f);
                    break;
                }
            }
        }
    }
    
    private void RefreshConnectionCache()
    {
        cachedConnections.Clear();
        var allSpawnPoints = UnityEngine.Object.FindObjectsOfType<SpawnPoint>(true);
        cachedConnections.AddRange(allSpawnPoints);
    }

    #endif
    
    /// <summary>
    /// Register this spawn point with the SceneLoader manager
    /// </summary>
    public void RegisterSpawnPoint()
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Spawn point ID cannot be empty", this);
            return;
        }
        
        // Only register in play mode
        if (!Application.isPlaying)
            return;
        
        // Get source scene
        var sourceScene = CurrentScene;
        if (sourceScene == null)
        {
            Debug.LogError($"Spawn point {id} has no source scene defined", this);
            return;
        }
        
        // Get destination scene
        var destScene = DestinationScene; 
        string destSceneName = destScene != null ? destScene.SceneName : string.Empty;
        
        // Create spawn point data
        SpawnPointData spawnData = new SpawnPointData
        {
            id = id,
            sourceSceneName = sourceScene.SceneName,
            destinationSceneName = destSceneName,
            connectedId = connectedSpawnPointId,
            position = transform.position,
            pointType = pointType,
            capacity = capacity
        };
        
        // Register with SceneLoader
        if (SceneLoader.I != null)
        {
            SceneLoader.I.RegisterSpawnPoint(spawnData);
        }
    }
    
    void Start()
    {
        if (Application.isPlaying)
        {
            RegisterSpawnPoint();
            
            // Make sure the SpawnPointManager exists
            if (SpawnPointManager.Instance != null)
            {
                SpawnPointManager.Instance.GetAllDefinitions();
            }
        }
    }
    
    /// <summary>
    /// Get spawn positions for multiple players
    /// </summary>
    public List<Vector2> GetSpawnPositionsForPlayers(int playerCount)
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 basePosition = transform.position;
        
        // Calculate positions in a small circle/cluster around this point
        int actualCount = Mathf.Min(playerCount, capacity);
        float radius = 1.0f; // Base radius for positioning
        
        for (int i = 0; i < actualCount; i++)
        {
            if (i == 0)
            {
                // First player goes at the exact position
                positions.Add(basePosition);
            }
            else
            {
                // Calculate positions in a circle
                float angle = i * (360f / actualCount) * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
                positions.Add(basePosition + offset);
            }
        }
        
        return positions;
    }
    
    /// <summary>
    /// Apply a definition to this spawn point
    /// </summary>
    public void ApplyDefinition(SpawnPointDefinition def)
    {
        if (def == null) 
            return;
        
        // Store reference
        definition = def;
        
        // Apply properties
        id = def.id;
        
        // Set scenes from definition
        SetCurrentScene(def.SourceScene);
        SetDestinationScene(def.DestinationScene);
        
        // Apply other properties
        connectedSpawnPointId = def.connectedSpawnPointId;
        pointType = def.pointType;
        capacity = def.capacity;
        gizmoColor = def.gizmoColor;
        
        // Update visuals
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}