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
    public GameScene.Type sourceScene;
    
    [Tooltip("Scene this spawn point leads to")]
    public GameScene.Type destinationScene;
    
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
    
    [TextArea(3, 5)]
    public string notes;
    
    public override string ToString()
    {
        return !string.IsNullOrEmpty(displayName) 
            ? displayName 
            : id;
    }
    
    // Create a SpawnPointData object from this definition (for runtime use)
    public SpawnPointData CreateRuntimeData(Vector2 position)
    {
        return new SpawnPointData
        {
            id = id,
            sourceScene = sourceScene,
            destinationScene = destinationScene,
            connectedId = connectedSpawnPointId,
            position = position,
            pointType = pointType,
            capacity = capacity
        };
    }
}