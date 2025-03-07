using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages scene definitions loaded from Resources
/// </summary>
public class SceneDefinitionManager : MonoBehaviour
{
    private static SceneDefinitionManager _instance;
    
    // Cached list of all scene definitions
    private static List<SceneDefinition> _cachedDefinitions;
    
    // Dictionary for faster lookups
    private static Dictionary<GameScene.Type, SceneDefinition> _definitionsByType;
    
    public static SceneDefinitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Only create a new instance if we're in play mode
                if (Application.isPlaying)
                {
                    Debug.Log("this shouldn't run");
                    // Create a new GameObject with the manager if it doesn't exist
                    var go = new GameObject("SceneDefinitionManager");
                    var sceneloader = FindFirstObjectByType<SceneLoader>();
                    if(sceneloader != null)
                    {
                        go.transform.SetParent(sceneloader.transform);
                        _instance = go.AddComponent<SceneDefinitionManager>();
                    }
                    

                }
                else
                {
                    // In editor mode, just find an existing instance or return null
                    _instance = FindObjectOfType<SceneDefinitionManager>();
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
        DontDestroyOnLoad(gameObject);
        
        // Initialize the cache
        InitializeCache();
    }
    
    // Initialize the cache by loading all scene definitions from Resources
    private void InitializeCache()
    {
        _cachedDefinitions = new List<SceneDefinition>();
        _definitionsByType = new Dictionary<GameScene.Type, SceneDefinition>();
        
        // Load all SceneDefinition assets from Resources/Scenes folder
        SceneDefinition[] definitions = Resources.LoadAll<SceneDefinition>("Scenes");
        
        // Make sure we have at least one definition for each scene type
        EnsureAllSceneTypesHaveDefinitions(definitions);
        
        foreach (var def in definitions)
        {
            _cachedDefinitions.Add(def);
            
            // Add to dictionary for faster lookups
            if (!_definitionsByType.ContainsKey(def.sceneType))
            {
                _definitionsByType.Add(def.sceneType, def);
            }
            else
            {
                Debug.LogWarning($"Duplicate scene type found: {def.sceneType} in {def.name}. Scene types must be unique.");
            }
        }
        
        Debug.Log($"SceneDefinitionManager: Loaded {_cachedDefinitions.Count} scene definitions");
    }
    
    // Make sure we have at least placeholder definitions for all scene types
    private void EnsureAllSceneTypesHaveDefinitions(SceneDefinition[] loadedDefinitions)
    {
        // Get all scene types
        var sceneTypes = System.Enum.GetValues(typeof(GameScene.Type));
        var missingTypes = new List<GameScene.Type>();
        
        // Check which types are missing
        foreach (GameScene.Type sceneType in sceneTypes)
        {
            if (sceneType == GameScene.Type.None)
                continue;
                
            bool hasDefinition = false;
            foreach (var def in loadedDefinitions)
            {
                if (def.sceneType == sceneType)
                {
                    hasDefinition = true;
                    break;
                }
            }
            
            if (!hasDefinition)
            {
                missingTypes.Add(sceneType);
            }
        }
        
        // Create runtime-only definitions for missing types
        foreach (var missingType in missingTypes)
        {
            var def = ScriptableObject.CreateInstance<SceneDefinition>();
            def.sceneType = missingType;
            def.displayName = FormatSceneTypeName(missingType.ToString());
            def.description = $"This is the {def.displayName} scene.";
            
            _cachedDefinitions.Add(def);
            _definitionsByType[missingType] = def;
            
            Debug.Log($"Created runtime definition for scene type: {missingType}");
        }
    }
    
    // Convert CamelCase to spaced words (e.g., "MainMenu" to "Main Menu")
    private string FormatSceneTypeName(string typeName)
    {
        string result = "";
        for (int i = 0; i < typeName.Length; i++)
        {
            if (i > 0 && char.IsUpper(typeName[i]))
            {
                result += " ";
            }
            result += typeName[i];
        }
        return result;
    }
    
    // Force reload all definitions (useful for editor scripts)
    public void ReloadDefinitions()
    {
        InitializeCache();
    }
    
    // Get all scene definitions
    public List<SceneDefinition> GetAllDefinitions()
    {
        if (_cachedDefinitions == null)
        {
            InitializeCache();
        }
        
        return _cachedDefinitions;
    }
    
    // Get a specific definition by scene type
    public SceneDefinition GetDefinitionByType(GameScene.Type sceneType)
    {
        if (_definitionsByType == null)
        {
            InitializeCache();
        }
        
        if (!_definitionsByType.ContainsKey(sceneType))
        {
            // If we don't have a definition for this type, create a runtime one
            var def = ScriptableObject.CreateInstance<SceneDefinition>();
            def.sceneType = sceneType;
            def.displayName = FormatSceneTypeName(sceneType.ToString());
            def.description = $"This is the {def.displayName} scene.";
            
            _definitionsByType[sceneType] = def;
            _cachedDefinitions.Add(def);
            
            Debug.Log($"Created runtime definition for scene type: {sceneType}");
            
            return def;
        }
        
        return _definitionsByType[sceneType];
    }
    
    // Get display name for a scene type
    public string GetSceneDisplayName(GameScene.Type sceneType)
    {
        var definition = GetDefinitionByType(sceneType);
        return !string.IsNullOrEmpty(definition.displayName) 
            ? definition.displayName 
            : FormatSceneTypeName(sceneType.ToString());
    }
    
#if UNITY_EDITOR
    // Editor-only method to create a new definition asset
    public static SceneDefinition CreateDefinitionAsset(GameScene.Type sceneType, string displayName)
    {
        var definition = ScriptableObject.CreateInstance<SceneDefinition>();
        definition.sceneType = sceneType;
        definition.displayName = displayName;
        
        string path = $"Assets/Resources/Scenes/{sceneType}.asset";
        UnityEditor.AssetDatabase.CreateAsset(definition, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        return definition;
    }
#endif
}