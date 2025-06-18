using System.Linq;

namespace GangstaBean.Console
{
    /// <summary>
    /// Help command to list available commands
    /// </summary>
    [ConsoleCommand]
    public class HelpCommand : IConsoleCommand
    {
        public string Name => "help";
        public string Description => "Show available commands or detailed help for a specific command";
        public string[] Aliases => new[] { "?", "h" };
        public string Usage => "help [command_name]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length <= 1;
        }
        
        public string Execute(string[] parameters)
        {
            var manager = ConsoleManager.I;
            
            if (parameters.Length == 0)
            {
                // List all commands
                var commands = manager.GetType()
                    .GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(manager) as System.Collections.Generic.Dictionary<string, IConsoleCommand>;
                
                if (commands == null || commands.Count == 0)
                {
                    return "No commands available.";
                }
                
                var result = "Available commands:\n";
                foreach (var command in commands.Values.OrderBy(c => c.Name))
                {
                    result += $"  {command.Name} - {command.Description}\n";
                }
                result += "\nType 'help <command>' for detailed usage.";
                return result;
            }
            else
            {
                // Show help for specific command
                string commandName = parameters[0].ToLower();
                
                // Try to find the command
                var commands = manager.GetType()
                    .GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(manager) as System.Collections.Generic.Dictionary<string, IConsoleCommand>;
                
                if (commands != null && commands.TryGetValue(commandName, out IConsoleCommand command))
                {
                    var result = $"Command: {command.Name}\n";
                    result += $"Description: {command.Description}\n";
                    result += $"Usage: {command.Usage}";
                    
                    if (command.Aliases != null && command.Aliases.Length > 0)
                    {
                        result += $"\nAliases: {string.Join(", ", command.Aliases)}";
                    }
                    
                    return result;
                }
                else
                {
                    return $"Command '{commandName}' not found.";
                }
            }
        }
    }
    
    /// <summary>
    /// Clear command to clear console output
    /// </summary>
    [ConsoleCommand]
    public class ClearCommand : IConsoleCommand
    {
        public string Name => "clear";
        public string Description => "Clear the console output";
        public string[] Aliases => new[] { "cls", "c" };
        public string Usage => "clear";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            var manager = ConsoleManager.I;
            
            // Clear the output messages
            var outputMessages = manager.GetType()
                .GetField("outputMessages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(manager) as System.Collections.Generic.List<ConsoleMessage>;
            
            outputMessages?.Clear();
            
            return "Console cleared.";
        }
    }
    
    /// <summary>
    /// Test command to verify console functionality
    /// </summary>
    [ConsoleCommand]
    public class TestCommand : IConsoleCommand
    {
        public string Name => "test";
        public string Description => "Test console functionality";
        public string[] Aliases => new[] { "t" };
        public string Usage => "test [message]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return true;
        }
        
        public string Execute(string[] parameters)
        {
            var manager = ConsoleManager.I;
            
            // Test different message types
            manager.AddMessage("Info: Console working!", ConsoleMessageType.Info);
            manager.AddMessage("Success: Commands executing!", ConsoleMessageType.Success);
            manager.AddMessage("Warning: This is a test warning", ConsoleMessageType.Warning);
            manager.AddMessage("Error: This is a test error", ConsoleMessageType.Error);
            
            if (parameters.Length == 0)
            {
                return "Console test complete - check output above!";
            }
            else
            {
                string message = string.Join(" ", parameters);
                return $"Echo: {message}";
            }
        }
    }
    
    /// <summary>
    /// Command to toggle input blocking when console is open
    /// </summary>
    [ConsoleCommand]
    public class InputBlockingCommand : IConsoleCommand
    {
        public string Name => "inputblocking";
        public string Description => "Toggle whether game input is disabled when console is open";
        public string[] Aliases => new[] { "input", "block" };
        public string Usage => "inputblocking [on/off]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length <= 1;
        }
        
        public string Execute(string[] parameters)
        {
            var manager = ConsoleManager.I;
            var disableField = manager.GetType().GetField("disableGameInputWhenOpen", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (disableField == null)
                return "Could not access input blocking setting";
            
            bool currentValue = (bool)disableField.GetValue(manager);
            
            if (parameters.Length == 0)
            {
                return $"Input blocking is currently: {(currentValue ? "ON" : "OFF")}";
            }
            
            string param = parameters[0].ToLower();
            bool newValue = currentValue;
            
            if (param == "on" || param == "true" || param == "1")
            {
                newValue = true;
            }
            else if (param == "off" || param == "false" || param == "0")
            {
                newValue = false;
            }
            else
            {
                return "Invalid parameter. Use 'on' or 'off'";
            }
            
            disableField.SetValue(manager, newValue);
            return $"Input blocking set to: {(newValue ? "ON" : "OFF")}";
        }
    }
    
    /// <summary>
    /// Command to validate console system setup
    /// </summary>
    [ConsoleCommand]
    public class ValidateConsoleCommand : IConsoleCommand
    {
        public string Name => "validate";
        public string Description => "Check console system setup and connections";
        public string[] Aliases => new[] { "check", "status" };
        public string Usage => "validate";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            var manager = ConsoleManager.I;
            var result = "Console System Validation:\n";
            
            result += $"Manager Instance: {(manager != null ? "✓ Found" : "✗ Missing")}\n";
            
            if (manager != null)
            {
                result += $"Console Open: {manager.IsConsoleOpen}\n";
                var commandsField = manager.GetType().GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var commandCount = commandsField?.GetValue(manager) is System.Collections.Generic.Dictionary<string, IConsoleCommand> commands ? commands.Count : 0;
                result += $"Command Count: {commandCount}\n";
                
                var uiField = manager.GetType().GetField("consoleUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var ui = uiField?.GetValue(manager);
                result += $"UI Connected: {(ui != null ? "✓ Yes" : "✗ No")}\n";
            }
            
            return result;
        }
    }
}