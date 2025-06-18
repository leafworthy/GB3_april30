using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GangstaBean.Console
{
    public class CommandBrowser : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform tabContainer;
        [SerializeField] private Transform commandListContainer;
        [SerializeField] private TextMeshProUGUI helpDisplay;
        [SerializeField] private ScrollRect commandScrollRect;
        
        [Header("Prefabs")]
        [SerializeField] private Button tabButtonPrefab;
        [SerializeField] private Button commandButtonPrefab;
        
        [Header("Visual Settings")]
        [SerializeField] private Color activeTabColor = Color.white;
        [SerializeField] private Color inactiveTabColor = Color.gray;
        [SerializeField] private Color commandHoverColor = Color.yellow;
        
        private Dictionary<string, List<string>> commandCategories = new Dictionary<string, List<string>>();
        private List<Button> tabButtons = new List<Button>();
        private List<Button> commandButtons = new List<Button>();
        private string currentCategory = "";
        private ConsoleManager consoleManager;
        
        private void Start()
        {
            consoleManager = ConsoleManager.I;
            if (consoleManager == null)
                consoleManager = FindObjectOfType<ConsoleManager>();
            
            InitializeCommandCategories();
            CreateTabs();
            SelectCategory("Basic");
        }
        
        private void InitializeCommandCategories()
        {
            commandCategories["Basic"] = new List<string> { "help", "clear", "test", "inputblocking", "validate" };
            commandCategories["Player"] = new List<string> { "respawn", "heal", "kill", "teleport", "god", "players", "sethealth" };
            commandCategories["Ammo"] = new List<string> { "addammo", "setammo", "ammo", "giveammo", "removeammo" };
            commandCategories["Enemy"] = new List<string> { "spawn", "killall", "spawnwave", "stopwave", "enemycount" };
            commandCategories["Game"] = new List<string> { "scene", "pause", "timescale", "restart", "quit" };
            commandCategories["Utility"] = new List<string> { "batch", "macro", "save", "load", "settings" };
        }
        
        private void CreateTabs()
        {
            if (tabContainer == null || tabButtonPrefab == null) return;
            
            foreach (var category in commandCategories.Keys)
            {
                var tabButton = Instantiate(tabButtonPrefab, tabContainer);
                var tabText = tabButton.GetComponentInChildren<TextMeshProUGUI>();
                if (tabText != null)
                    tabText.text = category;
                
                string categoryName = category;
                tabButton.onClick.AddListener(() => SelectCategory(categoryName));
                
                tabButtons.Add(tabButton);
            }
        }
        
        public void SelectCategory(string category)
        {
            if (!commandCategories.ContainsKey(category)) return;
            
            currentCategory = category;
            UpdateTabVisuals();
            PopulateCommandList();
            ClearHelpDisplay();
        }
        
        private void UpdateTabVisuals()
        {
            for (int i = 0; i < tabButtons.Count; i++)
            {
                var button = tabButtons[i];
                var text = button.GetComponentInChildren<TextMeshProUGUI>();
                if (text == null) continue;
                
                bool isActive = text.text == currentCategory;
                text.color = isActive ? activeTabColor : inactiveTabColor;
                
                var image = button.GetComponent<Image>();
                if (image != null)
                    image.color = isActive ? activeTabColor : inactiveTabColor;
            }
        }
        
        private void PopulateCommandList()
        {
            ClearCommandList();
            
            if (!commandCategories.ContainsKey(currentCategory)) return;
            
            var commands = commandCategories[currentCategory];
            foreach (var command in commands)
            {
                CreateCommandButton(command);
            }
        }
        
        private void ClearCommandList()
        {
            foreach (var button in commandButtons)
            {
                if (button != null)
                    DestroyImmediate(button.gameObject);
            }
            commandButtons.Clear();
        }
        
        private void CreateCommandButton(string command)
        {
            if (commandListContainer == null || commandButtonPrefab == null) return;
            
            var commandButton = Instantiate(commandButtonPrefab, commandListContainer);
            var commandText = commandButton.GetComponentInChildren<TextMeshProUGUI>();
            if (commandText != null)
                commandText.text = command;
            
            commandButton.onClick.AddListener(() => OnCommandSelected(command));
            
            var eventTrigger = commandButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = commandButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            
            var enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
            enterEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => ShowCommandHelp(command));
            eventTrigger.triggers.Add(enterEntry);
            
            commandButtons.Add(commandButton);
        }
        
        private void OnCommandSelected(string command)
        {
            var consoleUI = FindObjectOfType<ConsoleUI>();
            if (consoleUI != null)
            {
                var inputField = consoleUI.GetComponentInChildren<TMP_InputField>();
                if (inputField != null)
                {
                    inputField.text = command + " ";
                    inputField.caretPosition = inputField.text.Length;
                    inputField.ActivateInputField();
                }
            }
        }
        
        private void ShowCommandHelp(string command)
        {
            if (helpDisplay == null) return;
            
            string helpText = GetCommandHelp(command);
            helpDisplay.text = helpText;
        }
        
        private void ClearHelpDisplay()
        {
            if (helpDisplay != null)
                helpDisplay.text = "Hover over a command to see help information.";
        }
        
        private string GetCommandHelp(string command)
        {
            return command switch
            {
                "help" => "<b>help</b> [command]\nShows list of commands or detailed help for specific command",
                "clear" => "<b>clear</b>\nClears the console output",
                "test" => "<b>test</b>\nRuns console system test",
                "respawn" => "<b>respawn</b> <player> [x y] [fromsky]\nRespawns specified player at position",
                "heal" => "<b>heal</b> <player> [amount]\nHeals player by amount (default: full)",
                "kill" => "<b>kill</b> <player>\nKills specified player instantly",
                "teleport" => "<b>teleport</b> <player> <x> <y>\nTeleports player to coordinates",
                "god" => "<b>god</b> <player> [on/off]\nToggles invincibility for player",
                "addammo" => "<b>addammo</b> <type> <amount>\nAdds ammo of specified type",
                "spawn" => "<b>spawn</b> <enemy> [x y]\nSpawns enemy at position",
                "scene" => "<b>scene</b> <scenename>\nLoads specified scene",
                "pause" => "<b>pause</b> [on/off]\nPauses or unpauses the game",
                "timescale" => "<b>timescale</b> <value>\nSets game time scale (0-10)",
                _ => $"<b>{command}</b>\nNo detailed help available for this command."
            };
        }
        
        public void RefreshCommands()
        {
            InitializeCommandCategories();
            PopulateCommandList();
        }
        
        public void FilterCommands(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                PopulateCommandList();
                return;
            }
            
            ClearCommandList();
            
            var allCommands = commandCategories.Values.SelectMany(x => x).Distinct();
            var filteredCommands = allCommands.Where(cmd => cmd.ToLower().Contains(searchTerm.ToLower()));
            
            foreach (var command in filteredCommands)
            {
                CreateCommandButton(command);
            }
        }
    }
}