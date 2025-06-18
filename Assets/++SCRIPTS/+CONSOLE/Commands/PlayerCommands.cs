using System.Linq;
using UnityEngine;
using __SCRIPTS;

namespace GangstaBean.Console
{
    /// <summary>
    /// Utilities for player-related console commands
    /// </summary>
    public static class PlayerCommandHelper
    {
        public static Player GetPlayerByName(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
                return null;
            
            var players = Players.I.AllJoinedPlayers;
            
            // Try by exact name match or player number
            if (int.TryParse(playerName, out int playerIndex))
            {
                if (playerIndex >= 1 && playerIndex <= players.Count)
                    return players[playerIndex - 1];
            }
            
            // Try by name contains
            return players.FirstOrDefault(p => p.name.ToLower().Contains(playerName.ToLower()));
        }
        
        public static string GetPlayerList()
        {
            var players = Players.I.AllJoinedPlayers;
            if (players.Count == 0)
                return "No players found.";
                
            string result = "Active players:\n";
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                string status = player.state.ToString();
                string health = player.spawnedPlayerDefence?.Health.ToString("F0") ?? "N/A";
                string maxHealth = player.spawnedPlayerDefence?.HealthMax.ToString("F0") ?? "N/A";
                
                result += $"  {i + 1}. {player.name} - {status} (HP: {health}/{maxHealth})\n";
            }
            return result.TrimEnd();
        }
    }
    
    /// <summary>
    /// Respawn a player at their current position or a specified location
    /// </summary>
    [ConsoleCommand]
    public class RespawnCommand : IConsoleCommand
    {
        public string Name => "respawn";
        public string Description => "Respawn a player at current position or specified coordinates";
        public string[] Aliases => new[] { "res", "r" };
        public string Usage => "respawn <player> [x y] [fromsky]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length >= 1 && parameters.Length <= 4;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found. Use 'players' to list active players.";
            
            Vector2 spawnPosition = Vector2.zero;
            bool fallFromSky = false;
            
            // Parse position if provided
            if (parameters.Length >= 3)
            {
                if (float.TryParse(parameters[1], out float x) && float.TryParse(parameters[2], out float y))
                {
                    spawnPosition = new Vector2(x, y);
                }
                else
                {
                    return "Invalid coordinates. Use format: respawn player1 100 200";
                }
            }
            else if (player.SpawnedPlayerGO != null)
            {
                spawnPosition = player.SpawnedPlayerGO.transform.position;
            }
            
            // Parse fall from sky option
            if (parameters.Length >= 4)
            {
                fallFromSky = parameters[3].ToLower() == "true" || parameters[3].ToLower() == "fromsky";
            }
            
            // Destroy existing player object if it exists
            if (player.SpawnedPlayerGO != null)
            {
                ObjectMaker.I.Unmake(player.SpawnedPlayerGO);
            }
            
            // Spawn the player
            var spawnedGO = player.Spawn(spawnPosition, fallFromSky);
            
            return $"Respawned {player.name} at ({spawnPosition.x:F1}, {spawnPosition.y:F1})";
        }
    }
    
    /// <summary>
    /// Heal a player to full health or a specific amount
    /// </summary>
    [ConsoleCommand]
    public class HealCommand : IConsoleCommand
    {
        public string Name => "heal";
        public string Description => "Heal a player to full health or a specific amount";
        public string[] Aliases => new[] { "hp" };
        public string Usage => "heal <player> [amount]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length >= 1 && parameters.Length <= 2;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.spawnedPlayerDefence == null)
                return $"Player {player.name} is not spawned or has no health component.";
            
            float healAmount;
            
            if (parameters.Length >= 2)
            {
                if (!float.TryParse(parameters[1], out healAmount))
                    return "Invalid heal amount. Use a number.";
            }
            else
            {
                healAmount = player.spawnedPlayerDefence.HealthMax;
            }
            
            player.spawnedPlayerDefence.AddHealth(healAmount);
            
            return $"Healed {player.name} for {healAmount} HP. Current: {player.spawnedPlayerDefence.Health:F0}/{player.spawnedPlayerDefence.HealthMax:F0}";
        }
    }
    
    /// <summary>
    /// Kill a player instantly
    /// </summary>
    [ConsoleCommand]
    public class KillCommand : IConsoleCommand
    {
        public string Name => "kill";
        public string Description => "Kill a player instantly";
        public string[] Aliases => new[] { "k" };
        public string Usage => "kill <player>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 1;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.spawnedPlayerDefence == null)
                return $"Player {player.name} is not spawned.";
            
            player.spawnedPlayerDefence.DieNow();
            
            return $"Killed {player.name}.";
        }
    }
    
    /// <summary>
    /// Teleport a player to specific coordinates
    /// </summary>
    [ConsoleCommand]
    public class TeleportCommand : IConsoleCommand
    {
        public string Name => "teleport";
        public string Description => "Teleport a player to specific coordinates";
        public string[] Aliases => new[] { "tp", "goto" };
        public string Usage => "teleport <player> <x> <y>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 3;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.SpawnedPlayerGO == null)
                return $"Player {player.name} is not spawned.";
            
            if (!float.TryParse(parameters[1], out float x) || !float.TryParse(parameters[2], out float y))
                return "Invalid coordinates. Use format: teleport player1 100 200";
            
            player.SpawnedPlayerGO.transform.position = new Vector2(x, y);
            
            return $"Teleported {player.name} to ({x:F1}, {y:F1})";
        }
    }
    
    /// <summary>
    /// Set player god mode (invincibility)
    /// </summary>
    [ConsoleCommand]
    public class GodModeCommand : IConsoleCommand
    {
        public string Name => "god";
        public string Description => "Toggle god mode (invincibility) for a player";
        public string[] Aliases => new[] { "godmode", "invincible" };
        public string Usage => "god <player> [on/off]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length >= 1 && parameters.Length <= 2;
        }
        
        public string Execute(string[] parameters)
        {
            var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
            if (player == null)
                return $"Player '{parameters[0]}' not found.";
                
            if (player.spawnedPlayerDefence == null)
                return $"Player {player.name} is not spawned.";
            
            bool godMode;
            
            if (parameters.Length >= 2)
            {
                string toggle = parameters[1].ToLower();
                godMode = toggle == "on" || toggle == "true" || toggle == "1";
            }
            else
            {
                // Toggle current state
                godMode = !player.spawnedPlayerDefence.cantDie;
            }
            
            player.spawnedPlayerDefence.cantDie = godMode;
            
            return $"God mode {(godMode ? "enabled" : "disabled")} for {player.name}.";
        }
    }
    
    /// <summary>
    /// List all active players
    /// </summary>
    [ConsoleCommand]
    public class PlayersCommand : IConsoleCommand
    {
        public string Name => "players";
        public string Description => "List all active players with their status";
        public string[] Aliases => new[] { "playerlist", "pl" };
        public string Usage => "players";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            return PlayerCommandHelper.GetPlayerList();
        }
    }
    
    /// <summary>
    /// Set player health to a specific value
    /// </summary>
    [ConsoleCommand]
    public class SetHealthCommand : IConsoleCommand
    {
        public string Name => "sethealth";
        public string Description => "Set a player's health to a specific value";
        public string[] Aliases => new[] { "sethp" };
        public string Usage => "sethealth <player> <amount>";
        
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
                return $"Player {player.name} is not spawned or has no health component.";
            
            if (!float.TryParse(parameters[1], out float health))
                return "Invalid health amount. Use a number.";
            
            player.spawnedPlayerDefence.Health = Mathf.Clamp(health, 0, player.spawnedPlayerDefence.HealthMax);
            // Trigger UI update by calling SetHealth method indirectly
            var currentHealth = player.spawnedPlayerDefence.Health;
            player.spawnedPlayerDefence.AddHealth(0); // This will trigger OnFractionChanged
            
            return $"Set {player.name} health to {player.spawnedPlayerDefence.Health:F0}/{player.spawnedPlayerDefence.HealthMax:F0}";
        }
    }
}