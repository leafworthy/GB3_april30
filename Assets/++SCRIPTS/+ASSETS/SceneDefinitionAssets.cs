using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Asset container for easily accessing scene definitions.
/// </summary>
[Serializable, CreateAssetMenu(fileName = "SceneDefinitions", menuName = "Gangsta Bean/Assets/Scene Definitions", order = 1)]
public class SceneDefinitionAssets : ScriptableObject
{
    // Common scenes with direct references
    [Header("System Scenes")]
    public SceneDefinition gameManager;

    [Header("Menu Scenes")]
    public SceneDefinition mainMenu;
    public SceneDefinition characterSelect;
    public SceneDefinition restartLevel;
    public SceneDefinition endScreen;
    
    [Header("Gameplay Scenes")]
    public SceneDefinition gangstaBeanHouse;
    public SceneDefinition fancyHouse;
    public SceneDefinition baseballField;
    public SceneDefinition gasStation;
    
    [Header("Additional Scenes")]
    [Tooltip("Additional scenes not covered by the properties above")]
    public List<SceneDefinition> additionalScenes = new List<SceneDefinition>();

    // Quick lookups
    private Dictionary<string, SceneDefinition> _nameMap;

    /// <summary>
    /// Initialize lookup dictionaries with all scene references
    /// </summary>
    public void Initialize()
    {
        _nameMap = new Dictionary<string, SceneDefinition>(StringComparer.OrdinalIgnoreCase);

        // Add all directly referenced scenes to name dictionary
        AddToNameDictionary(mainMenu);
        AddToNameDictionary(characterSelect);
        AddToNameDictionary(gangstaBeanHouse);
        AddToNameDictionary(endScreen);
        AddToNameDictionary(fancyHouse);
        AddToNameDictionary(baseballField);
        AddToNameDictionary(gasStation);
        AddToNameDictionary(restartLevel);
        AddToNameDictionary(gameManager);
        
        // Add all additional scenes
        if (additionalScenes != null)
        {
            foreach (var scene in additionalScenes)
            {
                AddToNameDictionary(scene);
            }
        }
    }

    // Add a scene to the name dictionary
    private void AddToNameDictionary(SceneDefinition scene)
    {
        if (scene == null || !scene.IsValid())
            return;

        // Add to name map, using scene name as key
        if (!string.IsNullOrEmpty(scene.SceneName) && !_nameMap.ContainsKey(scene.SceneName))
        {
            _nameMap[scene.SceneName] = scene;
        }
    }

    /// <summary>
    /// Get a scene definition by its name
    /// </summary>
    public SceneDefinition GetByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return null;

        if (_nameMap == null)
            Initialize();

        return _nameMap.TryGetValue(sceneName, out var scene) ? scene : null;
    }

    /// <summary>
    /// Get all scenes as a flat list
    /// </summary>
    public List<SceneDefinition> GetAllScenes()
    {
        var result = new List<SceneDefinition>();

        // Add all directly referenced scenes
        if (mainMenu != null) result.Add(mainMenu);
        if (characterSelect != null) result.Add(characterSelect);
        if (gangstaBeanHouse != null) result.Add(gangstaBeanHouse);
        if (endScreen != null) result.Add(endScreen);
        if (fancyHouse != null) result.Add(fancyHouse);
        if (baseballField != null) result.Add(baseballField);
        if (gasStation != null) result.Add(gasStation);
        if (restartLevel != null) result.Add(restartLevel);
        if (gameManager != null) result.Add(gameManager);
        
        // Add additional scenes
        if (additionalScenes != null)
        {
            foreach (var scene in additionalScenes)
            {
                if (scene != null && scene.IsValid())
                {
                    result.Add(scene);
                }
            }
        }

        return result;
    }
    
    /// <summary>
    /// Get all gameplay level scenes
    /// </summary>
    public List<SceneDefinition> GetAllGameplayScenes()
    {
        return GetAllScenes().FindAll(scene => scene.IsGameplayLevel);
    }

    /// <summary>
    /// Find a scene definition that matches the given name
    /// </summary>
    public SceneDefinition FindScene(string sceneName)
    {
        // Simply use the dictionary lookup
        return GetByName(sceneName);
    }
    
    /// <summary>
    /// Find the best starting level from available scenes
    /// </summary>
    public SceneDefinition FindStartingLevel()
    {
        // First look for scenes with "First" or "Start" in the name
        foreach (var scene in GetAllGameplayScenes())
        {
            if (scene.name.Contains("First") || scene.name.Contains("Start"))
                return scene;
        }
        
        // Then use explicitly referenced scenes in priority order
        if (gangstaBeanHouse != null) return gangstaBeanHouse;
        if (baseballField != null) return baseballField;
        if (fancyHouse != null) return fancyHouse;
        if (gasStation != null) return gasStation;
        
        // Last resort: return the first gameplay scene found
        var gameplayScenes = GetAllGameplayScenes();
        if (gameplayScenes.Count > 0)
            return gameplayScenes[0];
            
        // If no gameplay scenes found, return null
        return null;
    }
    
    /// <summary>
    /// Add a scene definition to this container (useful for runtime-created definitions)
    /// </summary>
    public void AddScene(SceneDefinition scene)
    {
        if (scene == null || !scene.IsValid())
            return;
            
        // Make sure name dictionary is initialized
        if (_nameMap == null)
            Initialize();
            
        // Add to name dictionary
        AddToNameDictionary(scene);
        
        // Add to additional scenes list if not present in any other property
        if (scene != mainMenu && 
            scene != characterSelect &&
            scene != gangstaBeanHouse &&
            scene != endScreen &&
            scene != fancyHouse &&
            scene != baseballField &&
            scene != gasStation &&
            scene != restartLevel &&
            scene != gameManager &&
            !additionalScenes.Contains(scene))
        {
            additionalScenes.Add(scene);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Automatically initialize when modified in editor
        Initialize();
    }
#endif
}