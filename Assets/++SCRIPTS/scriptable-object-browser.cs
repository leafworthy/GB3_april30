using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class ASSETSBrowser : EditorWindow
{
    // Settings
    private string searchQuery = "";
    private Vector2 leftScrollPosition;
    private Vector2 mainScrollPosition;
    private Vector2 rightScrollPosition;
    private Dictionary<string, Color> assetTypeColors = new Dictionary<string, Color>();
    private Dictionary<string, bool> favoritePaths = new Dictionary<string, bool>();
    private List<ScriptableObject> favoriteObjects = new List<ScriptableObject>();
    
    // Left panel settings
    private float leftPanelWidth = 200f;
    private bool resizingLeftPanel = false;
    
    // Right panel settings
    private float rightPanelWidth = 300f;
    private bool resizingRightPanel = false;
    private bool showRightPanel = false;
    
    // Selected object
    private ScriptableObject selectedObject = null;
    
    // Asset Categories
    private Dictionary<string, List<ScriptableObject>> assetCategories = new Dictionary<string, List<ScriptableObject>>();
    private string selectedCategoryName = "";
    private string[] categoryNames;
    
    // Collapse/Expand state
    private Dictionary<string, bool> expandedStates = new Dictionary<string, bool>();
    private bool collapseAll = false;
    
    // Cached data
    private List<string> allAssetPaths = new List<string>();
    private GUIStyle headerStyle;
    private GUIStyle tabStyle;
    private GUIStyle selectedTabStyle;
    private GUIStyle boxStyle;
    private GUIStyle collapsedBoxStyle;
    private GUIStyle searchStyle;
    private GUIStyle favoriteStyle;
    private GUIStyle resizeHandleStyle;
    private Texture2D starIcon;
    private Texture2D starFilledIcon;
    private Texture2D expandIcon;
    private Texture2D collapseIcon;
    
    [MenuItem("Tools/ASSETS Browser")]
    public static void ShowWindow()
    {
        GetWindow<ASSETSBrowser>("ASSETS Browser");
    }

    private void OnEnable()
    {
        // Initialize styles
        InitStyles();
        
        // Load saved color preferences
        LoadColorPreferences();
        
        // Load favorites
        LoadFavorites();
        
        // Load expanded states
        LoadExpandedStates();
        
        // Load assets from the ASSETS singleton and Resources
        RefreshAssets();
    }
    
    private void InitStyles()
    {
        headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 14;
        headerStyle.margin = new RectOffset(5, 5, 5, 5);
        
        tabStyle = new GUIStyle(EditorStyles.label);
        tabStyle.padding = new RectOffset(10, 10, 5, 5);
        tabStyle.margin = new RectOffset(0, 0, 0, 0);
        tabStyle.alignment = TextAnchor.MiddleLeft;
        tabStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        
        selectedTabStyle = new GUIStyle(tabStyle);
        selectedTabStyle.normal.background = MakeColorTexture(new Color(0.6f, 0.6f, 0.8f, 0.5f));
        selectedTabStyle.normal.textColor = Color.white;
        
        boxStyle = new GUIStyle(EditorStyles.helpBox);
        boxStyle.padding = new RectOffset(10, 10, 10, 10);
        boxStyle.margin = new RectOffset(5, 5, 5, 5);
        
        collapsedBoxStyle = new GUIStyle(boxStyle);
        collapsedBoxStyle.padding = new RectOffset(10, 10, 5, 5);
        
        searchStyle = new GUIStyle(EditorStyles.toolbarSearchField);
        searchStyle.margin = new RectOffset(10, 10, 5, 5);
        
        favoriteStyle = new GUIStyle(EditorStyles.miniButton);
        favoriteStyle.padding = new RectOffset(2, 2, 2, 2);
        
        resizeHandleStyle = new GUIStyle();
        resizeHandleStyle.normal.background = EditorGUIUtility.whiteTexture;
        
        // Load icons
        starIcon = EditorGUIUtility.FindTexture("Favorite");
        starFilledIcon = EditorGUIUtility.FindTexture("Favorite Icon");
        expandIcon = EditorGUIUtility.FindTexture("d_winbtn_win_restore");
        collapseIcon = EditorGUIUtility.FindTexture("d_winbtn_win_min");
        if (expandIcon == null) expandIcon = EditorGUIUtility.FindTexture("animationvisibilitytoggleon");
        if (collapseIcon == null) collapseIcon = EditorGUIUtility.FindTexture("animationvisibilitytoggleoff");
    }
    
    private void LoadColorPreferences()
    {
        // Load custom saved colors for asset types
        string[] assetTypeNames = EditorPrefs.GetString("ASSETSBrowser_TypeNames", "").Split(',');
        foreach (string typeName in assetTypeNames)
        {
            if (string.IsNullOrEmpty(typeName)) continue;
            
            string colorStr = EditorPrefs.GetString("ASSETSBrowser_Color_" + typeName, "");
            if (!string.IsNullOrEmpty(colorStr))
            {
                string[] components = colorStr.Split(',');
                if (components.Length == 4)
                {
                    float r = float.Parse(components[0]);
                    float g = float.Parse(components[1]);
                    float b = float.Parse(components[2]);
                    float a = float.Parse(components[3]);
                    assetTypeColors[typeName] = new Color(r, g, b, a);
                }
            }
        }
        
        // Set default colors if none exist
        if (!assetTypeColors.ContainsKey("LevelAssets"))
            assetTypeColors["LevelAssets"] = new Color(0.2f, 0.6f, 0.3f); // Green
            
        if (!assetTypeColors.ContainsKey("CharacterPrefabAssets"))
            assetTypeColors["CharacterPrefabAssets"] = new Color(0.6f, 0.3f, 0.2f); // Orange/Brown
            
        if (!assetTypeColors.ContainsKey("UIAssets"))
            assetTypeColors["UIAssets"] = new Color(0.2f, 0.3f, 0.8f); // Blue
            
        if (!assetTypeColors.ContainsKey("FXAssets"))
            assetTypeColors["FXAssets"] = new Color(0.8f, 0.2f, 0.8f); // Purple
            
        if (!assetTypeColors.ContainsKey("Favorites"))
            assetTypeColors["Favorites"] = new Color(0.8f, 0.8f, 0.2f); // Yellow
            
        if (!assetTypeColors.ContainsKey("Resources"))
            assetTypeColors["Resources"] = new Color(0.5f, 0.5f, 0.5f); // Gray
    }
    
    private void SaveColorPreferences()
    {
        string typeNames = string.Join(",", assetTypeColors.Keys);
        EditorPrefs.SetString("ASSETSBrowser_TypeNames", typeNames);
        
        foreach (var kvp in assetTypeColors)
        {
            string colorStr = $"{kvp.Value.r},{kvp.Value.g},{kvp.Value.b},{kvp.Value.a}";
            EditorPrefs.SetString("ASSETSBrowser_Color_" + kvp.Key, colorStr);
        }
    }
    
    private void LoadFavorites()
    {
        string favoritesStr = EditorPrefs.GetString("ASSETSBrowser_Favorites", "");
        string[] paths = favoritesStr.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        favoritePaths.Clear();
        favoriteObjects.Clear();
        
        foreach (string path in paths)
        {
            favoritePaths[path] = true;
            ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (obj != null)
            {
                favoriteObjects.Add(obj);
            }
        }
    }
    
    private void SaveFavorites()
    {
        string favoritesStr = string.Join("|", favoritePaths.Keys.Where(k => favoritePaths[k]));
        EditorPrefs.SetString("ASSETSBrowser_Favorites", favoritesStr);
    }
    
    private void LoadExpandedStates()
    {
        string expandedStatesStr = EditorPrefs.GetString("ASSETSBrowser_ExpandedStates", "");
        string[] states = expandedStatesStr.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        expandedStates.Clear();
        
        foreach (string state in states)
        {
            string[] parts = state.Split(new char[] { '=' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                string path = parts[0];
                bool expanded = bool.Parse(parts[1]);
                expandedStates[path] = expanded;
            }
        }
    }
    
    private void SaveExpandedStates()
    {
        string expandedStatesStr = string.Join("|", 
            expandedStates.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        EditorPrefs.SetString("ASSETSBrowser_ExpandedStates", expandedStatesStr);
    }
    
    private void RefreshAssets()
    {
        assetCategories.Clear();
        allAssetPaths.Clear();
        
        // Add "Favorites" category
        assetCategories["Favorites"] = new List<ScriptableObject>(favoriteObjects);
        
        // Create categories based on the ASSETS singleton structure
        assetCategories["LevelAssets"] = new List<ScriptableObject>();
        assetCategories["CharacterPrefabAssets"] = new List<ScriptableObject>();
        assetCategories["UIAssets"] = new List<ScriptableObject>();
        assetCategories["FXAssets"] = new List<ScriptableObject>();
        
        // Get the assets from the ASSETS singleton
        var levelAssets = ASSETS.LevelAssets;
        var characterAssets = ASSETS.Players;
        var uiAssets = ASSETS.ui;
        var fxAssets = ASSETS.FX;
        
        // Process Level Assets
        if (levelAssets != null)
        {
            AddAssetToCategory(levelAssets, "LevelAssets");
            ProcessScriptableObjectFields(levelAssets, "LevelAssets");
        }
        
        // Process Character Assets
        if (characterAssets != null)
        {
            AddAssetToCategory(characterAssets, "CharacterPrefabAssets");
            ProcessScriptableObjectFields(characterAssets, "CharacterPrefabAssets");
        }
        
        // Process UI Assets
        if (uiAssets != null)
        {
            AddAssetToCategory(uiAssets, "UIAssets");
            ProcessScriptableObjectFields(uiAssets, "UIAssets");
        }
        
        // Process FX Assets
        if (fxAssets != null)
        {
            AddAssetToCategory(fxAssets, "FXAssets");
            ProcessScriptableObjectFields(fxAssets, "FXAssets");
        }
        
        // Load Resources folders
        LoadResourcesFolders();
        
        // Setup category names array
        categoryNames = assetCategories.Keys.ToArray();
        
        // If selected category is empty or invalid, select the first category
        if (string.IsNullOrEmpty(selectedCategoryName) || !assetCategories.ContainsKey(selectedCategoryName))
        {
            if (categoryNames.Length > 0)
            {
                selectedCategoryName = categoryNames[0];
            }
        }
    }
    
    private void LoadResourcesFolders()
    {
        // Check if the Assets/Resources folder exists
        string resourcesPath = "Assets/Resources";
        if (Directory.Exists(resourcesPath))
        {
            // Get all folders in the Resources directory
            string[] directories = Directory.GetDirectories(resourcesPath);
            
            foreach (string directory in directories)
            {
                string folderName = Path.GetFileName(directory);
                string categoryName = "Resources/" + folderName;
                
                // Add category if it doesn't exist
                if (!assetCategories.ContainsKey(categoryName))
                {
                    assetCategories[categoryName] = new List<ScriptableObject>();
                }
                
                // Find all ScriptableObjects in this folder
                string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { directory });
                
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                    
                    if (obj != null)
                    {
                        AddAssetToCategory(obj, categoryName);
                    }
                }
            }
        }
    }
    
    private void AddAssetToCategory(ScriptableObject asset, string category)
    {
        if (asset == null) return;
        
        // Add to category
        if (!assetCategories[category].Contains(asset))
        {
            assetCategories[category].Add(asset);
        }
        
        // Add to path tracking
        string path = AssetDatabase.GetAssetPath(asset);
        if (!allAssetPaths.Contains(path))
        {
            allAssetPaths.Add(path);
        }
        
        // Add to favorites if marked
        if (favoritePaths.ContainsKey(path) && favoritePaths[path])
        {
            if (!assetCategories["Favorites"].Contains(asset))
            {
                assetCategories["Favorites"].Add(asset);
            }
        }
        
        // Ensure we have an expanded state for this asset
        if (!expandedStates.ContainsKey(path))
        {
            expandedStates[path] = true; // Default to expanded
        }
    }
    
    private void ProcessScriptableObjectFields(ScriptableObject asset, string category)
    {
        if (asset == null) return;
        
        // Get all fields
        var fields = asset.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            // Check if field is ScriptableObject
            if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
            {
                var nestedAsset = field.GetValue(asset) as ScriptableObject;
                if (nestedAsset != null)
                {
                    AddAssetToCategory(nestedAsset, category);
                }
            }
            // Check for collections of ScriptableObjects
            else if (field.FieldType.IsArray && typeof(ScriptableObject).IsAssignableFrom(field.FieldType.GetElementType()))
            {
                var array = field.GetValue(asset) as ScriptableObject[];
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        if (item != null)
                        {
                            AddAssetToCategory(item, category);
                        }
                    }
                }
            }
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(field.FieldType) && 
                     field.FieldType.IsGenericType && 
                     typeof(ScriptableObject).IsAssignableFrom(field.FieldType.GetGenericArguments()[0]))
            {
                var collection = field.GetValue(asset) as System.Collections.IEnumerable;
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        if (item is ScriptableObject scriptableObj)
                        {
                            AddAssetToCategory(scriptableObj, category);
                        }
                    }
                }
            }
        }
    }
    
    private void OnGUI()
    {
        if (categoryNames == null || categoryNames.Length == 0)
        {
            RefreshAssets();
        }
        
        // Main layout
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        // Left panel (categories)
        DrawLeftPanel();
        
        // Resize handle for left panel
        DrawResizeHandle(ref leftPanelWidth, ref resizingLeftPanel, true);
        
        // Main content area
        DrawMainContent();
        
        // Right panel (details)
        if (showRightPanel)
        {
            // Resize handle for right panel
            DrawResizeHandle(ref rightPanelWidth, ref resizingRightPanel, false);
            
            // Right panel
            DrawRightPanel();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawResizeHandle(ref float panelWidth, ref bool resizing, bool isLeftPanel)
    {
        Rect resizeHandleRect = isLeftPanel 
            ? new Rect(leftPanelWidth, 0, 5, position.height)
            : new Rect(position.width - rightPanelWidth - 5, 0, 5, position.height);
        
        EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
        
        if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
        {
            resizing = true;
            Event.current.Use();
        }
        
        if (resizing && Event.current.type == EventType.MouseDrag)
        {
            if (isLeftPanel)
            {
                panelWidth += Event.current.delta.x;
                panelWidth = Mathf.Clamp(panelWidth, 100, position.width - 200);
            }
            else
            {
                panelWidth -= Event.current.delta.x;
                panelWidth = Mathf.Clamp(panelWidth, 200, position.width - leftPanelWidth - 100);
            }
            Event.current.Use();
            Repaint();
        }
        
        if (Event.current.type == EventType.MouseUp)
        {
            resizing = false;
            Event.current.Use();
        }
        
        // Draw the resize handle
        EditorGUI.DrawRect(resizeHandleRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
    }
    
    private void DrawLeftPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(leftPanelWidth), GUILayout.ExpandHeight(true));
        
        // Search box
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        searchQuery = EditorGUILayout.TextField(searchQuery, searchStyle);
        if (GUILayout.Button("×", EditorStyles.miniButtonRight, GUILayout.Width(20)))
        {
            searchQuery = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
        
        // Settings buttons
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
        {
            RefreshAssets();
        }
        
        if (GUILayout.Button("Colors", EditorStyles.toolbarButton))
        {
            ShowColorSettings();
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Collapse/Expand all
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        collapseAll = GUILayout.Toggle(collapseAll, "Collapse All", EditorStyles.toolbarButton);
        if (GUI.changed)
        {
            // Update all expanded states when toggle changes
            foreach (var path in allAssetPaths)
            {
                expandedStates[path] = !collapseAll;
            }
            SaveExpandedStates();
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Category tabs list
        leftScrollPosition = EditorGUILayout.BeginScrollView(leftScrollPosition);
        
        foreach (string categoryName in categoryNames)
        {
            bool isSelected = categoryName == selectedCategoryName;
            
            if (GUILayout.Toggle(isSelected, categoryName, isSelected ? selectedTabStyle : tabStyle, 
                GUILayout.Height(25)))
            {
                if (!isSelected)
                {
                    selectedCategoryName = categoryName;
                    // Reset selection when changing tabs
                    selectedObject = null;
                    showRightPanel = false;
                }
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawMainContent()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        // Header for current category
        EditorGUILayout.LabelField(selectedCategoryName, headerStyle);
        
        // Main scroll view
        mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);
        
        if (!string.IsNullOrEmpty(selectedCategoryName) && assetCategories.ContainsKey(selectedCategoryName))
        {
            List<ScriptableObject> objectsToShow = assetCategories[selectedCategoryName];
            
            // Filter objects based on search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                objectsToShow = objectsToShow
                    .Where(obj => obj != null && obj.name.ToLower().Contains(searchQuery.ToLower()))
                    .ToList();
            }
            
            foreach (ScriptableObject obj in objectsToShow)
            {
                DrawAssetInMainPanel(obj, selectedCategoryName);
            }
            
            if (objectsToShow.Count == 0)
            {
                EditorGUILayout.HelpBox("No assets found in this category.", MessageType.Info);
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawRightPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(rightPanelWidth), GUILayout.ExpandHeight(true));
        
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        EditorGUILayout.LabelField("Details", EditorStyles.boldLabel);
        
        if (GUILayout.Button("×", EditorStyles.miniButtonRight, GUILayout.Width(20)))
        {
            showRightPanel = false;
        }
        EditorGUILayout.EndHorizontal();
        
        if (selectedObject != null)
        {
            rightScrollPosition = EditorGUILayout.BeginScrollView(rightScrollPosition);
            
            // Object name
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
            string newName = EditorGUILayout.TextField(selectedObject.name);
            if (newName != selectedObject.name)
            {
                string path = AssetDatabase.GetAssetPath(selectedObject);
                AssetDatabase.RenameAsset(path, newName);
            }
            
            EditorGUILayout.Space();
            
            // Asset path
            string path2 = AssetDatabase.GetAssetPath(selectedObject);
            EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(path2, EditorStyles.wordWrappedLabel);
            
            EditorGUILayout.Space();
            
            // Type info
            string typeName = selectedObject.GetType().Name;
            EditorGUILayout.LabelField("Type", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(typeName);
            
            // Find the asset category
            string assetCategory = "Unknown";
            foreach (var category in assetCategories.Keys)
            {
                if (assetCategories[category].Contains(selectedObject) && category != "Favorites")
                {
                    assetCategory = category;
                    break;
                }
            }
            
            // Category info
            EditorGUILayout.LabelField("Category", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(assetCategory);
            
            // Color indicator for category
            Color categoryColor = assetTypeColors.ContainsKey(assetCategory) ? assetTypeColors[assetCategory] : Color.gray;
            Rect colorRect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.DrawRect(colorRect, categoryColor);
            
            EditorGUILayout.Space();
            
            // Properties
            EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
            
            SerializedObject serializedObject = new SerializedObject(selectedObject);
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                // Skip script field
                if (iterator.name == "m_Script") continue;
                
                EditorGUILayout.PropertyField(iterator, true);
            }
            
            // Apply any changes
            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUILayout.Space();
            
            // Quick actions
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Duplicate"))
            {
                DuplicateObject(selectedObject);
            }
            
            if (GUILayout.Button("Ping"))
            {
                EditorGUIUtility.PingObject(selectedObject);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select"))
            {
                Selection.activeObject = selectedObject;
            }
            
            if (GUILayout.Button("Edit in Inspector"))
            {
                OpenInInspector(selectedObject);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("Select an asset to view details.", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawAssetInMainPanel(ScriptableObject obj, string category)
    {
        if (obj == null) return;
        
        string path = AssetDatabase.GetAssetPath(obj);
        
        // Get the color for this category
        Color categoryColor = assetTypeColors.ContainsKey(category) ? assetTypeColors[category] : Color.gray;
        
        // Determine if expanded
        bool isExpanded = expandedStates.ContainsKey(path) && expandedStates[path];
        
        // Begin object box with background color
        Texture2D bgTexture = MakeColorTexture(new Color(categoryColor.r, categoryColor.g, categoryColor.b, 0.15f));
        GUIStyle customBoxStyle = isExpanded ? new GUIStyle(boxStyle) : new GUIStyle(collapsedBoxStyle);
        customBoxStyle.normal.background = bgTexture;
        
        EditorGUILayout.BeginVertical(customBoxStyle);
        
        // Object header with type color
        EditorGUILayout.BeginHorizontal();
        
        // Expand/Collapse button
        Texture2D toggleIcon = isExpanded ? collapseIcon : expandIcon;
        if (GUILayout.Button(new GUIContent(toggleIcon), EditorStyles.miniButton, GUILayout.Width(20), GUILayout.Height(20)))
        {
            expandedStates[path] = !isExpanded;
            SaveExpandedStates();
        }
        
        // Type label with color
        GUIStyle colorLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        colorLabelStyle.normal.textColor = categoryColor;
        EditorGUILayout.LabelField(obj.GetType().Name, colorLabelStyle, GUILayout.Width(120));
        
        // Object name as a field
        string newName = EditorGUILayout.TextField(obj.name);
        if (newName != obj.name)
        {
            AssetDatabase.RenameAsset(path, newName);
        }
        
        // Favorite toggle
        bool isFavorite = favoritePaths.ContainsKey(path) && favoritePaths[path];
        Texture2D starTexture = isFavorite ? starFilledIcon : starIcon;
        
        if (GUILayout.Button(new GUIContent(starTexture), favoriteStyle, GUILayout.Width(25), GUILayout.Height(20)))
        {
            favoritePaths[path] = !isFavorite;
            SaveFavorites();
            RefreshAssets();
        }
        
        // Details button
        if (GUILayout.Button("Details", EditorStyles.miniButton, GUILayout.Width(50)))
        {
            selectedObject = obj;
            showRightPanel = true;
        }
        
        // Ping button
        if (GUILayout.Button("Ping", EditorStyles.miniButton, GUILayout.Width(40)))
        {
            EditorGUIUtility.PingObject(obj);
        }
        
        EditorGUILayout.EndHorizontal();
        
        // If expanded, show more details
        if (isExpanded)
        {
            // Path display
            EditorGUILayout.LabelField(path, EditorStyles.miniLabel);
            
            // Separator
            EditorGUILayout.Space();
            
            // Show all properties
            SerializedObject serializedObject = new SerializedObject(obj);
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                // Skip script field
                if (iterator.name == "m_Script") continue;
                
                EditorGUILayout.PropertyField(iterator, true);
            }
            
            // Apply any changes
            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            // Action buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Select"))
            {
                Selection.activeObject = obj;
            }
            
            if (GUILayout.Button("Duplicate"))
            {
                DuplicateObject(obj);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
    }
    
    private void DuplicateObject(ScriptableObject obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        string newPath = AssetDatabase.GenerateUniqueAssetPath(path);
        
        AssetDatabase.CopyAsset(path, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        RefreshAssets();
    }
    
    private void OpenInInspector(ScriptableObject obj)
    {
        Selection.activeObject = obj;
        EditorUtility.FocusProjectWindow();
    }
    
    private void ShowColorSettings()
    {
        ColorSettingsWindow window = EditorWindow.GetWindow<ColorSettingsWindow>(true, "Category Color Settings");
        window.SetBrowser(this);
        window.Show();
    }
    
    public Dictionary<string, Color> GetAssetTypeColors()
    {
        return assetTypeColors;
    }
    
    public void SetAssetTypeColors(Dictionary<string, Color> colors)
    {
        assetTypeColors = new Dictionary<string, Color>(colors);
        SaveColorPreferences();
        Repaint();
    }
    
    private Texture2D MakeColorTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
    
    // Color Settings Window
    public class ColorSettingsWindow : EditorWindow
    {
        private ASSETSBrowser browser;
        private Dictionary<string, Color> editableColors = new Dictionary<string, Color>();
        private Vector2 scrollPosition;
        
        public void SetBrowser(ASSETSBrowser browserWindow)
        {
            browser = browserWindow;
            editableColors = new Dictionary<string, Color>(browser.GetAssetTypeColors());
        }
        
        private void OnGUI()
        {
            if (browser == null) return;
            
            EditorGUILayout.LabelField("Asset Category Colors", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            var keys = editableColors.Keys.ToList();
            foreach (var key in keys)
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(key, GUILayout.Width(150));
                Color newColor = EditorGUILayout.ColorField(editableColors[key]);
                
                if (newColor != editableColors[key])
                {
                    editableColors[key] = newColor;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Apply"))
            {
                browser.SetAssetTypeColors(editableColors);
            }
            
            if (GUILayout.Button("Reset to Defaults"))
            {
                ResetToDefaults();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void ResetToDefaults()
        {
            editableColors.Clear();
            
            // Set default colors
            editableColors["LevelAssets"] = new Color(0.2f, 0.6f, 0.3f); // Green
            editableColors["CharacterPrefabAssets"] = new Color(0.6f, 0.3f, 0.2f); // Orange/Brown
            editableColors["UIAssets"] = new Color(0.2f, 0.3f, 0.8f); // Blue
            editableColors["FXAssets"] = new Color(0.8f, 0.2f, 0.8f); // Purple
            editableColors["Favorites"] = new Color(0.8f, 0.8f, 0.2f); // Yellow
            editableColors["Resources"] = new Color(0.5f, 0.5f, 0.5f); // Gray
            
            Repaint();
        }
    }
}