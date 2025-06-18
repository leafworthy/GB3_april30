using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace GangstaBean.Console
{
    /// <summary>
    /// UI component for the debug console
    /// </summary>
    public class ConsoleUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject consolePanel;
        [SerializeField] private RectTransform headerBar;
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform sidePanel;
        [SerializeField] private RectTransform statusBar;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI outputText;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject suggestionPanel;
        [SerializeField] private Transform suggestionContainer;
        [SerializeField] private Button suggestionButtonPrefab;
        [SerializeField] private TextMeshProUGUI consoleTitle;
        [SerializeField] private Button minimizeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI memoryText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        [Header("Component References")]
        [SerializeField] private ConsolePanelManager panelManager;
        [SerializeField] private CommandBrowser commandBrowser;
        
        [Header("Visual Settings")]
        [SerializeField] private Color infoColor = Color.white;
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;
        [SerializeField] private Color inputColor = Color.cyan;
        
        [Header("Behavior")]
        [SerializeField] private bool autoComplete = true;
        [SerializeField] private bool showTimestamps = false;
        [SerializeField] private int maxSuggestions = 5;
        
        
        private ConsoleManager consoleManager;
        private List<Button> suggestionButtons = new List<Button>();
        private string currentInput = "";
        private bool isShowingSuggestions = false;
        private bool isMinimized = false;
        private float lastFpsUpdate = 0f;
        private int frameCount = 0;
        private float currentFps = 0f;
        
        // Fallback input actions for when no InputActionReference is assigned
        private InputAction fallbackTabAction;
        private InputAction fallbackUpArrowAction;
        private InputAction fallbackDownArrowAction;
        private InputAction fallbackEscapeAction;
        
        private void Awake()
        {
            // Ensure console starts hidden
            if (consolePanel != null)
                consolePanel.SetActive(false);
            
            if (suggestionPanel != null)
                suggestionPanel.SetActive(false);
        }
        
        private void Start()
        {
            ValidateUIReferences();
            ConnectToConsoleManager();
            SetupInputField();
            CreateInitialSuggestionButtons();
            SetupInputActions();
            SetupHeaderButtons();
            InitializeStatusBar();
            
            if (consoleManager != null)
            {
                consoleManager.AddMessage("Debug Console initialized. Type 'help' for available commands.", ConsoleMessageType.Info);
                consoleManager.AddMessage("Toggle with ~ (backtick) or C key", ConsoleMessageType.Info);
            }
        }
        
        private void ValidateUIReferences()
        {
            if (consolePanel == null)
                Debug.LogError("ConsoleUI: consolePanel reference is missing!");
            
            if (inputField == null)
                Debug.LogError("ConsoleUI: inputField reference is missing!");
            
            if (outputText == null)
            {
                Debug.LogError("ConsoleUI: outputText reference is missing!");
                // Try to find it automatically
                outputText = GetComponentInChildren<TextMeshProUGUI>();
                if (outputText != null)
                    Debug.Log("ConsoleUI: Found outputText automatically");
            }
            
            if (scrollRect == null)
            {
                Debug.LogWarning("ConsoleUI: scrollRect reference is missing - console may not scroll properly");
                // Try to find it automatically
                scrollRect = GetComponentInChildren<ScrollRect>();
                if (scrollRect != null)
                    Debug.Log("ConsoleUI: Found scrollRect automatically");
            }
        }
        
        private void ConnectToConsoleManager()
        {
            consoleManager = ConsoleManager.I;
            
            if (consoleManager == null)
            {
                consoleManager = FindObjectOfType<ConsoleManager>();
            }
            
            if (consoleManager != null)
            {
                consoleManager.OnMessageAdded += OnMessageAdded;
                consoleManager.OnConsoleToggled += HandleConsoleToggle;
                consoleManager.SetConsoleUI(this);
                Debug.Log("ConsoleUI: Successfully connected to ConsoleManager");
            }
            else
            {
                Debug.LogError("ConsoleUI: Failed to find ConsoleManager!");
                StartCoroutine(RetryConnectionAfterFrame());
            }
        }
        
        private System.Collections.IEnumerator RetryConnectionAfterFrame()
        {
            yield return null;
            
            consoleManager = ConsoleManager.I;
            if (consoleManager == null)
            {
                consoleManager = FindObjectOfType<ConsoleManager>();
            }
            
            if (consoleManager != null)
            {
                consoleManager.OnMessageAdded += OnMessageAdded;
                consoleManager.OnConsoleToggled += HandleConsoleToggle;
                consoleManager.SetConsoleUI(this);
                
                consoleManager.AddMessage("Debug Console initialized. Type 'help' for available commands.", ConsoleMessageType.Info);
                consoleManager.AddMessage("Toggle with ~ (backtick) or F1 key", ConsoleMessageType.Info);
                Debug.Log("ConsoleUI: Retry connection successful");
            }
            else
            {
                Debug.LogError("ConsoleUI: Retry connection failed - ConsoleManager still not found");
            }
        }
        
        private void SetupInputActions()
        {
            if (fallbackTabAction == null)
            {
                fallbackTabAction = new InputAction("ConsoleTab", InputActionType.Button, "<Keyboard>/tab");
                fallbackTabAction.performed += OnTabPressed;
                fallbackTabAction.Enable();
            }
            
            if (fallbackUpArrowAction == null)
            {
                fallbackUpArrowAction = new InputAction("ConsoleUpArrow", InputActionType.Button, "<Keyboard>/upArrow");
                fallbackUpArrowAction.performed += OnUpArrowPressed;
                fallbackUpArrowAction.Enable();
            }
            
            if (fallbackDownArrowAction == null)
            {
                fallbackDownArrowAction = new InputAction("ConsoleDownArrow", InputActionType.Button, "<Keyboard>/downArrow");
                fallbackDownArrowAction.performed += OnDownArrowPressed;
                fallbackDownArrowAction.Enable();
            }
            
            if (fallbackEscapeAction == null)
            {
                fallbackEscapeAction = new InputAction("ConsoleEscape", InputActionType.Button, "<Keyboard>/escape");
                fallbackEscapeAction.performed += OnEscapePressed;
                fallbackEscapeAction.Enable();
            }
        }
        
        private void OnDestroy()
        {
            if (consoleManager != null)
            {
                consoleManager.OnConsoleToggled -= HandleConsoleToggle;
                consoleManager.OnMessageAdded -= OnMessageAdded;
            }
            
            CleanupInputActions();
        }
        
        private void CleanupInputActions()
        {
            if (fallbackTabAction != null)
            {
                fallbackTabAction.performed -= OnTabPressed;
                fallbackTabAction.Disable();
                fallbackTabAction.Dispose();
                fallbackTabAction = null;
            }
            
            if (fallbackUpArrowAction != null)
            {
                fallbackUpArrowAction.performed -= OnUpArrowPressed;
                fallbackUpArrowAction.Disable();
                fallbackUpArrowAction.Dispose();
                fallbackUpArrowAction = null;
            }
            
            if (fallbackDownArrowAction != null)
            {
                fallbackDownArrowAction.performed -= OnDownArrowPressed;
                fallbackDownArrowAction.Disable();
                fallbackDownArrowAction.Dispose();
                fallbackDownArrowAction = null;
            }
            
            if (fallbackEscapeAction != null)
            {
                fallbackEscapeAction.performed -= OnEscapePressed;
                fallbackEscapeAction.Disable();
                fallbackEscapeAction.Dispose();
                fallbackEscapeAction = null;
            }
        }
        
        private void Update()
        {
            // Update suggestions as user types
            if (autoComplete && inputField != null && inputField.text != currentInput)
            {
                currentInput = inputField.text;
                UpdateSuggestions();
            }
            
            // Update status bar
            UpdateStatusBar();
        }
        
        private void OnTabPressed(InputAction.CallbackContext context)
        {
            if (consoleManager == null || !consoleManager.IsConsoleOpen || !autoComplete || inputField == null || !inputField.isFocused)
                return;
                
            HandleTabCompletion();
        }
        
        private void OnUpArrowPressed(InputAction.CallbackContext context)
        {
            if (consoleManager == null || !consoleManager.IsConsoleOpen || inputField == null || !inputField.isFocused)
                return;
                
            string historyCommand = consoleManager.NavigateHistory(1);
            if (!string.IsNullOrEmpty(historyCommand))
            {
                inputField.text = historyCommand;
                inputField.caretPosition = inputField.text.Length;
            }
        }
        
        private void OnDownArrowPressed(InputAction.CallbackContext context)
        {
            if (consoleManager == null || !consoleManager.IsConsoleOpen || inputField == null || !inputField.isFocused)
                return;
                
            string historyCommand = consoleManager.NavigateHistory(-1);
            inputField.text = historyCommand;
            inputField.caretPosition = inputField.text.Length;
        }
        
        private void OnEscapePressed(InputAction.CallbackContext context)
        {
            if (consoleManager == null || !consoleManager.IsConsoleOpen)
                return;
                
            consoleManager.ToggleConsole();
        }
        
        public void HandleConsoleToggle(bool isOpen)
        {
            if (consolePanel != null)
            {
                consolePanel.SetActive(isOpen);
            }
            
            if (isOpen)
            {
                // Show cursor and unlock it
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                if (inputField != null)
                {
                    inputField.Select();
                    inputField.ActivateInputField();
                }
                
                ScrollToBottom();
            }
            else
            {
                // Hide cursor and lock it (restore game state)
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                
                HideSuggestions();
            }
        }
        
        private void OnMessageAdded(ConsoleMessage message)
        {
            if (outputText != null)
            {
                UpdateOutputText();
                ScrollToBottom();
            }
        }
        
        private void UpdateOutputText()
        {
            if (outputText == null || consoleManager == null)
            {
                Debug.LogWarning("UpdateOutputText: outputText or consoleManager is null");
                return;
            }
            
            var messages = consoleManager.OutputMessages;
            if (messages == null || messages.Count == 0)
            {
                outputText.text = "No messages yet. Type 'help' for commands.";
                return;
            }
            
            string output = "";
            foreach (var message in messages)
            {
                string colorHex = GetColorHex(message.Type);
                string timestamp = showTimestamps ? $"[{message.Timestamp:F2}] " : "";
                output += $"<color={colorHex}>{timestamp}{message.Text}</color>\n";
            }
            
            outputText.text = output;
            
            // Force layout rebuild
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(outputText.rectTransform);
        }
        
        private string GetColorHex(ConsoleMessageType type)
        {
            Color color = type switch
            {
                ConsoleMessageType.Success => successColor,
                ConsoleMessageType.Warning => warningColor,
                ConsoleMessageType.Error => errorColor,
                ConsoleMessageType.Input => inputColor,
                _ => infoColor
            };
            
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }
        
        private void SetupInputField()
        {
            if (inputField != null)
            {
                inputField.onSubmit.AddListener(OnInputSubmit);
                inputField.onValueChanged.AddListener(OnInputValueChanged);
            }
        }
        
        private void OnInputSubmit(string input)
        {
            Debug.Log($"ConsoleUI: OnInputSubmit called with input: '{input}'");
            
            // Check if Shift is held down
            bool shiftHeld = UnityEngine.InputSystem.Keyboard.current?.shiftKey.isPressed ?? false;
            
            if (!string.IsNullOrWhiteSpace(input))
            {
                if (consoleManager != null)
                {
                    Debug.Log($"ConsoleUI: Executing command: {input} (Shift held: {shiftHeld})");
                    
                    if (shiftHeld)
                    {
                        // Shift+Enter: Execute command in unpaused state
                        Debug.Log("ConsoleUI: Shift+Enter detected, will execute in unpaused state");
                        
                        inputField.text = "";
                        HideSuggestions();
                        
                        // Force unpause for live execution
                        var pauseManager = __SCRIPTS.PauseManager.I;
                        bool wasGamePaused = pauseManager?.IsPaused ?? false;
                        
                        if (wasGamePaused && pauseManager != null)
                        {
                            pauseManager.Unpause();
                        }
                        
                        // Close console
                        consoleManager.ToggleConsole();
                        
                        // Execute command in unpaused state after brief delay
                        StartCoroutine(ExecuteCommandInLiveState(input, wasGamePaused));
                        
                        return; // Don't reactivate input field since console is closing
                    }
                    else
                    {
                        // Normal enter: Execute in current (paused) state
                        consoleManager.ExecuteCommand(input);
                        inputField.text = "";
                        HideSuggestions();
                        
                        // Force update output immediately
                        UpdateOutputText();
                        ScrollToBottom();
                    }
                }
                else
                {
                    Debug.LogError("ConsoleUI: ConsoleManager is null, cannot execute command!");
                }
            }
            
            // Keep input field focused (only if console is still open)
            if (consoleManager?.IsConsoleOpen == true)
            {
                inputField.ActivateInputField();
            }
        }
        
        private void OnInputValueChanged(string input)
        {
            currentInput = input;
            if (autoComplete)
            {
                UpdateSuggestions();
            }
        }
        
        private System.Collections.IEnumerator ExecuteCommandInLiveState(string command, bool shouldRepauseAfter)
        {
            // Wait a couple frames to ensure unpause and console close take effect
            yield return null;
            yield return null;
            
            if (consoleManager != null)
            {
                Debug.Log($"ConsoleUI: Executing command in live state: {command}");
                consoleManager.ExecuteCommand(command);
                
                // Optionally re-pause if game was paused before shift+enter
                // (This gives user control - they can choose whether to stay unpaused)
                // if (shouldRepauseAfter)
                // {
                //     yield return new WaitForSeconds(0.1f);
                //     var pauseManager = __SCRIPTS.PauseManager.I;
                //     if (pauseManager != null)
                //     {
                //         pauseManager.Pause(Players.I?.AllJoinedPlayers?.FirstOrDefault());
                //     }
                // }
            }
        }
        
        private void HandleTabCompletion()
        {
            if (string.IsNullOrEmpty(currentInput))
                return;
            
            var suggestions = consoleManager?.GetCommandSuggestions(currentInput);
            if (suggestions != null && suggestions.Count > 0)
            {
                inputField.text = suggestions[0];
                inputField.caretPosition = inputField.text.Length;
                HideSuggestions();
            }
        }
        
        private void UpdateSuggestions()
        {
            if (string.IsNullOrEmpty(currentInput))
            {
                HideSuggestions();
                return;
            }
            
            var suggestions = consoleManager?.GetCommandSuggestions(currentInput);
            if (suggestions == null || suggestions.Count == 0)
            {
                HideSuggestions();
                return;
            }
            
            ShowSuggestions(suggestions.Take(maxSuggestions).ToList());
        }
        
        private void ShowSuggestions(List<string> suggestions)
        {
            if (suggestionPanel == null || suggestionContainer == null)
                return;
            
            // Ensure we have enough buttons
            while (suggestionButtons.Count < suggestions.Count)
            {
                CreateSuggestionButton();
            }
            
            // Hide all buttons first
            foreach (var button in suggestionButtons)
            {
                button.gameObject.SetActive(false);
            }
            
            // Show and set up suggestion buttons
            for (int i = 0; i < suggestions.Count; i++)
            {
                var button = suggestionButtons[i];
                button.gameObject.SetActive(true);
                
                var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = suggestions[i];
                }
                
                // Set up click handler
                string suggestion = suggestions[i];
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSuggestion(suggestion));
            }
            
            suggestionPanel.SetActive(true);
            isShowingSuggestions = true;
        }
        
        private void HideSuggestions()
        {
            if (suggestionPanel != null)
            {
                suggestionPanel.SetActive(false);
            }
            isShowingSuggestions = false;
        }
        
        private void SelectSuggestion(string suggestion)
        {
            inputField.text = suggestion + " ";
            inputField.caretPosition = inputField.text.Length;
            HideSuggestions();
            inputField.ActivateInputField();
        }
        
        private void CreateInitialSuggestionButtons()
        {
            // Create initial suggestion buttons
            for (int i = 0; i < maxSuggestions; i++)
            {
                CreateSuggestionButton();
            }
        }
        
        private void CreateSuggestionButton()
        {
            if (suggestionButtonPrefab == null || suggestionContainer == null)
                return;
            
            var button = Instantiate(suggestionButtonPrefab, suggestionContainer);
            button.gameObject.SetActive(false);
            
            // Ensure the button has proper layout components
            if (button.GetComponent<LayoutElement>() == null)
            {
                var layout = button.gameObject.AddComponent<LayoutElement>();
                layout.minHeight = 20;
                layout.preferredHeight = 22;
                layout.flexibleWidth = 1;
            }
            
            suggestionButtons.Add(button);
        }
        
        private void ScrollToBottom()
        {
            if (scrollRect != null)
            {
                try
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
                catch (System.NullReferenceException)
                {
                    // Ignore TMP material errors during initialization
                    StartCoroutine(DelayedScrollToBottom());
                }
            }
        }
        
        private System.Collections.IEnumerator DelayedScrollToBottom()
        {
            yield return null; // Wait one frame
            yield return new WaitForEndOfFrame(); // Wait for end of frame to ensure TMP is initialized
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
        
        public void ToggleTimestamps()
        {
            showTimestamps = !showTimestamps;
            UpdateOutputText();
        }
        
        private void SetupHeaderButtons()
        {
            if (minimizeButton != null)
                minimizeButton.onClick.AddListener(ToggleMinimize);
            
            if (closeButton != null)
                closeButton.onClick.AddListener(() => consoleManager?.ToggleConsole());
            
            if (consoleTitle != null)
                consoleTitle.text = "Debug Console v2.0";
        }
        
        private void InitializeStatusBar()
        {
            if (fpsText != null)
                fpsText.text = "FPS: 0";
            
            if (memoryText != null)
                memoryText.text = "Memory: 0 MB";
            
            if (timeText != null)
                timeText.text = "Time: 0.00";
        }
        
        private void UpdateStatusBar()
        {
            frameCount++;
            float timeNow = Time.realtimeSinceStartup;
            
            if (timeNow - lastFpsUpdate >= 1.0f)
            {
                currentFps = frameCount / (timeNow - lastFpsUpdate);
                frameCount = 0;
                lastFpsUpdate = timeNow;
                
                if (fpsText != null)
                    fpsText.text = $"FPS: {currentFps:F0}";
            }
            
            if (memoryText != null)
            {
                float memoryMB = System.GC.GetTotalMemory(false) / (1024f * 1024f);
                memoryText.text = $"Memory: {memoryMB:F1} MB";
            }
            
            if (timeText != null)
                timeText.text = $"Time: {Time.time:F2}";
        }
        
        private void ToggleMinimize()
        {
            isMinimized = !isMinimized;
            
            if (mainPanel != null)
                mainPanel.gameObject.SetActive(!isMinimized);
            
            if (sidePanel != null)
                sidePanel.gameObject.SetActive(!isMinimized);
            
            if (statusBar != null)
                statusBar.gameObject.SetActive(!isMinimized);
            
            var minimizeText = minimizeButton?.GetComponentInChildren<TextMeshProUGUI>();
            if (minimizeText != null)
                minimizeText.text = isMinimized ? "+" : "-";
        }
        
        [ContextMenu("Test UI Activation")]
        public void TestActivation()
        {
            if (consolePanel != null)
            {
                bool newState = !consolePanel.activeSelf;
                consolePanel.SetActive(newState);
            }
        }
    }
}