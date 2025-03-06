using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor window that provides a visual interface for refactoring tasks
/// </summary>
public class RefactoringToolsWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private bool showPlayerSystemsFoldout = true;
    private bool showAttackSystemsFoldout = true;
    private bool showUISystemsFoldout = true;
    
    private string statusMessage = "";
    private bool isSuccess = true;
    
    [MenuItem("Tools/Refactoring/Refactoring Tools Window", false, 0)]
    public static void ShowWindow()
    {
        RefactoringToolsWindow window = GetWindow<RefactoringToolsWindow>();
        window.titleContent = new GUIContent("Refactoring Tools");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }
    
    private void OnGUI()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("Gangsta Bean 3 Refactoring Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Use these tools to automate the refactoring process.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            
            // Status message
            if (!string.IsNullOrEmpty(statusMessage))
            {
                EditorGUILayout.HelpBox(statusMessage, isSuccess ? MessageType.Info : MessageType.Error);
                EditorGUILayout.Space();
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                // Player Systems Section
                showPlayerSystemsFoldout = EditorGUILayout.Foldout(showPlayerSystemsFoldout, "Player Systems", true, EditorStyles.foldoutHeader);
                if (showPlayerSystemsFoldout)
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUILayout.LabelField("Add the new player components to player prefabs.", EditorStyles.wordWrappedLabel);
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUI.indentLevel * 15);
                    if (GUILayout.Button("Setup Player Systems", GUILayout.Height(30)))
                    {
                        RefactoringToolsMenu.SetupPlayerSystems();
                        statusMessage = "Player systems have been set up!";
                        isSuccess = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space();
                    
                    EditorGUI.indentLevel--;
                }
                
                // Attack Systems Section
                showAttackSystemsFoldout = EditorGUILayout.Foldout(showAttackSystemsFoldout, "Attack Systems", true, EditorStyles.foldoutHeader);
                if (showAttackSystemsFoldout)
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUILayout.LabelField("Add input adapters to attack components.", EditorStyles.wordWrappedLabel);
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUI.indentLevel * 15);
                    if (GUILayout.Button("Setup Attack Input Adapters", GUILayout.Height(30)))
                    {
                        RefactoringToolsMenu.SetupAttackInputAdapters();
                        statusMessage = "Attack input adapters have been set up!";
                        isSuccess = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space();
                    
                    EditorGUI.indentLevel--;
                }
                
                // UI Systems Section
                showUISystemsFoldout = EditorGUILayout.Foldout(showUISystemsFoldout, "UI Systems", true, EditorStyles.foldoutHeader);
                if (showUISystemsFoldout)
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUILayout.LabelField("Add UI adapters to HUD prefabs.", EditorStyles.wordWrappedLabel);
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUI.indentLevel * 15);
                    if (GUILayout.Button("Setup PlayerStats UI Adapters", GUILayout.Height(30)))
                    {
                        RefactoringToolsMenu.SetupPlayerStatsUIAdapter();
                        statusMessage = "PlayerStats UI adapters have been set up!";
                        isSuccess = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space();
                    
                    EditorGUI.indentLevel--;
                }
                
                // Apply changes to scenes
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Apply Changes to Scenes", EditorStyles.boldLabel);
                
                EditorGUILayout.LabelField("After setting up prefabs, apply changes to all scenes.", EditorStyles.wordWrappedLabel);
                
                if (GUILayout.Button("Apply Prefab Changes to Scenes", GUILayout.Height(30)))
                {
                    RefactoringToolsMenu.ApplyPrefabChangesToScenes();
                    statusMessage = "Please open and save each scene to apply the changes.";
                    isSuccess = true;
                }
                
                EditorGUILayout.Space(20);
                
                // Status updates
                EditorGUILayout.LabelField("Refactoring Status", EditorStyles.boldLabel);
                DrawStatusSection();
            }
            EditorGUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }
    
    private void DrawStatusSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Check if the required components are available
        bool hasPlayerContext = CheckTypeExists("PlayerContext");
        bool hasPlayerStats = CheckTypeExists("PlayerStatsHandlerRefactored");
        bool hasInputProvider = CheckTypeExists("IInputProvider");
        bool hasGunAttackAdapter = CheckTypeExists("GunAttackInputAdapter");
        bool hasStatsSaveSystem = CheckTypeExists("PlayerStatsSaveSystem");
        
        DrawStatusItem("PlayerContext Implementation", hasPlayerContext);
        DrawStatusItem("PlayerStatsHandlerRefactored Implementation", hasPlayerStats);
        DrawStatusItem("IInputProvider Interface", hasInputProvider);
        DrawStatusItem("GunAttackInputAdapter Implementation", hasGunAttackAdapter);
        DrawStatusItem("PlayerStatsSaveSystem Implementation", hasStatsSaveSystem);
        
        // Check if the player prefabs have been set up
        bool gangstaBeanSetup = IsPrefabSetUp("Assets/+PREFABS/+CHARS/+GB - CHARACTER V2.prefab");
        bool brockLeeSetup = IsPrefabSetUp("Assets/+PREFABS/+CHARS/+LEE - CHARACTER -V2.prefab");
        
        DrawStatusItem("GangstaBean Prefab Setup", gangstaBeanSetup);
        DrawStatusItem("BrockLee Prefab Setup", brockLeeSetup);
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawStatusItem(string label, bool isComplete)
    {
        EditorGUILayout.BeginHorizontal();
        
        // Status icon
        GUIStyle iconStyle = new GUIStyle(EditorStyles.label);
        iconStyle.normal.textColor = isComplete ? Color.green : Color.gray;
        EditorGUILayout.LabelField(isComplete ? "✓" : "○", iconStyle, GUILayout.Width(20));
        
        // Label
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = isComplete ? Color.black : new Color(0.5f, 0.5f, 0.5f);
        EditorGUILayout.LabelField(label, labelStyle);
        
        EditorGUILayout.EndHorizontal();
    }
    
    private bool CheckTypeExists(string typeName)
    {
        // Check if a type exists in the loaded assemblies
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetType(typeName) != null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private bool IsPrefabSetUp(string prefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return false;
        
        // Check if the prefab has the necessary components
        return prefab.GetComponent<PlayerContext>() != null &&
               prefab.GetComponent<PlayerStatsHandlerRefactored>() != null &&
               prefab.GetComponent<PlayerLegacyAdapter>() != null &&
               prefab.GetComponent<UnityInputProviderAdapter>() != null &&
               prefab.GetComponent<PlayerStatsSaveSystem>() != null;
    }
}