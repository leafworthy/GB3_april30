using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace GangstaBean.Console.Editor
{
    public class NewConsoleSetupEditor : EditorWindow
    {
        private bool setupConsoleCanvas = true;
        private bool setupConsoleManager = true;
        private bool replaceExistingConsole = false;
        private Canvas targetCanvas;
        private GameObject existingConsole;
        
        [MenuItem("Tools/Console/New Console Setup")]
        public static void ShowWindow()
        {
            GetWindow<NewConsoleSetupEditor>("New Console Setup");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("New Console Layout Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            setupConsoleCanvas = EditorGUILayout.Toggle("Setup Console Canvas", setupConsoleCanvas);
            setupConsoleManager = EditorGUILayout.Toggle("Setup Console Manager", setupConsoleManager);
            replaceExistingConsole = EditorGUILayout.Toggle("Replace Existing Console", replaceExistingConsole);
            
            GUILayout.Space(10);
            
            if (replaceExistingConsole)
            {
                existingConsole = EditorGUILayout.ObjectField("Existing Console", existingConsole, typeof(GameObject), true) as GameObject;
                targetCanvas = EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true) as Canvas;
            }
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("Setup New Console", GUILayout.Height(30)))
            {
                SetupNewConsole();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Create Console Theme Asset", GUILayout.Height(25)))
            {
                CreateConsoleThemeAsset();
            }
            
            GUILayout.Space(20);
            EditorGUILayout.HelpBox("This will create a modern multi-panel console layout with resizable panels, command browser, and theming support.", MessageType.Info);
        }
        
        private void SetupNewConsole()
        {
            // Ensure EventSystem exists
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
            }
            
            // Setup canvas
            Canvas canvas = targetCanvas;
            if (setupConsoleCanvas || canvas == null)
            {
                canvas = CreateConsoleCanvas();
            }
            
            // Remove existing console if requested
            if (replaceExistingConsole && existingConsole != null)
            {
                DestroyImmediate(existingConsole);
            }
            
            // Create new console hierarchy
            GameObject consoleRoot = CreateConsoleHierarchy(canvas);
            
            // Setup console manager
            if (setupConsoleManager)
            {
                SetupConsoleManagerConnection(consoleRoot);
            }
            
            Debug.Log("New Console Setup Complete!");
            EditorUtility.SetDirty(canvas.gameObject);
        }
        
        private Canvas CreateConsoleCanvas()
        {
            GameObject canvasGO = new GameObject("Console Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            return canvas;
        }
        
        private GameObject CreateConsoleHierarchy(Canvas canvas)
        {
            // Root console panel
            GameObject consolePanel = CreateUIElement("Console Panel", canvas.transform, true);
            SetFullScreen(consolePanel.GetComponent<RectTransform>());
            
            Image consoleBg = consolePanel.AddComponent<Image>();
            consoleBg.color = new Color(0, 0, 0, 0.8f);
            consolePanel.SetActive(false);
            
            // Header bar
            GameObject headerBar = CreateHeaderBar(consolePanel.transform);
            
            // Content area
            GameObject contentArea = CreateContentArea(consolePanel.transform);
            
            // Status bar
            GameObject statusBar = CreateStatusBar(consolePanel.transform);
            
            // Create main and side panels in content area
            GameObject mainPanel = CreateMainPanel(contentArea.transform);
            GameObject splitterHandle = CreateSplitterHandle(contentArea.transform);
            GameObject sidePanel = CreateSidePanel(contentArea.transform);
            
            // Add and configure components
            SetupComponents(consolePanel, headerBar, mainPanel, sidePanel, splitterHandle, statusBar);
            
            return consolePanel;
        }
        
        private GameObject CreateHeaderBar(Transform parent)
        {
            GameObject headerBar = CreateUIElement("Header Bar", parent, false);
            RectTransform headerRect = headerBar.GetComponent<RectTransform>();
            
            // Anchor to top, stretch horizontally
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 30);
            
            Image headerBg = headerBar.AddComponent<Image>();
            headerBg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // Console title
            GameObject title = CreateTextElement("Console Title", headerBar.transform, "Debug Console v2.0");
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0.8f, 1);
            titleRect.offsetMin = new Vector2(10, 0);
            titleRect.offsetMax = new Vector2(0, 0);
            
            // Minimize button
            GameObject minimizeBtn = CreateButton("Minimize Button", headerBar.transform, "-");
            RectTransform minRect = minimizeBtn.GetComponent<RectTransform>();
            minRect.anchorMin = new Vector2(0.85f, 0.1f);
            minRect.anchorMax = new Vector2(0.92f, 0.9f);
            minRect.offsetMin = Vector2.zero;
            minRect.offsetMax = Vector2.zero;
            
            // Close button
            GameObject closeBtn = CreateButton("Close Button", headerBar.transform, "X");
            RectTransform closeRect = closeBtn.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.93f, 0.1f);
            closeRect.anchorMax = new Vector2(1f, 0.9f);
            closeRect.offsetMin = Vector2.zero;
            closeRect.offsetMax = new Vector2(-5, 0);
            
            return headerBar;
        }
        
        private GameObject CreateContentArea(Transform parent)
        {
            GameObject contentArea = CreateUIElement("Content Area", parent, false);
            RectTransform contentRect = contentArea.GetComponent<RectTransform>();
            
            // Fill space between header and status bar
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = new Vector2(0, 20); // Above status bar
            contentRect.offsetMax = new Vector2(0, -30); // Below header bar
            
            return contentArea;
        }
        
        private GameObject CreateStatusBar(Transform parent)
        {
            GameObject statusBar = CreateUIElement("Status Bar", parent, false);
            RectTransform statusRect = statusBar.GetComponent<RectTransform>();
            
            // Anchor to bottom, stretch horizontally
            statusRect.anchorMin = new Vector2(0, 0);
            statusRect.anchorMax = new Vector2(1, 0);
            statusRect.anchoredPosition = Vector2.zero;
            statusRect.sizeDelta = new Vector2(0, 20);
            
            Image statusBg = statusBar.AddComponent<Image>();
            statusBg.color = new Color(0.08f, 0.08f, 0.08f, 1f);
            
            // FPS text
            GameObject fpsText = CreateTextElement("FPS Text", statusBar.transform, "FPS: 60");
            RectTransform fpsRect = fpsText.GetComponent<RectTransform>();
            fpsRect.anchorMin = new Vector2(0, 0);
            fpsRect.anchorMax = new Vector2(0.3f, 1);
            fpsRect.offsetMin = new Vector2(10, 0);
            fpsRect.offsetMax = Vector2.zero;
            
            // Memory text
            GameObject memText = CreateTextElement("Memory Text", statusBar.transform, "Memory: 100 MB");
            RectTransform memRect = memText.GetComponent<RectTransform>();
            memRect.anchorMin = new Vector2(0.3f, 0);
            memRect.anchorMax = new Vector2(0.7f, 1);
            memRect.offsetMin = Vector2.zero;
            memRect.offsetMax = Vector2.zero;
            
            // Time text
            GameObject timeText = CreateTextElement("Time Text", statusBar.transform, "Time: 0.00");
            RectTransform timeRect = timeText.GetComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(0.7f, 0);
            timeRect.anchorMax = new Vector2(1, 1);
            timeRect.offsetMin = Vector2.zero;
            timeRect.offsetMax = new Vector2(-10, 0);
            
            return statusBar;
        }
        
        private GameObject CreateMainPanel(Transform parent)
        {
            GameObject mainPanel = CreateUIElement("Main Panel", parent, false);
            RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
            
            // 75% width, left side
            mainRect.anchorMin = new Vector2(0, 0);
            mainRect.anchorMax = new Vector2(0.75f, 1);
            mainRect.offsetMin = Vector2.zero;
            mainRect.offsetMax = new Vector2(-2, 0); // Leave space for splitter
            
            Image mainBg = mainPanel.AddComponent<Image>();
            mainBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Output scroll view
            GameObject scrollView = CreateScrollView("Output Scroll View", mainPanel.transform);
            
            // Input field
            GameObject inputField = CreateInputField("Input Field", mainPanel.transform);
            
            // Suggestion panel
            GameObject suggestionPanel = CreateSuggestionPanel("Suggestion Panel", mainPanel.transform);
            
            return mainPanel;
        }
        
        private GameObject CreateSplitterHandle(Transform parent)
        {
            GameObject splitter = CreateUIElement("Splitter Handle", parent, false);
            RectTransform splitterRect = splitter.GetComponent<RectTransform>();
            
            // 4px wide, between panels
            splitterRect.anchorMin = new Vector2(0.75f, 0);
            splitterRect.anchorMax = new Vector2(0.75f, 1);
            splitterRect.anchoredPosition = Vector2.zero;
            splitterRect.sizeDelta = new Vector2(4, 0);
            
            Image splitterImg = splitter.AddComponent<Image>();
            splitterImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            // Add cursor change on hover
            splitter.AddComponent<EventTrigger>();
            
            return splitter;
        }
        
        private GameObject CreateSidePanel(Transform parent)
        {
            GameObject sidePanel = CreateUIElement("Side Panel", parent, false);
            RectTransform sideRect = sidePanel.GetComponent<RectTransform>();
            
            // 25% width, right side
            sideRect.anchorMin = new Vector2(0.75f, 0);
            sideRect.anchorMax = new Vector2(1, 1);
            sideRect.offsetMin = new Vector2(4, 0); // After splitter
            sideRect.offsetMax = Vector2.zero;
            
            Image sideBg = sidePanel.AddComponent<Image>();
            sideBg.color = new Color(0.08f, 0.08f, 0.08f, 1f);
            
            // Tab container
            GameObject tabContainer = CreateTabContainer("Tab Container", sidePanel.transform);
            
            // Command list scroll view
            GameObject commandList = CreateCommandListView("Command List Container", sidePanel.transform);
            
            // Help display
            GameObject helpDisplay = CreateTextElement("Help Display", sidePanel.transform, "Hover over a command to see help information.");
            RectTransform helpRect = helpDisplay.GetComponent<RectTransform>();
            helpRect.anchorMin = new Vector2(0, 0);
            helpRect.anchorMax = new Vector2(1, 0.3f);
            helpRect.offsetMin = new Vector2(5, 5);
            helpRect.offsetMax = new Vector2(-5, 0);
            
            return sidePanel;
        }
        
        private void SetupComponents(GameObject consolePanel, GameObject headerBar, GameObject mainPanel, 
                                   GameObject sidePanel, GameObject splitterHandle, GameObject statusBar)
        {
            // Add ConsolePanelManager
            ConsolePanelManager panelManager = consolePanel.AddComponent<ConsolePanelManager>();
            
            // Add ConsoleThemeManager
            ConsoleThemeManager themeManager = consolePanel.AddComponent<ConsoleThemeManager>();
            
            // Add CommandBrowser to side panel
            CommandBrowser commandBrowser = sidePanel.AddComponent<CommandBrowser>();
            
            // Add ConsoleUI
            ConsoleUI consoleUI = consolePanel.AddComponent<ConsoleUI>();
            
            // Auto-assign references
            AssignReferences(consoleUI, panelManager, commandBrowser, themeManager, 
                           consolePanel, headerBar, mainPanel, sidePanel, splitterHandle, statusBar);
        }
        
        private void AssignReferences(ConsoleUI consoleUI, ConsolePanelManager panelManager, 
                                    CommandBrowser commandBrowser, ConsoleThemeManager themeManager,
                                    GameObject consolePanel, GameObject headerBar, GameObject mainPanel, 
                                    GameObject sidePanel, GameObject splitterHandle, GameObject statusBar)
        {
            // Use reflection to assign private fields
            var consoleUIType = typeof(ConsoleUI);
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            
            // Console UI references
            consoleUIType.GetField("consolePanel", flags)?.SetValue(consoleUI, consolePanel);
            consoleUIType.GetField("headerBar", flags)?.SetValue(consoleUI, headerBar.GetComponent<RectTransform>());
            consoleUIType.GetField("mainPanel", flags)?.SetValue(consoleUI, mainPanel.GetComponent<RectTransform>());
            consoleUIType.GetField("sidePanel", flags)?.SetValue(consoleUI, sidePanel.GetComponent<RectTransform>());
            consoleUIType.GetField("statusBar", flags)?.SetValue(consoleUI, statusBar.GetComponent<RectTransform>());
            consoleUIType.GetField("panelManager", flags)?.SetValue(consoleUI, panelManager);
            consoleUIType.GetField("commandBrowser", flags)?.SetValue(consoleUI, commandBrowser);
            
            // Find and assign other UI elements
            consoleUIType.GetField("inputField", flags)?.SetValue(consoleUI, mainPanel.GetComponentInChildren<TMP_InputField>());
            consoleUIType.GetField("outputText", flags)?.SetValue(consoleUI, mainPanel.GetComponentInChildren<TextMeshProUGUI>());
            consoleUIType.GetField("scrollRect", flags)?.SetValue(consoleUI, mainPanel.GetComponentInChildren<ScrollRect>());
            
            // Find and assign suggestion panel
            var suggestionPanel = mainPanel.transform.Find("Suggestion Panel");
            if (suggestionPanel != null)
            {
                consoleUIType.GetField("suggestionPanel", flags)?.SetValue(consoleUI, suggestionPanel.gameObject);
                consoleUIType.GetField("suggestionContainer", flags)?.SetValue(consoleUI, suggestionPanel);
                
                // Create suggestion button prefab
                GameObject suggestionButtonPrefab = CreateSuggestionButtonPrefab();
                consoleUIType.GetField("suggestionButtonPrefab", flags)?.SetValue(consoleUI, suggestionButtonPrefab.GetComponent<Button>());
            }
            
            var headerTexts = headerBar.GetComponentsInChildren<TextMeshProUGUI>();
            var headerButtons = headerBar.GetComponentsInChildren<Button>();
            if (headerTexts.Length > 0) consoleUIType.GetField("consoleTitle", flags)?.SetValue(consoleUI, headerTexts[0]);
            if (headerButtons.Length > 0) consoleUIType.GetField("minimizeButton", flags)?.SetValue(consoleUI, headerButtons[0]);
            if (headerButtons.Length > 1) consoleUIType.GetField("closeButton", flags)?.SetValue(consoleUI, headerButtons[1]);
            
            var statusTexts = statusBar.GetComponentsInChildren<TextMeshProUGUI>();
            if (statusTexts.Length > 0) consoleUIType.GetField("fpsText", flags)?.SetValue(consoleUI, statusTexts[0]);
            if (statusTexts.Length > 1) consoleUIType.GetField("memoryText", flags)?.SetValue(consoleUI, statusTexts[1]);
            if (statusTexts.Length > 2) consoleUIType.GetField("timeText", flags)?.SetValue(consoleUI, statusTexts[2]);
            
            // Panel Manager references
            var panelManagerType = typeof(ConsolePanelManager);
            panelManagerType.GetField("mainPanel", flags)?.SetValue(panelManager, mainPanel.GetComponent<RectTransform>());
            panelManagerType.GetField("sidePanel", flags)?.SetValue(panelManager, sidePanel.GetComponent<RectTransform>());
            panelManagerType.GetField("splitterHandle", flags)?.SetValue(panelManager, splitterHandle.GetComponent<RectTransform>());
            
            // Command Browser references
            var commandBrowserType = typeof(CommandBrowser);
            var tabContainer = sidePanel.transform.Find("Tab Container");
            var commandListContainer = sidePanel.transform.Find("Command List Container/Viewport/Content");
            var helpDisplay = sidePanel.transform.Find("Help Display");
            var commandScrollRect = sidePanel.GetComponentInChildren<ScrollRect>();
            
            commandBrowserType.GetField("tabContainer", flags)?.SetValue(commandBrowser, tabContainer);
            commandBrowserType.GetField("commandListContainer", flags)?.SetValue(commandBrowser, commandListContainer);
            commandBrowserType.GetField("helpDisplay", flags)?.SetValue(commandBrowser, helpDisplay?.GetComponent<TextMeshProUGUI>());
            commandBrowserType.GetField("commandScrollRect", flags)?.SetValue(commandBrowser, commandScrollRect);
            
            // Create and assign prefabs for CommandBrowser
            CreateCommandBrowserPrefabs(commandBrowser, flags);
        }
        
        private void CreateCommandBrowserPrefabs(CommandBrowser commandBrowser, System.Reflection.BindingFlags flags)
        {
            var commandBrowserType = typeof(CommandBrowser);
            
            // Create Tab Button Prefab
            GameObject tabButtonPrefab = CreateTabButtonPrefab();
            commandBrowserType.GetField("tabButtonPrefab", flags)?.SetValue(commandBrowser, tabButtonPrefab.GetComponent<Button>());
            
            // Create Command Button Prefab
            GameObject commandButtonPrefab = CreateCommandButtonPrefab();
            commandBrowserType.GetField("commandButtonPrefab", flags)?.SetValue(commandBrowser, commandButtonPrefab.GetComponent<Button>());
        }
        
        private GameObject CreateTabButtonPrefab()
        {
            GameObject prefab = new GameObject("TabButtonPrefab");
            
            // Add RectTransform
            RectTransform rect = prefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 25);
            
            // Add Image component
            Image img = prefab.AddComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            // Add Button component
            Button btn = prefab.AddComponent<Button>();
            var colorBlock = btn.colors;
            colorBlock.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colorBlock.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            colorBlock.pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            colorBlock.selectedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            btn.colors = colorBlock;
            
            // Add text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(prefab.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            SetTextMeshProFont(text);
            text.text = "Tab";
            text.fontSize = 10;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            
            // Add Layout Element
            LayoutElement layout = prefab.AddComponent<LayoutElement>();
            layout.minWidth = 60;
            layout.preferredWidth = 80;
            layout.flexibleWidth = 0;
            
            return prefab;
        }
        
        private GameObject CreateCommandButtonPrefab()
        {
            GameObject prefab = new GameObject("CommandButtonPrefab");
            
            // Add RectTransform
            RectTransform rect = prefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 20);
            
            // Add Image component
            Image img = prefab.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Add Button component
            Button btn = prefab.AddComponent<Button>();
            var colorBlock = btn.colors;
            colorBlock.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            colorBlock.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colorBlock.pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            colorBlock.selectedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            btn.colors = colorBlock;
            
            // Add text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(prefab.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 0);
            textRect.offsetMax = new Vector2(-5, 0);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            SetTextMeshProFont(text);
            text.text = "command";
            text.fontSize = 9;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.raycastTarget = false;
            
            // Add Layout Element
            LayoutElement layout = prefab.AddComponent<LayoutElement>();
            layout.minHeight = 18;
            layout.preferredHeight = 20;
            layout.flexibleWidth = 1;
            
            return prefab;
        }
        
        private GameObject CreateSuggestionButtonPrefab()
        {
            GameObject prefab = new GameObject("SuggestionButtonPrefab");
            
            // Add RectTransform
            RectTransform rect = prefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 18);
            
            // Add Image component
            Image img = prefab.AddComponent<Image>();
            img.color = new Color(0.25f, 0.25f, 0.25f, 0.95f);
            
            // Add Button component
            Button btn = prefab.AddComponent<Button>();
            var colorBlock = btn.colors;
            colorBlock.normalColor = new Color(0.25f, 0.25f, 0.25f, 0.95f);
            colorBlock.highlightedColor = new Color(0.35f, 0.35f, 0.35f, 0.95f);
            colorBlock.pressedColor = new Color(0.45f, 0.45f, 0.45f, 0.95f);
            colorBlock.selectedColor = new Color(0.35f, 0.35f, 0.35f, 0.95f);
            btn.colors = colorBlock;
            
            // Add text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(prefab.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 0);
            textRect.offsetMax = new Vector2(-5, 0);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            SetTextMeshProFont(text);
            text.text = "suggestion";
            text.fontSize = 8;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.raycastTarget = false;
            
            // Add Layout Element
            LayoutElement layout = prefab.AddComponent<LayoutElement>();
            layout.minHeight = 16;
            layout.preferredHeight = 18;
            layout.flexibleWidth = 1;
            
            return prefab;
        }
        
        private void SetupConsoleManagerConnection(GameObject consoleRoot)
        {
            ConsoleManager consoleManager = FindObjectOfType<ConsoleManager>();
            if (consoleManager == null)
            {
                GameObject managerGO = new GameObject("Console Manager");
                consoleManager = managerGO.AddComponent<ConsoleManager>();
            }
            
            // Connect console manager to new UI
            var consoleUI = consoleRoot.GetComponent<ConsoleUI>();
            if (consoleUI != null)
            {
                consoleManager.SetConsoleUI(consoleUI);
            }
        }
        
        private void CreateConsoleThemeAsset()
        {
            ConsoleTheme theme = CreateInstance<ConsoleTheme>();
            theme.name = "Default Dark Theme";
            
            string path = "Assets/Resources/Console Themes";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Console Themes");
            }
            
            AssetDatabase.CreateAsset(theme, $"{path}/DefaultDarkTheme.asset");
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Console theme created at {path}/DefaultDarkTheme.asset");
        }
        
        // Helper methods for creating UI elements
        private GameObject CreateUIElement(string name, Transform parent, bool fullScreen)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            
            RectTransform rect = go.AddComponent<RectTransform>();
            if (fullScreen)
            {
                SetFullScreen(rect);
            }
            
            return go;
        }
        
        private void SetFullScreen(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        
        private GameObject CreateTextElement(string name, Transform parent, string text)
        {
            GameObject go = CreateUIElement(name, parent, false);
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            
            // Find and assign a TextMeshPro font asset
            SetTextMeshProFont(tmp);
            
            tmp.text = text;
            tmp.fontSize = 12;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.raycastTarget = false; // Improve performance
            return go;
        }
        
        private void SetTextMeshProFont(TextMeshProUGUI tmp)
        {
            // Try to load LiberationSans SDF (standard TMP font)
            var liberationFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/++SCRIPTS/Plugins/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            if (liberationFont != null)
            {
                tmp.font = liberationFont;
                return;
            }
            
            // Try to find any of the project's custom fonts
            string[] preferredFonts = {
                "Assets/+FONTS/8-BIT WONDER SDF.asset",
                "Assets/+FONTS/Daydream SDF.asset",
                "Assets/+FONTS/SmackyFormula SDF.asset"
            };
            
            foreach (string fontPath in preferredFonts)
            {
                var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
                if (font != null)
                {
                    tmp.font = font;
                    return;
                }
            }
            
            // Find any TMP font asset in the project
            string[] fontGuids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            if (fontGuids.Length > 0)
            {
                string fontPath = AssetDatabase.GUIDToAssetPath(fontGuids[0]);
                tmp.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
                return;
            }
            
            Debug.LogWarning("Could not find any TextMeshPro fonts. Please assign font manually after setup.");
        }
        
        private GameObject CreateButton(string name, Transform parent, string text)
        {
            GameObject go = CreateUIElement(name, parent, false);
            Image img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            Button btn = go.AddComponent<Button>();
            
            GameObject textGO = CreateTextElement("Text", go.transform, text);
            SetFullScreen(textGO.GetComponent<RectTransform>());
            textGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            
            return go;
        }
        
        private GameObject CreateScrollView(string name, Transform parent)
        {
            GameObject scrollView = CreateUIElement(name, parent, false);
            RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0.1f);
            scrollRect.anchorMax = new Vector2(1, 0.9f);
            scrollRect.offsetMin = new Vector2(5, 5);
            scrollRect.offsetMax = new Vector2(-5, -5);
            
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            
            GameObject viewport = CreateUIElement("Viewport", scrollView.transform, true);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            
            GameObject content = CreateUIElement("Content", viewport.transform, false);
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0, 1);
            
            GameObject outputText = CreateTextElement("Output Text", content.transform, "Console ready. Type 'help' for commands.");
            SetFullScreen(outputText.GetComponent<RectTransform>());
            outputText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;
            
            scroll.content = contentRect;
            scroll.viewport = viewport.GetComponent<RectTransform>();
            scroll.vertical = true;
            scroll.horizontal = false;
            
            return scrollView;
        }
        
        private GameObject CreateInputField(string name, Transform parent)
        {
            GameObject inputField = CreateUIElement(name, parent, false);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0, 0);
            inputRect.anchorMax = new Vector2(1, 0.1f);
            inputRect.offsetMin = new Vector2(5, 5);
            inputRect.offsetMax = new Vector2(-5, -5);
            
            Image inputBg = inputField.AddComponent<Image>();
            inputBg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            TMP_InputField input = inputField.AddComponent<TMP_InputField>();
            
            GameObject textArea = CreateUIElement("Text Area", inputField.transform, true);
            textArea.AddComponent<RectMask2D>();
            
            GameObject placeholder = CreateTextElement("Placeholder", textArea.transform, "Enter command...");
            SetFullScreen(placeholder.GetComponent<RectTransform>());
            placeholder.GetComponent<TextMeshProUGUI>().color = Color.gray;
            
            GameObject text = CreateTextElement("Text", textArea.transform, "");
            SetFullScreen(text.GetComponent<RectTransform>());
            
            input.textViewport = textArea.GetComponent<RectTransform>();
            input.textComponent = text.GetComponent<TextMeshProUGUI>();
            input.placeholder = placeholder.GetComponent<TextMeshProUGUI>();
            
            return inputField;
        }
        
        private GameObject CreateSuggestionPanel(string name, Transform parent)
        {
            GameObject suggestionPanel = CreateUIElement(name, parent, false);
            RectTransform suggestionRect = suggestionPanel.GetComponent<RectTransform>();
            
            // Position above input field
            suggestionRect.anchorMin = new Vector2(0, 0.1f);
            suggestionRect.anchorMax = new Vector2(1, 0.3f);
            suggestionRect.offsetMin = new Vector2(5, 0);
            suggestionRect.offsetMax = new Vector2(-5, 0);
            
            suggestionPanel.SetActive(false);
            
            Image bg = suggestionPanel.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
            
            // Add VerticalLayoutGroup for suggestion buttons
            VerticalLayoutGroup layout = suggestionPanel.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 1;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.padding = new RectOffset(2, 2, 2, 2);
            
            return suggestionPanel;
        }
        
        private GameObject CreateTabContainer(string name, Transform parent)
        {
            GameObject tabContainer = CreateUIElement(name, parent, false);
            RectTransform tabRect = tabContainer.GetComponent<RectTransform>();
            tabRect.anchorMin = new Vector2(0, 0.85f);
            tabRect.anchorMax = new Vector2(1, 1);
            tabRect.offsetMin = new Vector2(5, 0);
            tabRect.offsetMax = new Vector2(-5, -5);
            
            HorizontalLayoutGroup layout = tabContainer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 2;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            
            return tabContainer;
        }
        
        private GameObject CreateCommandListView(string name, Transform parent)
        {
            GameObject commandList = CreateUIElement(name, parent, false);
            RectTransform commandRect = commandList.GetComponent<RectTransform>();
            commandRect.anchorMin = new Vector2(0, 0.3f);
            commandRect.anchorMax = new Vector2(1, 0.85f);
            commandRect.offsetMin = new Vector2(5, 5);
            commandRect.offsetMax = new Vector2(-5, -5);
            
            Image bg = commandList.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            
            ScrollRect scroll = commandList.AddComponent<ScrollRect>();
            
            GameObject viewport = CreateUIElement("Viewport", commandList.transform, true);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            
            GameObject content = CreateUIElement("Content", viewport.transform, false);
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0, 1);
            
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 1;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            
            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scroll.content = contentRect;
            scroll.viewport = viewport.GetComponent<RectTransform>();
            scroll.vertical = true;
            scroll.horizontal = false;
            
            return commandList;
        }
    }
}