using UnityEngine;
using __SCRIPTS;

namespace GangstaBean.Console
{
    /// <summary>
    /// Toggle no-clip mode for a player (walk through walls)
    /// </summary>
    [ConsoleCommand]
    public class NoClipCommand : IConsoleCommand
    {
        public string Name => "noclip";
        public string Description => "Toggle no-clip mode (walk through walls) for a player";
        public string[] Aliases => new[] { "nc", "fly" };
        public string Usage => "noclip <player> [on/off]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length >= 1 && parameters.Length <= 2;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.SpawnedPlayerGO == null)
                return $"Player {player.name} is not spawned.";
            
            bool noClip;
            
            // Get all colliders on the player
            var colliders = player.SpawnedPlayerGO.GetComponentsInChildren<Collider2D>();
            
            if (parameters.Length >= 2)
            {
                string toggle = parameters[1].ToLower();
                noClip = toggle == "on" || toggle == "true" || toggle == "1";
            }
            else
            {
                // Toggle based on current state (check if any collider is disabled)
                noClip = colliders.Length > 0 && colliders[0].enabled;
            }
            
            // Enable/disable all colliders
            foreach (var collider in colliders)
            {
                collider.enabled = !noClip;
            }
            
            return $"No-clip {(noClip ? "enabled" : "disabled")} for {player.name}.";
        }
    }
    
    /// <summary>
    /// Show FPS and performance information
    /// </summary>
    [ConsoleCommand]
    public class FPSCommand : IConsoleCommand
    {
        public string Name => "fps";
        public string Description => "Show current FPS and performance information";
        public string[] Aliases => new[] { "performance", "perf" };
        public string Usage => "fps";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            float fps = 1f / Time.deltaTime;
            float avgFps = Time.frameCount / Time.time;
            
            string result = $"Performance Information:\n";
            result += $"Current FPS: {fps:F1}\n";
            result += $"Average FPS: {avgFps:F1}\n";
            result += $"Delta Time: {Time.deltaTime * 1000:F2}ms\n";
            result += $"Time Scale: {Time.timeScale}\n";
            result += $"Frame Count: {Time.frameCount}\n";
            result += $"Game Time: {Time.time:F1}s\n";
            result += $"Real Time: {Time.realtimeSinceStartup:F1}s";
            
            return result;
        }
    }
    
    /// <summary>
    /// Teleport to a specific player
    /// </summary>
    [ConsoleCommand]
    public class GotoPlayerCommand : IConsoleCommand
    {
        public string Name => "gotoplayer";
        public string Description => "Teleport one player to another player's location";
        public string[] Aliases => new[] { "gotopl", "tpto" };
        public string Usage => "gotoplayer <from_player> <to_player>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 2;
        }
        
        public string Execute(string[] parameters)
        {
            var fromPlayer = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (fromPlayer == null)
                return $"Player '{parameters[0]}' not found.";
                
            var toPlayer = PlayerCommandHelper.GetPlayerByName(parameters[1]);
            if (toPlayer == null)
                return $"Player '{parameters[1]}' not found.";
                
            if (fromPlayer.SpawnedPlayerGO == null)
                return $"Player {fromPlayer.name} is not spawned.";
                
            if (toPlayer.SpawnedPlayerGO == null)
                return $"Player {toPlayer.name} is not spawned.";
            
            Vector2 targetPosition = toPlayer.SpawnedPlayerGO.transform.position;
            fromPlayer.SpawnedPlayerGO.transform.position = targetPosition;
            
            return $"Teleported {fromPlayer.name} to {toPlayer.name}'s location ({targetPosition.x:F1}, {targetPosition.y:F1})";
        }
    }
    
    /// <summary>
    /// Set player movement speed
    /// </summary>
    [ConsoleCommand]
    public class SetSpeedCommand : IConsoleCommand
    {
        public string Name => "setspeed";
        public string Description => "Set a player's movement speed multiplier";
        public string[] Aliases => new[] { "speed" };
        public string Usage => "setspeed <player> <multiplier>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 2;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.spawnedPlayerDefence == null)
                return $"Player {player.name} is not spawned.";
            
            if (!float.TryParse(parameters[1], out float speedMultiplier))
                return "Invalid speed multiplier. Use a number (e.g., 1.0 for normal, 2.0 for double speed).";
            
            if (speedMultiplier < 0)
                return "Speed multiplier cannot be negative.";
            
            player.spawnedPlayerDefence.SetExtraMaxSpeedFactor(speedMultiplier - 1f); // -1 because it's an extra factor
            
            return $"Set {player.name} speed multiplier to {speedMultiplier}x.";
        }
    }
    
    /// <summary>
    /// Set player damage multiplier
    /// </summary>
    [ConsoleCommand]
    public class SetDamageCommand : IConsoleCommand
    {
        public string Name => "setdamage";
        public string Description => "Set a player's damage multiplier";
        public string[] Aliases => new[] { "damage", "dmg" };
        public string Usage => "setdamage <player> <multiplier>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 2;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.spawnedPlayerDefence == null)
                return $"Player {player.name} is not spawned.";
            
            if (!float.TryParse(parameters[1], out float damageMultiplier))
                return "Invalid damage multiplier. Use a number (e.g., 1.0 for normal, 2.0 for double damage).";
            
            if (damageMultiplier < 0)
                return "Damage multiplier cannot be negative.";
            
            player.spawnedPlayerDefence.SetExtraMaxDamageFactor(damageMultiplier - 1f); // -1 because it's an extra factor
            
            return $"Set {player.name} damage multiplier to {damageMultiplier}x.";
        }
    }
    
    /// <summary>
    /// Get current position of a player
    /// </summary>
    [ConsoleCommand]
    public class WhereIsCommand : IConsoleCommand
    {
        public string Name => "whereis";
        public string Description => "Get the current position of a player";
        public string[] Aliases => new[] { "pos", "position" };
        public string Usage => "whereis <player>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 1;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.SpawnedPlayerGO == null)
                return $"Player {player.name} is not spawned.";
            
            Vector2 position = player.SpawnedPlayerGO.transform.position;
            return $"{player.name} is at position ({position.x:F2}, {position.y:F2})";
        }
    }
    
    /// <summary>
    /// List all available scenes in build settings
    /// </summary>
    [ConsoleCommand]
    public class ListScenesCommand : IConsoleCommand
    {
        public string Name => "listscenes";
        public string Description => "List all available scenes in build settings";
        public string[] Aliases => new[] { "scenes", "levels" };
        public string Usage => "listscenes";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            string result = "Available scenes:\n";
            
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                result += $"  {i}: {sceneName}\n";
            }
            
            result += $"\nCurrent scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}";
            
            return result;
        }
    }
    
    /// <summary>
    /// Execute multiple commands in sequence
    /// </summary>
    [ConsoleCommand]
    public class ExecCommand : IConsoleCommand
    {
        public string Name => "exec";
        public string Description => "Execute multiple commands separated by semicolons";
        public string[] Aliases => new[] { "multi", "batch" };
        public string Usage => "exec \"command1; command2; command3\"";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 1;
        }
        
        public string Execute(string[] parameters)
        {
            string commandString = parameters[0];
            string[] commands = commandString.Split(';');
            
            string result = $"Executing {commands.Length} commands:\n";
            
            var consoleManager = ConsoleManager.I;
            if (consoleManager == null)
                return "Console manager not found.";
            
            for (int i = 0; i < commands.Length; i++)
            {
                string command = commands[i].Trim();
                if (string.IsNullOrEmpty(command))
                    continue;
                
                result += $"  {i + 1}. {command}\n";
                consoleManager.ExecuteCommand(command);
            }
            
            return result.TrimEnd();
        }
    }
}