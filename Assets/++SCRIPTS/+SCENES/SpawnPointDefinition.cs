using UnityEngine;

/// <summary>
/// ScriptableObject to define spawn point data that can be reused across scenes
/// </summary>
[CreateAssetMenu(fileName = "New Spawn Point", menuName = "Gangsta Bean/Spawn Point Definition", order = 1)]
public class SpawnPointDefinition : ScriptableObject
{
    [Header("Identification")]
    [Tooltip("Unique identifier for this spawn point")]
    public string id;
    
    [Tooltip("User-friendly name to show in dropdown lists")]
    public string displayName;

    [Header("Connection")]
    [Tooltip("Scene this spawn point is in")]
    [SerializeField] private SceneDefinition sourceScene;
    
    [Tooltip("Scene this spawn point leads to")]
    [SerializeField] private SceneDefinition destinationScene;
    
    [Tooltip("Connected spawn point ID in the destination scene")]
    public string connectedSpawnPointId;
    
    [Header("Configuration")]
    [Tooltip("Defines if this is an entry point, exit point, or both")]
    public SpawnPointType pointType = SpawnPointType.Both;
    
    [Tooltip("Capacity - how many players can use this spawn point simultaneously")]
    public int capacity = 4;
    
    [Header("Visualization")]
    [Tooltip("Color to use when visualizing this spawn point")]
    public Color gizmoColor = new Color(0, 0.5f, 1f, 0.5f);
    
    /// <summary>
    /// The source scene this spawn point belongs to
    /// </summary>
    public SceneDefinition SourceScene => sourceScene;
    
    /// <summary>
    /// The destination scene this spawn point leads to
    /// </summary>
    public SceneDefinition DestinationScene => destinationScene;
    
    /// <summary>
    /// Set the source scene for this spawn point
    /// </summary>
    public void SetSourceScene(SceneDefinition scene)
    {
        if (scene != null)
        {
            sourceScene = scene;
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
    /// String representation for display in the inspector 
    /// </summary>
    public override string ToString()
    {
        if (!string.IsNullOrEmpty(displayName))
            return displayName;
            
        if (!string.IsNullOrEmpty(id))
            return id;
            
        return name;
    }
    
    /// <summary>
    /// Helper method to create a spawn point definition from source data
    /// </summary>
    public static SpawnPointDefinition Create(string id, SceneDefinition sourceScene, SceneDefinition destScene, 
        string connectedId = "", SpawnPointType type = SpawnPointType.Both)
    {
        var definition = CreateInstance<SpawnPointDefinition>();
        definition.id = id;
        definition.displayName = id;
        definition.SetSourceScene(sourceScene);
        definition.SetDestinationScene(destScene);
        definition.connectedSpawnPointId = connectedId;
        definition.pointType = type;
        
        return definition;
    }
}