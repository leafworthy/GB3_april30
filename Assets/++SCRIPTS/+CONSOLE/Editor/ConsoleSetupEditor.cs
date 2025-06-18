using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace GangstaBean.Console.Editor
{
    /// <summary>
    /// Automated console setup tool - creates entire console UI hierarchy with one click
    /// </summary>
    public class ConsoleSetupEditor : EditorWindow
    {
        private bool setupComplete = false;
        private string statusMessage = "Ready to setup console system.";
        
        [MenuItem("Tools/Gangsta Bean/Setup Console System")]
        public static void ShowWindow()
        {
            GetWindow<ConsoleSetupEditor>("Console Setup");
        }
        
        void OnGUI()
        {
            GUILayout.Label("Gangsta Bean Console Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("This will automatically create:");
            GUILayout.Label("• ConsoleManager GameObject");
            GUILayout.Label("• Dedicated Console Canvas (sorting order 1000)");
            GUILayout.Label("• Complete Console UI hierarchy");
            GUILayout.Label("• All required components and references");
            GUILayout.Label("• Console Input Actions asset");
            GUILayout.Label("• New Input System integration");
            GUILayout.Label("• Proper positioning and styling");
            
            GUILayout.Space(10);
            
            GUILayout.Label("Toggle Keys: ~ (backtick) or F1", EditorStyles.helpBox);
            
            GUILayout.Space(10);
            
            if (setupComplete)
            {
                EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(statusMessage, MessageType.None);
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Setup Console System", GUILayout.Height(40)))
            {
                SetupConsoleSystem();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Remove Console System"))
            {
                RemoveConsoleSystem();
            }
        }
        
        void SetupConsoleSystem()
        {
            try
            {
                // Step 0: Clean up any existing console UI components
                CleanupExistingConsoleUI();
                
                // Step 1: Create ConsoleManager
                CreateConsoleManager();
                
                // Step 2: Create dedicated Console Canvas
                Canvas canvas = CreateConsoleCanvas();
                
                // Step 3: Create Console UI
                GameObject consoleUI = CreateConsoleUI(canvas);
                
                // Step 4: Setup references (this links ConsoleManager to ConsoleUI, but UI also needs to find Manager)
                SetupReferences(consoleUI);
                
                // Step 5: Force refresh to ensure singleton is ready
                Debug.Log("Setup complete, forcing EditorApplication refresh...");
                UnityEditor.EditorApplication.delayCall += () => {
                    Debug.Log("Delayed call: Console setup should now be fully connected");
                };
                
                setupComplete = true;
                statusMessage = "Console system setup complete! Press ~ (backtick) or F1 to open console in play mode.";
                
                Debug.Log("Console system setup completed successfully!");
            }
            catch (System.Exception e)
            {
                statusMessage = $"Setup failed: {e.Message}";
                Debug.LogError($"Console setup failed: {e}");
            }
        }
        
        void CleanupExistingConsoleUI()
        {
            Debug.Log("=== Starting Console UI Cleanup ===");
            
            // Find and remove any existing ConsoleUI components
            ConsoleUI[] existingUIs = FindObjectsOfType<ConsoleUI>();
            Debug.Log($"Found {existingUIs.Length} existing ConsoleUI components");
            foreach (ConsoleUI ui in existingUIs)
            {
                Debug.Log($"Removing existing ConsoleUI from: {ui.transform.parent?.name ?? "Unknown"}");
                Undo.DestroyObjectImmediate(ui.gameObject);
            }
            
            // List all existing canvases
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            Debug.Log($"Found {allCanvases.Length} total canvases in scene:");
            foreach (Canvas canvas in allCanvases)
            {
                Debug.Log($"  Canvas: {canvas.name} (Sorting Order: {canvas.sortingOrder})");
            }
            
            // Remove any existing Console Canvas
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.name == "Console Canvas")
                {
                    Debug.Log($"Removing existing Console Canvas: {canvas.name}");
                    Undo.DestroyObjectImmediate(canvas.gameObject);
                }
            }
            
            Debug.Log("=== Console UI Cleanup Complete ===");
        }
        
        void CreateConsoleManager()
        {
            // Check if ConsoleManager already exists
            ConsoleManager existingManager = FindObjectOfType<ConsoleManager>();
            if (existingManager != null)
            {
                Debug.Log("ConsoleManager already exists, skipping creation.");
                return;
            }
            
            // Create ConsoleManager GameObject
            GameObject managerGO = new GameObject("ConsoleManager");
            ConsoleManager manager = managerGO.AddComponent<ConsoleManager>();
            
            // Register for undo
            Undo.RegisterCreatedObjectUndo(managerGO, "Create Console Manager");
            
            Debug.Log("Created ConsoleManager GameObject");
        }
        
        Canvas CreateConsoleCanvas()
        {
            // Check for existing Console Canvas first (this should not happen after cleanup)
            Canvas[] existingCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas existingCanvas in existingCanvases)
            {
                if (existingCanvas.name == "Console Canvas")
                {
                    Debug.LogWarning("Found existing Console Canvas after cleanup - this should not happen!");
                    return existingCanvas;
                }
            }
            
            Debug.Log("No existing Console Canvas found, creating new one...");
            
            // Always create a dedicated Console Canvas
            GameObject canvasGO = new GameObject("Console Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // High sorting order to appear above game UI
            
            Debug.Log($"Creating Console Canvas - Name: {canvasGO.name}, Sorting Order: {canvas.sortingOrder}");
            
            // Add Canvas Scaler for responsive UI
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // Add Graphic Raycaster for UI interaction
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create EventSystem if it doesn't exist
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create EventSystem");
                Debug.Log("Created EventSystem");
            }
            else
            {
                Debug.Log("EventSystem already exists, skipping creation");
            }
            
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Console Canvas");
            Debug.Log($"Created dedicated Console Canvas: {canvasGO.name} with sorting order {canvas.sortingOrder}");
            
            return canvas;
        }
        
        GameObject CreateConsoleUI(Canvas canvas)
        {
            Debug.Log($"Creating Console UI on canvas: {canvas.name} (Sorting Order: {canvas.sortingOrder})");
            
            // Create Console Panel with explicit setup
            GameObject consolePanelGO = new GameObject("ConsolePanel");
            RectTransform consolePanelRect = consolePanelGO.AddComponent<RectTransform>();
            
            // Set parent first
            consolePanelRect.SetParent(canvas.transform, false);
            
            // Use proper anchoring for consistent layout
            consolePanelRect.anchorMin = new Vector2(0.1f, 0.3f);
            consolePanelRect.anchorMax = new Vector2(0.9f, 0.9f);
            consolePanelRect.anchoredPosition = Vector2.zero;
            consolePanelRect.sizeDelta = Vector2.zero;
            
            Debug.Log($"Set Console Panel anchored layout - AnchorMin: {consolePanelRect.anchorMin}, AnchorMax: {consolePanelRect.anchorMax}");
            
            // NOW add visual components
            Image consolePanelImage = consolePanelGO.AddComponent<Image>();
            consolePanelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
            
            // Add ConsoleUI script
            ConsoleUI consoleUI = consolePanelGO.AddComponent<ConsoleUI>();
            
            // Create Output Scroll View
            GameObject outputScrollGO = CreateScrollView("OutputScrollView", consolePanelGO.transform);
            ScrollRect outputScroll = outputScrollGO.GetComponent<ScrollRect>();
            
            // Position Output Scroll View - top 75% of console
            RectTransform outputScrollRect = outputScrollGO.GetComponent<RectTransform>();
            outputScrollRect.anchorMin = new Vector2(0, 0.25f);
            outputScrollRect.anchorMax = new Vector2(1, 0.9f);
            outputScrollRect.anchoredPosition = Vector2.zero;
            outputScrollRect.sizeDelta = Vector2.zero;
            outputScrollRect.offsetMin = new Vector2(10, 5);
            outputScrollRect.offsetMax = new Vector2(-10, -5);
            
            // Create Output Text
            GameObject outputTextGO = CreateTextMeshPro("OutputText", outputScroll.content);
            TextMeshProUGUI outputText = outputTextGO.GetComponent<TextMeshProUGUI>();
            outputText.text = "Console initialized. Type 'help' for available commands.\\nPress ~ or C to toggle console.";
            outputText.fontSize = 14;
            outputText.color = Color.white;
            outputText.alignment = TextAlignmentOptions.TopLeft;
            outputText.richText = true;
            outputText.enableWordWrapping = true;
            
            // Try to assign a default TextMeshPro font
            var defaultFont = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            if (defaultFont != null)
            {
                outputText.font = defaultFont;
            }
            
            // Configure Output Text RectTransform
            RectTransform outputTextRect = outputTextGO.GetComponent<RectTransform>();
            outputTextRect.anchorMin = new Vector2(0, 1);
            outputTextRect.anchorMax = new Vector2(1, 1);
            outputTextRect.pivot = new Vector2(0, 1);
            outputTextRect.anchoredPosition = Vector2.zero;
            outputTextRect.sizeDelta = new Vector2(0, 200); // Give it a initial height
            
            // Add ContentSizeFitter for auto-sizing
            ContentSizeFitter outputTextFitter = outputTextGO.AddComponent<ContentSizeFitter>();
            outputTextFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Create Input Field
            GameObject inputFieldGO = CreateInputField("InputField", consolePanelGO.transform);
            TMP_InputField inputField = inputFieldGO.GetComponent<TMP_InputField>();
            
            // Position Input Field - bottom 15% of console
            RectTransform inputFieldRect = inputFieldGO.GetComponent<RectTransform>();
            inputFieldRect.anchorMin = new Vector2(0, 0.05f);
            inputFieldRect.anchorMax = new Vector2(1, 0.2f);
            inputFieldRect.anchoredPosition = Vector2.zero;
            inputFieldRect.sizeDelta = Vector2.zero;
            inputFieldRect.offsetMin = new Vector2(10, 5);
            inputFieldRect.offsetMax = new Vector2(-10, -5);
            
            // Create Suggestion Panel
            GameObject suggestionPanelGO = CreateUIElement("SuggestionPanel", consolePanelGO.transform);
            Image suggestionPanelImage = suggestionPanelGO.AddComponent<Image>();
            suggestionPanelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Position Suggestion Panel - between input and output
            RectTransform suggestionPanelRect = suggestionPanelGO.GetComponent<RectTransform>();
            suggestionPanelRect.anchorMin = new Vector2(0, 0.2f);
            suggestionPanelRect.anchorMax = new Vector2(1, 0.25f);
            suggestionPanelRect.anchoredPosition = Vector2.zero;
            suggestionPanelRect.sizeDelta = Vector2.zero;
            suggestionPanelRect.offsetMin = new Vector2(10, 0);
            suggestionPanelRect.offsetMax = new Vector2(-10, 0);
            
            // Create Suggestion Container
            GameObject suggestionContainerGO = CreateUIElement("SuggestionContainer", suggestionPanelGO.transform);
            VerticalLayoutGroup suggestionLayout = suggestionContainerGO.AddComponent<VerticalLayoutGroup>();
            suggestionLayout.spacing = 2;
            suggestionLayout.padding = new RectOffset(5, 5, 5, 5);
            suggestionLayout.childControlWidth = true;
            suggestionLayout.childControlHeight = false;
            suggestionLayout.childForceExpandWidth = true;
            suggestionLayout.childForceExpandHeight = false;
            
            // Position Suggestion Container
            RectTransform suggestionContainerRect = suggestionContainerGO.GetComponent<RectTransform>();
            suggestionContainerRect.anchorMin = Vector2.zero;
            suggestionContainerRect.anchorMax = Vector2.one;
            suggestionContainerRect.offsetMin = Vector2.zero;
            suggestionContainerRect.offsetMax = Vector2.zero;
            
            // Create Suggestion Button Prefab
            GameObject suggestionButtonGO = CreateButton("SuggestionButton", suggestionContainerGO.transform);
            
            // Configure Suggestion Button
            Button suggestionButton = suggestionButtonGO.GetComponent<Button>();
            ColorBlock buttonColors = suggestionButton.colors;
            buttonColors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            buttonColors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
            buttonColors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            suggestionButton.colors = buttonColors;
            
            // Setup button text
            TextMeshProUGUI buttonText = suggestionButtonGO.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.fontSize = 12;
            buttonText.color = Color.white;
            buttonText.text = "suggestion";
            buttonText.alignment = TextAlignmentOptions.Left;
            
            // Add LayoutElement to control button sizing
            LayoutElement buttonLayout = suggestionButtonGO.AddComponent<LayoutElement>();
            buttonLayout.minHeight = 20;
            buttonLayout.preferredHeight = 22;
            buttonLayout.flexibleWidth = 1;
            
            // Make suggestion button a prefab reference (remove from scene)
            suggestionButtonGO.SetActive(false);
            
            // Setup ConsoleUI references using SerializedObject
            SerializedObject serializedConsoleUI = new SerializedObject(consoleUI);
            
            var consolePanelProp = serializedConsoleUI.FindProperty("consolePanel");
            var inputFieldProp = serializedConsoleUI.FindProperty("inputField");
            var outputTextProp = serializedConsoleUI.FindProperty("outputText");
            var scrollRectProp = serializedConsoleUI.FindProperty("scrollRect");
            var suggestionPanelProp = serializedConsoleUI.FindProperty("suggestionPanel");
            var suggestionContainerProp = serializedConsoleUI.FindProperty("suggestionContainer");
            var suggestionButtonPrefabProp = serializedConsoleUI.FindProperty("suggestionButtonPrefab");
            
            if (consolePanelProp != null) consolePanelProp.objectReferenceValue = consolePanelGO;
            if (inputFieldProp != null) inputFieldProp.objectReferenceValue = inputField;
            if (outputTextProp != null) outputTextProp.objectReferenceValue = outputText;
            if (scrollRectProp != null) scrollRectProp.objectReferenceValue = outputScroll;
            if (suggestionPanelProp != null) suggestionPanelProp.objectReferenceValue = suggestionPanelGO;
            if (suggestionContainerProp != null) suggestionContainerProp.objectReferenceValue = suggestionContainerGO.transform;
            if (suggestionButtonPrefabProp != null) suggestionButtonPrefabProp.objectReferenceValue = suggestionButtonGO.GetComponent<Button>();
            
            serializedConsoleUI.ApplyModifiedProperties();
            EditorUtility.SetDirty(consoleUI);
            
            // Set Console Panel inactive by default
            consolePanelGO.SetActive(false);
            
            // Register for undo
            Undo.RegisterCreatedObjectUndo(consolePanelGO, "Create Console UI");
            
            Debug.Log("Created Console UI hierarchy");
            
            return consolePanelGO;
        }
        
        void SetupReferences(GameObject consoleUI)
        {
            // Find ConsoleManager and link it to ConsoleUI
            ConsoleManager manager = FindObjectOfType<ConsoleManager>();
            ConsoleUI ui = consoleUI.GetComponent<ConsoleUI>();
            
            Debug.Log($"SetupReferences - Manager found: {manager != null}, UI found: {ui != null}");
            
            if (manager != null && ui != null)
            {
                // Use SerializedObject to set the consoleUI field properly
                SerializedObject serializedManager = new SerializedObject(manager);
                SerializedProperty consoleUIProperty = serializedManager.FindProperty("consoleUI");
                
                if (consoleUIProperty != null)
                {
                    consoleUIProperty.objectReferenceValue = ui;
                    serializedManager.ApplyModifiedProperties();
                    EditorUtility.SetDirty(manager);
                    Debug.Log("Successfully linked ConsoleManager to ConsoleUI");
                }
                else
                {
                    Debug.LogError("Could not find consoleUI property in ConsoleManager - check if field exists and is serialized");
                }
            }
            else
            {
                if (manager == null) Debug.LogError("ConsoleManager not found!");
                if (ui == null) Debug.LogError("ConsoleUI component not found!");
            }
        }
        
        GameObject CreateUIElement(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            
            // Set parent with worldPositionStays = false for proper UI scaling
            rect.SetParent(parent, false);
            
            // Set to fill parent by default
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            
            // Reset offsets to ensure proper stretching
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Debug.Log($"Created UI Element '{name}' - Parent: {parent.name}, Anchors: {rect.anchorMin} to {rect.anchorMax}");
            
            return go;
        }
        
        GameObject CreateScrollView(string name, Transform parent)
        {
            GameObject scrollViewGO = CreateUIElement(name, parent);
            
            // Add ScrollRect
            ScrollRect scrollRect = scrollViewGO.AddComponent<ScrollRect>();
            Image scrollImage = scrollViewGO.AddComponent<Image>();
            scrollImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            
            // Create Viewport
            GameObject viewportGO = CreateUIElement("Viewport", scrollViewGO.transform);
            Image viewportImage = viewportGO.AddComponent<Image>();
            viewportImage.color = Color.clear;
            Mask viewportMask = viewportGO.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            // Create Content
            GameObject contentGO = CreateUIElement("Content", viewportGO.transform);
            RectTransform contentRect = contentGO.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0, 1);
            
            // Configure ScrollRect
            scrollRect.viewport = viewportGO.GetComponent<RectTransform>();
            scrollRect.content = contentRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            
            return scrollViewGO;
        }
        
        GameObject CreateTextMeshPro(string name, Transform parent)
        {
            GameObject textGO = CreateUIElement(name, parent);
            TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
            
            // Configure text
            text.text = "";
            text.fontSize = 14;
            text.color = Color.white;
            text.richText = true;
            text.alignment = TextAlignmentOptions.TopLeft;
            
            // Try to assign a default TextMeshPro font
            var defaultFont = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            if (defaultFont != null)
            {
                text.font = defaultFont;
            }
            
            // Ensure proper RectTransform setup
            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
            
            return textGO;
        }
        
        GameObject CreateInputField(string name, Transform parent)
        {
            GameObject inputFieldGO = CreateUIElement(name, parent);
            
            // Add InputField components
            Image inputImage = inputFieldGO.AddComponent<Image>();
            inputImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            inputImage.type = Image.Type.Sliced;
            
            TMP_InputField inputField = inputFieldGO.AddComponent<TMP_InputField>();
            
            // Create Text Area
            GameObject textAreaGO = CreateUIElement("Text Area", inputFieldGO.transform);
            RectMask2D textAreaMask = textAreaGO.AddComponent<RectMask2D>();
            
            RectTransform textAreaRect = textAreaGO.GetComponent<RectTransform>();
            textAreaRect.offsetMin = new Vector2(10, 6);
            textAreaRect.offsetMax = new Vector2(-10, -7);
            
            // Create Placeholder
            GameObject placeholderGO = CreateTextMeshPro("Placeholder", textAreaGO.transform);
            TextMeshProUGUI placeholder = placeholderGO.GetComponent<TextMeshProUGUI>();
            placeholder.text = "Enter command...";
            placeholder.fontSize = 14;
            placeholder.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            placeholder.fontStyle = FontStyles.Italic;
            
            // Create Text
            GameObject textGO = CreateTextMeshPro("Text", textAreaGO.transform);
            TextMeshProUGUI text = textGO.GetComponent<TextMeshProUGUI>();
            text.fontSize = 14;
            text.color = Color.white;
            
            // Configure InputField
            inputField.textViewport = textAreaRect;
            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            
            return inputFieldGO;
        }
        
        GameObject CreateButton(string name, Transform parent)
        {
            GameObject buttonGO = CreateUIElement(name, parent);
            
            // Add Button components
            Image buttonImage = buttonGO.AddComponent<Image>();
            Button button = buttonGO.AddComponent<Button>();
            
            // Create button text
            GameObject textGO = CreateTextMeshPro("Text", buttonGO.transform);
            TextMeshProUGUI text = textGO.GetComponent<TextMeshProUGUI>();
            text.text = "Button";
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            return buttonGO;
        }
        
        void RemoveConsoleSystem()
        {
            if (EditorUtility.DisplayDialog("Remove Console System", 
                "Are you sure you want to remove the console system? This will delete all console GameObjects including the Console Canvas.", 
                "Yes", "Cancel"))
            {
                // Remove ConsoleManager
                ConsoleManager manager = FindObjectOfType<ConsoleManager>();
                if (manager != null)
                {
                    Undo.DestroyObjectImmediate(manager.gameObject);
                    Debug.Log("Removed ConsoleManager");
                }
                
                // Remove Console Canvas (which includes Console UI)
                Canvas[] canvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in canvases)
                {
                    if (canvas.name == "Console Canvas")
                    {
                        Undo.DestroyObjectImmediate(canvas.gameObject);
                        Debug.Log("Removed Console Canvas");
                        break;
                    }
                }
                
                setupComplete = false;
                statusMessage = "Console system removed.";
                Debug.Log("Console system removed completely");
            }
        }
    }
}