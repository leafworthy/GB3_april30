using System.Linq;
using UnityEngine;
using __SCRIPTS;

namespace GangstaBean.Console
{
    /// <summary>
    /// Fill all ammo for a player or all players
    /// </summary>
    [ConsoleCommand]
    public class FillAmmoCommand : IConsoleCommand
    {
        public string Name => "fillammo";
        public string Description => "Fill ammo for a specific player or all players";
        public string[] Aliases => new[] { "ammo", "fillall" };
        public string Usage => "fillammo [player/all] [ammo_type]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length <= 2;
        }
        
        public string Execute(string[] parameters)
        {
            // Default to all players if no parameter
            bool fillAllPlayers = parameters.Length == 0 || parameters[0].ToLower() == "all";
            AmmoInventory.AmmoType? specificAmmoType = null;
            
            // Parse ammo type if specified
            if (parameters.Length >= 2)
            {
                specificAmmoType = ParseAmmoType(parameters[1]);
                if (!specificAmmoType.HasValue)
                    return $"Invalid ammo type '{parameters[1]}'. Valid types: primary, secondary, tertiary, unlimited";
            }
            
            if (fillAllPlayers)
            {
                int playerCount = 0;
                foreach (var player in Players.I.AllJoinedPlayers)
                {
                    if (FillPlayerAmmo(player, specificAmmoType))
                        playerCount++;
                }
                
                string ammoTypeStr = specificAmmoType?.ToString().ToLower() ?? "all";
                return $"Filled {ammoTypeStr} ammo for {playerCount} players.";
            }
            else
            {
                var player = PlayerCommandHelper.GetPlayerByName(parameters[0]);
                if (player == null)
                    return $"Player '{parameters[0]}' not found.";
                
                if (FillPlayerAmmo(player, specificAmmoType))
                {
                    string ammoTypeStr = specificAmmoType?.ToString().ToLower() ?? "all";
                    return $"Filled {ammoTypeStr} ammo for {player.name}.";
                }
                else
                {
                    return $"Could not fill ammo for {player.name} (not spawned or no ammo component).";
                }
            }
        }
        
        private bool FillPlayerAmmo(Player player, AmmoInventory.AmmoType? specificType)
        {
            if (player.SpawnedPlayerGO == null)
                return false;
                
            var ammoInventory = player.SpawnedPlayerGO.GetComponent<AmmoInventory>();
            if (ammoInventory == null)
                return false;
            
            if (specificType.HasValue)
            {
                FillSpecificAmmoType(ammoInventory, specificType.Value);
            }
            else
            {
                // Fill all ammo types
                FillSpecificAmmoType(ammoInventory, AmmoInventory.AmmoType.Primary);
                FillSpecificAmmoType(ammoInventory, AmmoInventory.AmmoType.Secondary);
                FillSpecificAmmoType(ammoInventory, AmmoInventory.AmmoType.Tertiary);
                FillSpecificAmmoType(ammoInventory, AmmoInventory.AmmoType.Unlimited);
            }
            
            return true;
        }
        
        private void FillSpecificAmmoType(AmmoInventory inventory, AmmoInventory.AmmoType ammoType)
        {
            // Add a large amount to ensure it's filled
            inventory.AddAmmoToReserve(ammoType, 9999);
        }
        
        private AmmoInventory.AmmoType? ParseAmmoType(string ammoTypeStr)
        {
            return ammoTypeStr.ToLower() switch
            {
                "primary" or "1" or "p" => AmmoInventory.AmmoType.Primary,
                "secondary" or "2" or "s" => AmmoInventory.AmmoType.Secondary,
                "tertiary" or "3" or "t" => AmmoInventory.AmmoType.Tertiary,
                "unlimited" or "4" or "u" => AmmoInventory.AmmoType.Unlimited,
                _ => null
            };
        }
    }
    
    /// <summary>
    /// Give specific amount of ammo to a player
    /// </summary>
    [ConsoleCommand]
    public class GiveAmmoCommand : IConsoleCommand
    {
        public string Name => "giveammo";
        public string Description => "Give a specific amount of ammo to a player";
        public string[] Aliases => new[] { "give" };
        public string Usage => "giveammo <player> <ammo_type> <amount>";
        
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
            
            var ammoInventory = player.SpawnedPlayerGO.GetComponent<AmmoInventory>();
            if (ammoInventory == null)
                return $"Player {player.name} has no ammo inventory.";
            
            var ammoType = ParseAmmoType(parameters[1]);
            if (!ammoType.HasValue)
                return $"Invalid ammo type '{parameters[1]}'. Valid types: primary, secondary, tertiary, unlimited";
            
            if (!int.TryParse(parameters[2], out int amount))
                return "Invalid amount. Use a whole number.";
            
            ammoInventory.AddAmmoToReserve(ammoType.Value, amount);
            
            return $"Gave {amount} {ammoType.Value.ToString().ToLower()} ammo to {player.name}.";
        }
        
        private AmmoInventory.AmmoType? ParseAmmoType(string ammoTypeStr)
        {
            return ammoTypeStr.ToLower() switch
            {
                "primary" or "1" or "p" => AmmoInventory.AmmoType.Primary,
                "secondary" or "2" or "s" => AmmoInventory.AmmoType.Secondary,
                "tertiary" or "3" or "t" => AmmoInventory.AmmoType.Tertiary,
                "unlimited" or "4" or "u" => AmmoInventory.AmmoType.Unlimited,
                _ => null
            };
        }
    }
    
    /// <summary>
    /// Clear all ammo for a player
    /// </summary>
    [ConsoleCommand]
    public class ClearAmmoCommand : IConsoleCommand
    {
        public string Name => "clearammo";
        public string Description => "Clear all ammo for a player";
        public string[] Aliases => new[] { "noammo" };
        public string Usage => "clearammo <player> [ammo_type]";
        
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
            
            var ammoInventory = player.SpawnedPlayerGO.GetComponent<AmmoInventory>();
            if (ammoInventory == null)
                return $"Player {player.name} has no ammo inventory.";
            
            AmmoInventory.AmmoType? specificType = null;
            if (parameters.Length >= 2)
            {
                specificType = ParseAmmoType(parameters[1]);
                if (!specificType.HasValue)
                    return $"Invalid ammo type '{parameters[1]}'. Valid types: primary, secondary, tertiary, unlimited";
            }
            
            if (specificType.HasValue)
            {
                ClearSpecificAmmo(ammoInventory, specificType.Value);
                return $"Cleared {specificType.Value.ToString().ToLower()} ammo for {player.name}.";
            }
            else
            {
                // Clear all ammo types
                ClearSpecificAmmo(ammoInventory, AmmoInventory.AmmoType.Primary);
                ClearSpecificAmmo(ammoInventory, AmmoInventory.AmmoType.Secondary);
                ClearSpecificAmmo(ammoInventory, AmmoInventory.AmmoType.Tertiary);
                ClearSpecificAmmo(ammoInventory, AmmoInventory.AmmoType.Unlimited);
                return $"Cleared all ammo for {player.name}.";
            }
        }
        
        private void ClearSpecificAmmo(AmmoInventory inventory, AmmoInventory.AmmoType ammoType)
        {
            // Remove a large amount to ensure it's cleared
            inventory.AddAmmoToReserve(ammoType, -9999);
        }
        
        private AmmoInventory.AmmoType? ParseAmmoType(string ammoTypeStr)
        {
            return ammoTypeStr.ToLower() switch
            {
                "primary" or "1" or "p" => AmmoInventory.AmmoType.Primary,
                "secondary" or "2" or "s" => AmmoInventory.AmmoType.Secondary,
                "tertiary" or "3" or "t" => AmmoInventory.AmmoType.Tertiary,
                "unlimited" or "4" or "u" => AmmoInventory.AmmoType.Unlimited,
                _ => null
            };
        }
    }
    
    /// <summary>
    /// Show ammo status for a player
    /// </summary>
    [ConsoleCommand]
    public class AmmoStatusCommand : IConsoleCommand
    {
        public string Name => "ammostatus";
        public string Description => "Show current ammo status for a player";
        public string[] Aliases => new[] { "ammoinfo", "checkammo" };
        public string Usage => "ammostatus <player>";
        
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
            
            var ammoInventory = player.SpawnedPlayerGO.GetComponent<AmmoInventory>();
            if (ammoInventory == null)
                return $"Player {player.name} has no ammo inventory.";
            
            string result = $"Ammo status for {player.name}:\n";
            result += $"  Primary: {GetAmmoAmount(ammoInventory.primaryAmmo)}\n";
            result += $"  Secondary: {GetAmmoAmount(ammoInventory.secondaryAmmo)}\n";
            result += $"  Tertiary: {GetAmmoAmount(ammoInventory.tertiaryAmmo)}\n";
            result += $"  Unlimited: {GetAmmoAmount(ammoInventory.unlimitedAmmo)}";
            
            return result;
        }
        
        private string GetAmmoAmount(Ammo ammo)
        {
            if (ammo == null)
                return "N/A";
                
            if (ammo.unlimited)
                return "Unlimited";
                
            return $"{ammo.AmmoInClip}/{ammo.clipSize} (Reserve: {ammo.reserveAmmo}/{ammo.maxReserveAmmo})";
        }
    }
}