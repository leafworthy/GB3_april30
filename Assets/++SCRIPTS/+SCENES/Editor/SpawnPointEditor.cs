using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(SpawnPoint))]
public class SpawnPointEditor : Editor
{
    // Cache the loaded definitions
    private List<SpawnPointDefinition> allDefinitions;
    private List<SpawnPointDefinition> sceneDefinitions;
    private string[] definitionNames;
    private int selectedDefinitionIndex = -1;
    
    // The target SpawnPoint component
    private SpawnPoint spawnPoint;
    
    // SerializedProperties
    private SerializedProperty idProperty;
    private SerializedProperty currentSceneProperty;
    private SerializedProperty pointTypeProperty;
    private SerializedProperty destinationSceneProperty;
    private SerializedProperty connectedSpawnPointIdProperty;
    private SerializedProperty capacityProperty;
    private SerializedProperty gizmoColorProperty;
    private SerializedProperty gizmoSizeProperty;
    
    private bool showAdvancedOptions = false;
    private bool showConnectionOptions = false;
    private bool definitionsLoaded = false;
    
    private void OnEnable()
    {
        spawnPoint = (SpawnPoint)target;
        
        // Get serialized properties
        idProperty = serializedObject.FindProperty("id");
        currentSceneProperty = serializedObject.FindProperty("currentScene");
        pointTypeProperty = serializedObject.FindProperty("pointType");
        destinationSceneProperty = serializedObject.FindProperty("destinationScene");
        connectedSpawnPointIdProperty = serializedObject.FindProperty("connectedSpawnPointId");
        capacityProperty = serializedObject.FindProperty("capacity");
        gizmoColorProperty = serializedObject.FindProperty("gizmoColor");
        gizmoSizeProperty = serializedObject.FindProperty("gizmoSize");
        
        // Load definitions
        LoadDefinitions();
    }
    
    private void LoadDefinitions()
    {
        // Load all definitions from Resources
        allDefinitions = Resources.LoadAll<SpawnPointDefinition>("SpawnPoints").ToList();
        
        // Create array of names for the popup
        definitionNames = new string[allDefinitions.Count + 1];
        definitionNames[0] = "None (Custom)";
        
        for (int i = 0; i < allDefinitions.Count; i++)
        {
            var def = allDefinitions[i];
            string name = !string.IsNullOrEmpty(def.displayName) 
                ? $"{def.displayName} ({def.id})" 
                : def.id;
                
            definitionNames[i + 1] = name;
            
            // Check if this definition matches the current SpawnPoint
            if (def.id == spawnPoint.id)
            {
                selectedDefinitionIndex = i + 1;
            }
        }
        
        // If no matching definition was found, select "None"
        if (selectedDefinitionIndex == -1)
        {
            selectedDefinitionIndex = 0;
        }
        
        definitionsLoaded = true;
    }
    
  
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // Display intro
        EditorGUILayout.HelpBox(
            "Spawn Points define entry and exit locations for level transitions. " +
            "Link them with LevelLoadInteraction components to create navigation between scenes.", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        // Display definition selector
        if (!definitionsLoaded)
        {
            LoadDefinitions();
        }
        
        EditorGUILayout.LabelField("Spawn Point Definition", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        
        int newSelectedIndex = EditorGUILayout.Popup("Load Definition", selectedDefinitionIndex, definitionNames);
        
        if (GUILayout.Button("Refresh", GUILayout.Width(60)))
        {
            LoadDefinitions();
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Handle definition selection change
        if (newSelectedIndex != selectedDefinitionIndex)
        {
            selectedDefinitionIndex = newSelectedIndex;
            
            if (selectedDefinitionIndex > 0)
            {
                // Apply the selected definition properties to the spawn point
                var selectedDefinition = allDefinitions[selectedDefinitionIndex - 1];
                
                idProperty.stringValue = selectedDefinition.id;
           
                
                // Find the index of the point type enum value
                for (int i = 0; i < System.Enum.GetValues(typeof(SpawnPointType)).Length; i++)
                {
                    if ((SpawnPointType)i == selectedDefinition.pointType)
                    {
                        pointTypeProperty.enumValueIndex = i;
                        break;
                    }
                }
                
                connectedSpawnPointIdProperty.stringValue = selectedDefinition.connectedSpawnPointId;
                capacityProperty.intValue = selectedDefinition.capacity;
                gizmoColorProperty.colorValue = selectedDefinition.gizmoColor;
                
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        EditorGUILayout.Space();
        
        // Basic settings
        EditorGUILayout.LabelField("Basic Settings", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(idProperty);
        EditorGUILayout.PropertyField(currentSceneProperty);
        EditorGUILayout.PropertyField(pointTypeProperty);
        
        // Connection settings
        SpawnPointType pointType = (SpawnPointType)pointTypeProperty.enumValueIndex;
        
        if (pointType == SpawnPointType.Exit || pointType == SpawnPointType.Both)
        {
            EditorGUILayout.PropertyField(destinationSceneProperty);
            
            EditorGUI.BeginChangeCheck();
            
            // Get available destination scene spawn points
          
             
        }
        
        // Advanced settings
        EditorGUILayout.Space();
        showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced Settings", true);
        
        if (showAdvancedOptions)
        {
            EditorGUILayout.PropertyField(capacityProperty);
            EditorGUILayout.PropertyField(gizmoColorProperty);
            EditorGUILayout.PropertyField(gizmoSizeProperty);
        }
        
        // Add button to create a new definition from this spawn point
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Save as Definition"))
        {
            // Create a definition asset from this spawn point
            SaveAsDefinition();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void SaveAsDefinition()
    {
        // Check if the ID is valid
        if (string.IsNullOrEmpty(spawnPoint.id))
        {
            EditorUtility.DisplayDialog("Error", "The spawn point must have a valid ID to be saved as a definition.", "OK");
            return;
        }
        
        // Check if a definition with this ID already exists
        if (allDefinitions.Any(d => d.id == spawnPoint.id))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Overwrite Definition", 
                $"A definition with ID '{spawnPoint.id}' already exists. Do you want to overwrite it?", 
                "Overwrite", "Cancel");
                
            if (!overwrite)
            {
                return;
            }
        }
        
        // Create a new definition asset
        CreateOrUpdateDefinition();
    }
    
    private void CreateOrUpdateDefinition()
    {
        // Check if the Resources/SpawnPoints folder exists
        string folderPath = "Assets/Resources/SpawnPoints";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            // Create the folder if it doesn't exist
            string resourcesPath = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.CreateFolder(resourcesPath, "SpawnPoints");
        }
        
        // Check if a definition with this ID already exists
        string assetPath = $"{folderPath}/{spawnPoint.id}.asset";
        SpawnPointDefinition definition = AssetDatabase.LoadAssetAtPath<SpawnPointDefinition>(assetPath);
        
        if (definition == null)
        {
            // Create a new definition
            definition = ScriptableObject.CreateInstance<SpawnPointDefinition>();
            AssetDatabase.CreateAsset(definition, assetPath);
        }
        
        // Update the definition properties
        definition.id = spawnPoint.id;
        definition.displayName = spawnPoint.id; // Use ID as display name initially
        definition.connectedSpawnPointId = spawnPoint.connectedSpawnPointId;
        definition.pointType = spawnPoint.pointType;
        definition.capacity = spawnPoint.capacity;
        definition.gizmoColor = spawnPoint.gizmoColor;
        
        // Save the asset
        EditorUtility.SetDirty(definition);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Reload definitions
        LoadDefinitions();
        
        EditorUtility.DisplayDialog("Definition Saved", 
            $"Spawn point definition '{spawnPoint.id}' has been saved to Resources/SpawnPoints.", "OK");
    }
    
    [MenuItem("GameObject/Gangsta Bean/Spawn Point", false, 10)]
    public static void CreateSpawnPoint()
    {
        // Create a new GameObject with a SpawnPoint component
        GameObject newObject = new GameObject("SpawnPoint");
        SpawnPoint spawnPoint = newObject.AddComponent<SpawnPoint>();
        
        // Set default values
        spawnPoint.id = "spawn_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        
        // Position the object at the scene view camera position
        SceneView view = SceneView.lastActiveSceneView;
        if (view != null)
        {
            newObject.transform.position = view.camera.transform.position + view.camera.transform.forward * 2;
        }
        
        // Select the new object
        Selection.activeGameObject = newObject;
        
        // Ensure the object is registered for undo
        Undo.RegisterCreatedObjectUndo(newObject, "Create Spawn Point");
    }
}