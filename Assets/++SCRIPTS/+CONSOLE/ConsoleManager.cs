using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using __SCRIPTS;

namespace GangstaBean.Console
{
    /// <summary>
    /// Central manager for the debug console system
    /// </summary>
    public class ConsoleManager : Singleton<ConsoleManager>
    {
        [Header("Console Settings")]
        [SerializeField] private bool enableConsole = true;
        [SerializeField] private int maxHistoryEntries = 100;
        [SerializeField] private int maxOutputLines = 50;
        [SerializeField] private bool disableGameInputWhenOpen = true;
        [SerializeField] private bool onlyAvailableInLevel = true;

        [Header("UI References")]
        [SerializeField] public ConsoleUI consoleUI;

        // Command registry
        private Dictionary<string, IConsoleCommand> commands = new Dictionary<string, IConsoleCommand>();
        private Dictionary<string, string> aliases = new Dictionary<string, string>(); // alias -> command name

        // Console state
        private List<string> commandHistory = new List<string>();
        private List<ConsoleMessage> outputMessages = new List<ConsoleMessage>();
        private int historyIndex = -1;
        private string previousActionMap = ""; // Track what action map was active before console opened

        // Events
        public event Action<bool> OnConsoleToggled;
        public event Action<ConsoleMessage> OnMessageAdded;
        public event Action OnHistoryChanged;

        public bool IsConsoleOpen { get; private set; }
        public IReadOnlyList<ConsoleMessage> OutputMessages => outputMessages.AsReadOnly();
        public IReadOnlyList<string> CommandHistory => commandHistory.AsReadOnly();

        protected override void Awake()
        {
            base.Awake();
            RegisterDefaultCommands();
            AutoRegisterCommands();
            SetupInput();
        }
        
        private void Start()
        {
            if (consoleUI != null)
            {
                SetConsoleUI(consoleUI);
            }
        }

        private void OnEnable()
        {
            SetupInput();
        }

        private void OnDisable()
        {
            CleanupInput();
        }

        private InputAction toggleAction;
        
        private void SetupInput()
        {
            if (toggleAction == null)
            {
                toggleAction = new InputAction("ToggleConsole", InputActionType.Button);
                toggleAction.AddBinding("<Keyboard>/backquote");
                toggleAction.AddBinding("<Keyboard>/c");
                toggleAction.performed += OnToggleConsole;
            }
            
            toggleAction.Enable();
        }
        
        private void CleanupInput()
        {
            if (toggleAction != null)
            {
                toggleAction.performed -= OnToggleConsole;
                toggleAction.Disable();
                toggleAction.Dispose();
                toggleAction = null;
            }
        }

        private void OnToggleConsole(InputAction.CallbackContext context)
        {
            if (!enableConsole || !context.performed)
                return;
                
            // Check if console is only available in level
            if (onlyAvailableInLevel && !IsInGameLevel())
            {
                Debug.Log("Console is only available during gameplay levels");
                return;
            }
            
            // Simple toggle - let console handle pause coordination internally
            ToggleConsole();
        }
        
        /// <summary>
        /// Check if we're currently in a gameplay level (not menu scenes)
        /// </summary>
        private bool IsInGameLevel()
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // List of menu scenes where console should be disabled
            string[] menuScenes = {
                "1_MainMenuScene",
                "2_CharacterSelectScene", 
                "5_RestartLevelScene",
                "GameOverScene",
                "UpgradesScene"
            };
            
            foreach (string menuScene in menuScenes)
            {
                if (currentScene.Contains(menuScene))
                    return false;
            }
            
            return true;
        }

        private bool wasGamePausedBeforeConsole = false;
        
        /// <summary>
        /// Toggle console visibility
        /// </summary>
        public void ToggleConsole()
        {
            IsConsoleOpen = !IsConsoleOpen;
            
            var pauseManager = PauseManager.I;
            
            if (IsConsoleOpen)
            {
                // Opening console
                historyIndex = -1;
                
                // Remember if game was already paused
                wasGamePausedBeforeConsole = pauseManager?.IsPaused ?? false;
                
                // If game isn't paused, pause it for console convenience
                if (pauseManager != null && !pauseManager.IsPaused)
                {
                    var firstPlayer = Players.I?.AllJoinedPlayers?.FirstOrDefault();
                    if (firstPlayer != null)
                    {
                        pauseManager.Pause(firstPlayer);
                        Debug.Log("Console: Auto-paused game for console access");
                    }
                }
                
                HandleInputSwitch(true);
            }
            else
            {
                // Closing console
                HandleInputSwitch(false);
                
                // Only unpause if we auto-paused (game wasn't paused before console opened)
                if (pauseManager != null && pauseManager.IsPaused && !wasGamePausedBeforeConsole)
                {
                    pauseManager.Unpause();
                    Debug.Log("Console: Auto-unpaused game after console close");
                }
                
                wasGamePausedBeforeConsole = false;
            }
            
            OnConsoleToggled?.Invoke(IsConsoleOpen);
            
            if (consoleUI != null)
            {
                consoleUI.HandleConsoleToggle(IsConsoleOpen);
            }
        }
        
        /// <summary>
        /// Manual test method to verify console toggle works
        /// </summary>
        [ContextMenu("Test Console Toggle")]
        public void TestToggle()
        {
            Debug.Log("Manual toggle test triggered");
            ToggleConsole();
        }

        /// <summary>
        /// Set the UI reference - useful for manual setup
        /// </summary>
        public void SetConsoleUI(ConsoleUI ui)
        {
            consoleUI = ui;
            
            OnConsoleToggled -= ui.HandleConsoleToggle;
            OnConsoleToggled += ui.HandleConsoleToggle;
        }

        /// <summary>
        /// Handle switching input between game and console modes
        /// </summary>
        private void HandleInputSwitch(bool consoleOpen)
        {
            if (!disableGameInputWhenOpen)
                return;

            try
            {
                if (consoleOpen)
                {
                    // Don't override action maps if pause manager is handling them
                    var pauseManager = PauseManager.I;
                    if (pauseManager?.IsPaused == true)
                    {
                        // Game is paused, pause manager controls action maps
                        // Console just needs to be visible and functional
                        Debug.Log("Console opened during pause - letting PauseManager handle action maps");
                        return;
                    }
                    
                    // Store current action map from first available player
                    var firstPlayer = Players.I?.AllJoinedPlayers?.FirstOrDefault();
                    if (firstPlayer?.input != null)
                    {
                        previousActionMap = firstPlayer.input.currentActionMap.name;
                        Debug.Log($"Console opened: Storing action map '{previousActionMap}' from player input");
                    }
                    else
                    {
                        // Fallback: try to determine from scene context
                        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                        if (currentScene.Contains("MainMenu") || currentScene.Contains("CharacterSelect"))
                        {
                            previousActionMap = Players.UIActionMap;
                            Debug.Log("Console opened: Detected menu scene, storing UI action map");
                        }
                        else
                        {
                            previousActionMap = Players.PlayerActionMap;
                            Debug.Log("Console opened: Using default PlayerMovement action map");
                        }
                    }
                    
                    // Switch to Console action map (blocks game input)
                    Players.SetActionMaps(Players.ConsoleActionMap);
                    Debug.Log("Switched to Console action map");
                }
                else
                {
                    // Don't restore action maps if pause manager should handle them
                    var pauseManager = PauseManager.I;
                    if (pauseManager?.IsPaused == true)
                    {
                        // Game is still paused, let pause manager control action maps
                        Debug.Log("Console closed during pause - letting PauseManager handle action maps");
                        return;
                    }
                    
                    // Restore previous action map with validation
                    string mapToRestore = !string.IsNullOrEmpty(previousActionMap) ? previousActionMap : Players.PlayerActionMap;
                    Players.SetActionMaps(mapToRestore);
                    Debug.Log($"Console closed: Restored action map '{mapToRestore}'");
                    
                    // Clear the stored action map
                    previousActionMap = "";
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to switch input action maps: {e.Message}");
                // Fallback: try to restore to appropriate action map
                if (!consoleOpen)
                {
                    try
                    {
                        var pauseManager = PauseManager.I;
                        string fallbackMap;
                        
                        if (pauseManager?.IsPaused == true)
                        {
                            fallbackMap = Players.UIActionMap;
                        }
                        else
                        {
                            fallbackMap = IsInGameLevel() ? Players.PlayerActionMap : Players.UIActionMap;
                        }
                        
                        Players.SetActionMaps(fallbackMap);
                        Debug.Log($"Fallback: Restored to {fallbackMap} action map");
                    }
                    catch
                    {
                        Debug.LogError("Failed to restore input to any action map");
                    }
                }
            }
        }

        /// <summary>
        /// Execute a command string
        /// </summary>
        /// <param name="commandLine">Full command line input</param>
        public void ExecuteCommand(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return;

            // Add to history
            AddToHistory(commandLine);

            // Echo the command
            AddMessage($"> {commandLine}", ConsoleMessageType.Input);

            // Parse command
            string[] parts = ParseCommandLine(commandLine);
            if (parts.Length == 0)
                return;

            string commandName = parts[0].ToLower();
            string[] parameters = parts.Skip(1).ToArray();

            // Find command (check aliases first)
            if (aliases.TryGetValue(commandName, out string actualCommandName))
            {
                commandName = actualCommandName;
            }

            if (commands.TryGetValue(commandName, out IConsoleCommand command))
            {
                try
                {
                    if (command.ValidateParameters(parameters))
                    {
                        string result = command.Execute(parameters);
                        if (!string.IsNullOrEmpty(result))
                        {
                            AddMessage(result, ConsoleMessageType.Success);
                        }
                    }
                    else
                    {
                        AddMessage($"Invalid parameters. Usage: {command.Usage}", ConsoleMessageType.Error);
                    }
                }
                catch (Exception e)
                {
                    AddMessage($"Error executing command: {e.Message}", ConsoleMessageType.Error);
                }
            }
            else
            {
                AddMessage($"Unknown command: {commandName}. Type 'help' for available commands.", ConsoleMessageType.Error);
            }
        }

        /// <summary>
        /// Register a command
        /// </summary>
        public void RegisterCommand(IConsoleCommand command)
        {
            commands[command.Name.ToLower()] = command;

            // Register aliases
            if (command.Aliases != null)
            {
                foreach (string alias in command.Aliases)
                {
                    aliases[alias.ToLower()] = command.Name.ToLower();
                }
            }
        }

        /// <summary>
        /// Get command suggestions for autocomplete
        /// </summary>
        public List<string> GetCommandSuggestions(string input)
        {
            if (string.IsNullOrEmpty(input))
                return commands.Keys.ToList();

            string lowerInput = input.ToLower();
            var suggestions = new List<string>();

            // Add exact matches first
            suggestions.AddRange(commands.Keys.Where(cmd => cmd.StartsWith(lowerInput)));
            suggestions.AddRange(aliases.Keys.Where(alias => alias.StartsWith(lowerInput)));

            return suggestions.Distinct().Take(10).ToList();
        }

        /// <summary>
        /// Navigate command history
        /// </summary>
        public string NavigateHistory(int direction)
        {
            if (commandHistory.Count == 0)
                return string.Empty;

            historyIndex += direction;
            historyIndex = Mathf.Clamp(historyIndex, -1, commandHistory.Count - 1);

            return historyIndex >= 0 ? commandHistory[commandHistory.Count - 1 - historyIndex] : string.Empty;
        }

        /// <summary>
        /// Add a message to console output
        /// </summary>
        public void AddMessage(string message, ConsoleMessageType type = ConsoleMessageType.Info)
        {
            var consoleMessage = new ConsoleMessage(message, type, Time.time);
            outputMessages.Add(consoleMessage);

            // Limit output history
            while (outputMessages.Count > maxOutputLines)
            {
                outputMessages.RemoveAt(0);
            }

            OnMessageAdded?.Invoke(consoleMessage);
        }

        private void AddToHistory(string command)
        {
            commandHistory.Add(command);

            // Limit history
            while (commandHistory.Count > maxHistoryEntries)
            {
                commandHistory.RemoveAt(0);
            }

            OnHistoryChanged?.Invoke();
        }

        private string[] ParseCommandLine(string commandLine)
        {
            var parts = new List<string>();
            var currentPart = "";
            bool inQuotes = false;

            for (int i = 0; i < commandLine.Length; i++)
            {
                char c = commandLine[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ' ' && !inQuotes)
                {
                    if (!string.IsNullOrEmpty(currentPart))
                    {
                        parts.Add(currentPart);
                        currentPart = "";
                    }
                }
                else
                {
                    currentPart += c;
                }
            }

            if (!string.IsNullOrEmpty(currentPart))
            {
                parts.Add(currentPart);
            }

            return parts.ToArray();
        }

        private void RegisterDefaultCommands()
        {
            RegisterCommand(new HelpCommand());
            RegisterCommand(new ClearCommand());
        }

        private void AutoRegisterCommands()
        {
            // Find all classes with ConsoleCommandAttribute
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var commandTypes = assembly.GetTypes()
                        .Where(t => typeof(IConsoleCommand).IsAssignableFrom(t) &&
                                   !t.IsInterface &&
                                   !t.IsAbstract &&
                                   t.GetCustomAttribute<ConsoleCommandAttribute>()?.AutoRegister == true);

                    foreach (var type in commandTypes)
                    {
                        try
                        {
                            var command = (IConsoleCommand)Activator.CreateInstance(type);
                            RegisterCommand(command);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Failed to register command {type.Name}: {e.Message}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to scan assembly {assembly.FullName}: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Console message types for color coding
    /// </summary>
    public enum ConsoleMessageType
    {
        Info,
        Success,
        Warning,
        Error,
        Input
    }

    /// <summary>
    /// Console message data structure
    /// </summary>
    [System.Serializable]
    public class ConsoleMessage
    {
        public string Text { get; }
        public ConsoleMessageType Type { get; }
        public float Timestamp { get; }

        public ConsoleMessage(string text, ConsoleMessageType type, float timestamp)
        {
            Text = text;
            Type = type;
            Timestamp = timestamp;
        }
    }
}
