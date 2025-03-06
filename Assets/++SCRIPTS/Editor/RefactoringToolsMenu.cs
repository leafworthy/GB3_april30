using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Editor tool to automate the refactoring process by automatically adding
/// required components to player prefabs and setting up the necessary references
/// </summary>
public class RefactoringToolsMenu : MonoBehaviour
{
    // Define paths to important prefabs
    private const string GANGSTA_BEAN_PREFAB_PATH = "Assets/+PREFABS/+CHARS/+GB - CHARACTER V2.prefab";
    private const string BROCK_LEE_PREFAB_PATH = "Assets/+PREFABS/+CHARS/+LEE - CHARACTER -V2.prefab";
    private const string PLAYER_DATA_PATH = "Assets/Resources/PlayerData"; // Adjust if your player data is in a different location

    [MenuItem("Tools/Refactoring/Setup Player Systems", false, 100)]
    public static void SetupPlayerSystems()
    {
        bool success = true;
        List<string> messages = new List<string>();
        
        // Process GangstaBean prefab
        messages.Add("Processing GangstaBean prefab...");
        success &= ProcessPlayerPrefab(GANGSTA_BEAN_PREFAB_PATH, "GangstaBean", ref messages);
        
        // Process BrockLee prefab
        messages.Add("Processing BrockLee prefab...");
        success &= ProcessPlayerPrefab(BROCK_LEE_PREFAB_PATH, "BrockLee", ref messages);
        
        // Show results
        if (success)
        {
            messages.Add("Player systems have been successfully set up!");
            ShowResultDialog("Success", string.Join("\n", messages), true);
        }
        else
        {
            messages.Add("There were issues setting up the player systems. Check the console for details.");
            ShowResultDialog("Issues Found", string.Join("\n", messages), false);
        }
    }

    [MenuItem("Tools/Refactoring/Setup Attack Input Adapters", false, 101)]
    public static void SetupAttackInputAdapters()
    {
        bool success = true;
        List<string> messages = new List<string>();
        
        // Find all GameObjects with GunAttackRefactored
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int adaptersAdded = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                bool modified = false;
                
                // Check for GunAttackRefactored components
                var gunAttackComponents = prefab.GetComponentsInChildren<GunAttackRefactored>(true);
                foreach (var gunAttack in gunAttackComponents)
                {
                    // Add GunAttackInputAdapter if needed
                    if (gunAttack.gameObject.GetComponent<GunAttackInputAdapter>() == null)
                    {
                        gunAttack.gameObject.AddComponent<GunAttackInputAdapter>();
                        adaptersAdded++;
                        modified = true;
                    }
                }
                
                if (modified)
                {
                    EditorUtility.SetDirty(prefab);
                    messages.Add($"Added GunAttackInputAdapter to {prefab.name}");
                }
            }
        }
        
        if (adaptersAdded > 0)
        {
            AssetDatabase.SaveAssets();
            messages.Add($"Added {adaptersAdded} GunAttackInputAdapter components.");
            ShowResultDialog("Success", string.Join("\n", messages), true);
        }
        else
        {
            messages.Add("No GunAttackRefactored components found that needed adapters.");
            ShowResultDialog("No Changes", string.Join("\n", messages), true);
        }
    }

    [MenuItem("Tools/Refactoring/Setup PlayerStatsUIAdapter on HUD Prefabs", false, 103)]
    public static void SetupPlayerStatsUIAdapter()
    {
        bool success = true;
        List<string> messages = new List<string>();
        
        // Find all HUD prefabs
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/+PREFABS/+UI" });
        int adaptersAdded = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            // Only process prefabs that are likely to be HUD prefabs
            if (path.Contains("HUD") || path.Contains("hud"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    bool modified = false;
                    
                    // Check if the prefab already has the adapter
                    var existingAdapter = prefab.GetComponent<PlayerStatsUIAdapter>();
                    if (existingAdapter == null)
                    {
                        // Add the adapter
                        prefab.AddComponent<PlayerStatsUIAdapter>();
                        adaptersAdded++;
                        modified = true;
                        messages.Add($"Added PlayerStatsUIAdapter to {prefab.name}");
                    }
                    else
                    {
                        messages.Add($"PlayerStatsUIAdapter already exists on {prefab.name}");
                    }
                    
                    if (modified)
                    {
                        EditorUtility.SetDirty(prefab);
                    }
                }
            }
        }
        
        if (adaptersAdded > 0)
        {
            AssetDatabase.SaveAssets();
            messages.Add($"Added {adaptersAdded} PlayerStatsUIAdapter components to HUD prefabs.");
            ShowResultDialog("Success", string.Join("\n", messages), true);
        }
        else
        {
            messages.Add("No HUD prefabs found that needed PlayerStatsUIAdapter.");
            ShowResultDialog("No Changes", string.Join("\n", messages), true);
        }
    }

    [MenuItem("Tools/Refactoring/Apply Prefab Changes to Scenes", false, 102)]
    public static void ApplyPrefabChangesToScenes()
    {
        bool success = true;
        List<string> messages = new List<string>();
        
        // Find all scene files
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/+SCENES" });
        messages.Add($"Found {sceneGuids.Length} scenes to process.");
        
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            messages.Add($"Scene updates will be applied when you open: {scenePath}");
        }
        
        ShowResultDialog("Scene Update Required", 
            "To complete the process, you need to open each scene and save it to apply the prefab updates.\n\n" +
            string.Join("\n", messages), true);
    }

    private static bool ProcessPlayerPrefab(string prefabPath, string playerName, ref List<string> messages)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            messages.Add($"Error: Could not find prefab at {prefabPath}");
            return false;
        }

        bool modified = false;
        
        // 1. Find or add PlayerContext
        PlayerContext playerContext = prefab.GetComponent<PlayerContext>();
        if (playerContext == null)
        {
            playerContext = prefab.AddComponent<PlayerContext>();
            messages.Add($"Added PlayerContext to {playerName}");
            modified = true;
        }
        
        // 2. Find or add PlayerStatsHandlerRefactored
        PlayerStatsHandlerRefactored statsHandler = prefab.GetComponent<PlayerStatsHandlerRefactored>();
        if (statsHandler == null)
        {
            statsHandler = prefab.AddComponent<PlayerStatsHandlerRefactored>();
            messages.Add($"Added PlayerStatsHandlerRefactored to {playerName}");
            modified = true;
            
            // Try to find appropriate PlayerData
            PlayerData playerData = FindPlayerData(playerName);
            if (playerData != null)
            {
                SerializedObject serializedStats = new SerializedObject(statsHandler);
                SerializedProperty playerDataProp = serializedStats.FindProperty("playerData");
                playerDataProp.objectReferenceValue = playerData;
                serializedStats.ApplyModifiedProperties();
                messages.Add($"Assigned {playerData.name} to PlayerStatsHandlerRefactored");
            }
            else
            {
                messages.Add($"Warning: Could not find PlayerData for {playerName}");
            }
        }
        
        // 3. Find or add PlayerLegacyAdapter
        var legacyAdapter = prefab.GetComponent<PlayerLegacyAdapter>();
        if (legacyAdapter == null)
        {
            prefab.AddComponent<PlayerLegacyAdapter>();
            messages.Add($"Added PlayerLegacyAdapter to {playerName}");
            modified = true;
        }
        
        // 4. Find or add UnityInputProviderAdapter
        var inputAdapter = prefab.GetComponent<UnityInputProviderAdapter>();
        if (inputAdapter == null)
        {
            prefab.AddComponent<UnityInputProviderAdapter>();
            messages.Add($"Added UnityInputProviderAdapter to {playerName}");
            modified = true;
        }
        
        // 5. Find or add PlayerStatsSaveSystem
        var statsSaveSystem = prefab.GetComponent<PlayerStatsSaveSystem>();
        if (statsSaveSystem == null)
        {
            statsSaveSystem = prefab.AddComponent<PlayerStatsSaveSystem>();
            messages.Add($"Added PlayerStatsSaveSystem to {playerName}");
            modified = true;
            
            // Configure PlayerStatsSaveSystem defaults if needed
            SerializedObject serializedSaveSystem = new SerializedObject(statsSaveSystem);
            SerializedProperty saveFileNameProp = serializedSaveSystem.FindProperty("saveFileName");
            saveFileNameProp.stringValue = playerName.ToLower() + "_stats";
            SerializedProperty autoSaveOnExitProp = serializedSaveSystem.FindProperty("autoSaveOnExit");
            autoSaveOnExitProp.boolValue = true;
            serializedSaveSystem.ApplyModifiedProperties();
            messages.Add($"Configured PlayerStatsSaveSystem defaults for {playerName}");
        }
        
        // 6. Add GunAttackInputAdapter to any GunAttackRefactored components
        var gunAttackComponents = prefab.GetComponentsInChildren<GunAttackRefactored>(true);
        foreach (var gunAttack in gunAttackComponents)
        {
            if (gunAttack.gameObject.GetComponent<GunAttackInputAdapter>() == null)
            {
                gunAttack.gameObject.AddComponent<GunAttackInputAdapter>();
                messages.Add($"Added GunAttackInputAdapter to {gunAttack.gameObject.name}");
                modified = true;
            }
        }
        
        // Save changes if needed
        if (modified)
        {
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            messages.Add($"Saved changes to {playerName} prefab");
        }
        else
        {
            messages.Add($"No changes needed for {playerName} prefab");
        }
        
        return true;
    }

    private static PlayerData FindPlayerData(string playerName)
    {
        // Find all PlayerData scriptable objects
        string[] guids = AssetDatabase.FindAssets("t:PlayerData", new[] { PLAYER_DATA_PATH });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PlayerData data = AssetDatabase.LoadAssetAtPath<PlayerData>(path);
            
            if (data != null && path.Contains(playerName))
            {
                return data;
            }
        }
        
        // If no specific match, return the first one as a fallback
        if (guids.Length > 0)
        {
            string firstPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<PlayerData>(firstPath);
        }
        
        return null;
    }

    private static void ShowResultDialog(string title, string message, bool success)
    {
        if (success)
        {
            EditorUtility.DisplayDialog(title, message, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog(title, message, "OK");
            Debug.LogError(message);
        }
    }
}