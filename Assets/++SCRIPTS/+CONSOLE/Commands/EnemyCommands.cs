using System.Linq;
using UnityEngine;
using __SCRIPTS;
using __SCRIPTS._ZOMBIESPAWN;

namespace GangstaBean.Console
{
    /// <summary>
    /// Utilities for enemy-related console commands
    /// </summary>
    public static class EnemyCommandHelper
    {
        public static int GetActiveEnemyCount()
        {
            var enemyManager = EnemyManager.I;
            if (enemyManager == null)
                return 0;
            
            // Use reflection to access private _allEnemies field
            var field = typeof(EnemyManager).GetField("_allEnemies", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            
            if (field?.GetValue(null) is System.Collections.Generic.List<Life> enemies)
            {
                return enemies.Count(e => e != null && !e.IsDead());
            }
            
            return 0;
        }
        
        public static System.Collections.Generic.List<Life> GetActiveEnemies()
        {
            var enemyManager = EnemyManager.I;
            if (enemyManager == null)
                return new System.Collections.Generic.List<Life>();
            
            // Use reflection to access private _allEnemies field
            var field = typeof(EnemyManager).GetField("_allEnemies", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            
            if (field?.GetValue(null) is System.Collections.Generic.List<Life> enemies)
            {
                return enemies.Where(e => e != null && !e.IsDead()).ToList();
            }
            
            return new System.Collections.Generic.List<Life>();
        }
    }
    
    /// <summary>
    /// Spawn an enemy at specified coordinates
    /// </summary>
    [ConsoleCommand]
    public class SpawnEnemyCommand : IConsoleCommand
    {
        public string Name => "spawnenemy";
        public string Description => "Spawn an enemy at specified coordinates";
        public string[] Aliases => new[] { "spawn", "se" };
        public string Usage => "spawnenemy <enemy_type> <x> <y>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 3;
        }
        
        public string Execute(string[] parameters)
        {
            string enemyType = parameters[0].ToLower();
            
            if (!float.TryParse(parameters[1], out float x) || !float.TryParse(parameters[2], out float y))
                return "Invalid coordinates. Use format: spawnenemy zombie 100 200";
            
            Vector2 spawnPosition = new Vector2(x, y);
            
            // Try to find enemy prefab from ObjectMaker's pool list
            var objectMaker = ObjectMaker.I;
            if (objectMaker == null)
                return "ObjectMaker not found.";
            
            GameObject enemyPrefab = FindEnemyPrefab(enemyType);
            if (enemyPrefab == null)
                return $"Enemy type '{enemyType}' not found. Available types: zombie, cone, toast, tmato, donut, brock";
            
            // Spawn the enemy
            var spawnedEnemy = objectMaker.Make(enemyPrefab, spawnPosition);
            if (spawnedEnemy == null)
                return "Failed to spawn enemy.";
            
            // Set up the enemy
            var life = spawnedEnemy.GetComponent<Life>();
            if (life != null)
            {
                life.SetPlayer(Players.EnemyPlayer);
                EnemyManager.I.CollectEnemy(spawnedEnemy);
            }
            
            return $"Spawned {enemyType} at ({x:F1}, {y:F1})";
        }
        
        private GameObject FindEnemyPrefab(string enemyType)
        {
            var objectMaker = ObjectMaker.I;
            
            // Look through the ObjectsToPool list for matching enemy types
            foreach (var obj in objectMaker.ObjectsToPool)
            {
                if (obj == null) continue;
                
                string objName = obj.name.ToLower();
                
                // Match enemy types by name
                if ((enemyType == "zombie" && objName.Contains("zombie")) ||
                    (enemyType == "cone" && objName.Contains("cone")) ||
                    (enemyType == "toast" && objName.Contains("toast")) ||
                    (enemyType == "tmato" && objName.Contains("tmato")) ||
                    (enemyType == "donut" && objName.Contains("donut")) ||
                    (enemyType == "brock" && objName.Contains("brock")) ||
                    objName.Contains(enemyType))
                {
                    return obj;
                }
            }
            
            return null;
        }
    }
    
    /// <summary>
    /// Kill all enemies or enemies near a position
    /// </summary>
    [ConsoleCommand]
    public class KillEnemiesCommand : IConsoleCommand
    {
        public string Name => "killenemies";
        public string Description => "Kill all enemies or enemies within radius of position";
        public string[] Aliases => new[] { "killall", "ke" };
        public string Usage => "killenemies [x y radius]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0 || parameters.Length == 3;
        }
        
        public string Execute(string[] parameters)
        {
            var activeEnemies = EnemyCommandHelper.GetActiveEnemies();
            if (activeEnemies.Count == 0)
                return "No active enemies found.";
            
            int killedCount = 0;
            
            if (parameters.Length == 0)
            {
                // Kill all enemies with massive damage
                foreach (var enemy in activeEnemies)
                {
                    if (enemy != null && !enemy.IsDead())
                    {
                        var attack = new Attack(null, enemy, 99999f);
                        enemy.TakeDamage(attack);
                        killedCount++;
                    }
                }
                
                return $"Killed {killedCount} enemies.";
            }
            else
            {
                // Kill enemies within radius
                if (!float.TryParse(parameters[0], out float x) || 
                    !float.TryParse(parameters[1], out float y) ||
                    !float.TryParse(parameters[2], out float radius))
                {
                    return "Invalid parameters. Use format: killenemies 100 200 50";
                }
                
                Vector2 center = new Vector2(x, y);
                
                foreach (var enemy in activeEnemies)
                {
                    if (enemy != null && !enemy.IsDead())
                    {
                        float distance = Vector2.Distance(enemy.transform.position, center);
                        if (distance <= radius)
                        {
                            var attack = new Attack(null, enemy, 99999f);
                        enemy.TakeDamage(attack);
                            killedCount++;
                        }
                    }
                }
                
                return $"Killed {killedCount} enemies within {radius} units of ({x:F1}, {y:F1}).";
            }
        }
    }
    
    /// <summary>
    /// Clear all enemies without killing them (for cleanup)
    /// </summary>
    [ConsoleCommand]
    public class ClearEnemiesCommand : IConsoleCommand
    {
        public string Name => "clearenemies";
        public string Description => "Clear all enemies instantly (cleanup)";
        public string[] Aliases => new[] { "clearall", "ce" };
        public string Usage => "clearenemies";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            var activeEnemies = EnemyCommandHelper.GetActiveEnemies();
            int clearedCount = 0;
            
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    ObjectMaker.I.Unmake(enemy.gameObject);
                    clearedCount++;
                }
            }
            
            return $"Cleared {clearedCount} enemies.";
        }
    }
    
    /// <summary>
    /// Trigger a zombie wave
    /// </summary>
    [ConsoleCommand]
    public class TriggerWaveCommand : IConsoleCommand
    {
        public string Name => "wave";
        public string Description => "Trigger a specific zombie wave or advance to next wave";
        public string[] Aliases => new[] { "triggerwave", "w" };
        public string Usage => "wave [wave_number]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length <= 1;
        }
        
        public string Execute(string[] parameters)
        {
            var waveManager = Object.FindFirstObjectByType<ZombieWaveManager>();
            if (waveManager == null)
                return "No zombie wave manager found in current level.";
            
            if (!waveManager.isActive)
                return "Zombie wave manager is not active.";
            
            if (parameters.Length == 0)
            {
                // Trigger next wave
                ZombieWaveManager.OnWaveEnd?.Invoke();
                return "Triggered next wave.";
            }
            else
            {
                if (!int.TryParse(parameters[0], out int waveNumber))
                    return "Invalid wave number. Use a whole number.";
                
                if (waveNumber < 1)
                    return "Wave number must be 1 or higher.";
                
                // Try to access the current wave index and set it
                var field = typeof(ZombieWaveManager).GetField("currentWaveIndex", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field != null)
                {
                    field.SetValue(waveManager, waveNumber - 1);
                    ZombieWaveManager.OnWaveEnd?.Invoke();
                    return $"Set current wave to {waveNumber} and triggered it.";
                }
                else
                {
                    return "Unable to set specific wave number.";
                }
            }
        }
    }
    
    /// <summary>
    /// Show enemy status information
    /// </summary>
    [ConsoleCommand]
    public class EnemyStatusCommand : IConsoleCommand
    {
        public string Name => "enemystatus";
        public string Description => "Show information about active enemies";
        public string[] Aliases => new[] { "enemies", "es" };
        public string Usage => "enemystatus";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            var activeEnemies = EnemyCommandHelper.GetActiveEnemies();
            
            if (activeEnemies.Count == 0)
                return "No active enemies found.";
            
            string result = $"Active enemies ({activeEnemies.Count}):\n";
            
            // Group enemies by type
            var enemyGroups = activeEnemies
                .GroupBy(e => e.gameObject.name.Replace("(Clone)", "").Trim())
                .OrderBy(g => g.Key);
            
            foreach (var group in enemyGroups)
            {
                string enemyType = group.Key;
                int count = group.Count();
                result += $"  {enemyType}: {count}\n";
            }
            
            // Show wave manager status if available
            var waveManager = Object.FindFirstObjectByType<ZombieWaveManager>();
            if (waveManager != null && waveManager.isActive)
            {
                int currentWave = waveManager.GetCurrentWaveIndex() + 1;
                float timeToNext = waveManager.GetTimeTillNextWave();
                result += $"\nWave Manager: Wave {currentWave}, Next in {timeToNext:F1}s";
            }
            
            return result.TrimEnd();
        }
    }
}