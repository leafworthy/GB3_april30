using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages spawn point definitions loaded from Resources
/// </summary>
public class SpawnPointManager : MonoBehaviour
{
    private static SpawnPointManager _instance;
    
    // Cached list of all spawn point definitions
    private static List<SpawnPointDefinition> _cachedDefinitions;
    
    // Dictionary for faster lookups
    private static Dictionary<string, SpawnPointDefinition> _definitionsById;
    
    public static SpawnPointManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Only create a new instance if we're in play mode
                if (Application.isPlaying)
                {
                    // Create a new GameObject with the manager if it doesn't exist
                    var go = new GameObject("SpawnPointManager");
                    _instance = go.AddComponent<SpawnPointManager>();
                    DontDestroyOnLoad(go);
                }
                else
                {
                    // In editor mode, just find an existing instance or return null
                    _instance = FindObjectOfType<SpawnPointManager>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Only use DontDestroyOnLoad in play mode
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
            
            // Initialize the cache
            InitializeCache();
        }
    }
    
    // Initialize the cache by loading all spawn point definitions from Resources
    private void InitializeCache()
    {
        _cachedDefinitions = new List<SpawnPointDefinition>();
        _definitionsById = new Dictionary<string, SpawnPointDefinition>();
        
        // Load all SpawnPointDefinition assets from Resources/SpawnPoints folder
        SpawnPointDefinition[] definitions = Resources.LoadAll<SpawnPointDefinition>("SpawnPoints");
        
        foreach (var def in definitions)
        {
            _cachedDefinitions.Add(def);
            
            // Add to dictionary for faster lookups
            if (!string.IsNullOrEmpty(def.id) && !_definitionsById.ContainsKey(def.id))
            {
                _definitionsById.Add(def.id, def);
            }
            else if (!string.IsNullOrEmpty(def.id))
            {
                Debug.LogWarning($"Duplicate spawn point ID found: {def.id} in {def.name}. IDs must be unique.");
            }
        }
        
        Debug.Log($"SpawnPointManager: Loaded {_cachedDefinitions.Count} spawn point definitions");
    }
    
    // Force reload all definitions (useful for editor scripts)
    public void ReloadDefinitions()
    {
        InitializeCache();
    }
    
    // Get all spawn point definitions
    public List<SpawnPointDefinition> GetAllDefinitions()
    {
        if (_cachedDefinitions == null)
        {
            InitializeCache();
        }
        
        return _cachedDefinitions;
    }
    
    // Get definitions for a specific scene
    public List<SpawnPointDefinition> GetDefinitionsForScene(GameScene.Type sceneType)
    {
        if (_cachedDefinitions == null)
        {
            InitializeCache();
        }
        
        return _cachedDefinitions
            .Where(d => d.sourceScene == sceneType)
            .ToList();
    }
    
    // Get a specific definition by ID
    public SpawnPointDefinition GetDefinitionById(string id)
    {
        if (_definitionsById == null)
        {
            InitializeCache();
        }
        
        if (string.IsNullOrEmpty(id) || !_definitionsById.ContainsKey(id))
        {
            return null;
        }
        
        return _definitionsById[id];
    }
    
    // Get connected definition
    public SpawnPointDefinition GetConnectedDefinition(SpawnPointDefinition source)
    {
        if (source == null || string.IsNullOrEmpty(source.connectedSpawnPointId))
        {
            return null;
        }
        
        return GetDefinitionById(source.connectedSpawnPointId);
    }
    
#if UNITY_EDITOR
    // Editor-only method to create a new definition asset
    public static SpawnPointDefinition CreateDefinitionAsset(string id, string displayName, GameScene.Type sourceScene)
    {
        var definition = ScriptableObject.CreateInstance<SpawnPointDefinition>();
        definition.id = id;
        definition.displayName = displayName;
        definition.sourceScene = sourceScene;
        
        string path = $"Assets/Resources/SpawnPoints/{id}.asset";
        UnityEditor.AssetDatabase.CreateAsset(definition, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        return definition;
    }
#endif
}