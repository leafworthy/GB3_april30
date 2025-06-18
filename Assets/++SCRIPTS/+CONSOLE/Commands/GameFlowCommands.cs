using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using __SCRIPTS;

namespace GangstaBean.Console
{
    /// <summary>
    /// Pause the game
    /// </summary>
    [ConsoleCommand]
    public class PauseCommand : IConsoleCommand
    {
        public string Name => "pause";
        public string Description => "Pause or unpause the game";
        public string[] Aliases => new[] { "p" };
        public string Usage => "pause [on/off]";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length <= 1;
        }
        
        public string Execute(string[] parameters)
        {
            var pauseManager = PauseManager.I;
            if (pauseManager == null)
                return "PauseManager not found.";
            
            bool shouldPause;
            
            if (parameters.Length == 0)
            {
                // Toggle current state
                shouldPause = !pauseManager.IsPaused;
            }
            else
            {
                string toggle = parameters[0].ToLower();
                shouldPause = toggle == "on" || toggle == "true" || toggle == "1";
            }
            
            if (shouldPause && !pauseManager.IsPaused)
            {
                pauseManager.Pause(Players.I.AllJoinedPlayers.FirstOrDefault());
                return "Game paused.";
            }
            else if (!shouldPause && pauseManager.IsPaused)
            {
                pauseManager.Unpause();
                return "Game unpaused.";
            }
            else
            {
                return $"Game is already {(pauseManager.IsPaused ? "paused" : "unpaused")}.";
            }
        }
    }
    
    /// <summary>
    /// Change game time scale
    /// </summary>
    [ConsoleCommand]
    public class TimeScaleCommand : IConsoleCommand
    {
        public string Name => "timescale";
        public string Description => "Set the game time scale (speed)";
        public string[] Aliases => new[] { "speed", "ts" };
        public string Usage => "timescale <scale>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 1;
        }
        
        public string Execute(string[] parameters)
        {
            if (!float.TryParse(parameters[0], out float scale))
                return "Invalid time scale. Use a number (e.g., 0.5 for half speed, 2.0 for double speed).";
            
            if (scale < 0)
                return "Time scale cannot be negative.";
            
            if (scale > 10)
                return "Time scale limited to maximum of 10x for stability.";
            
            Time.timeScale = scale;
            
            if (scale == 0)
                return "Time frozen (timescale = 0).";
            else if (scale == 1)
                return "Time scale reset to normal.";
            else if (scale < 1)
                return $"Time slowed to {scale}x speed.";
            else
                return $"Time accelerated to {scale}x speed.";
        }
    }
    
    /// <summary>
    /// Restart the current level
    /// </summary>
    [ConsoleCommand]
    public class RestartLevelCommand : IConsoleCommand
    {
        public string Name => "restart";
        public string Description => "Restart the current level";
        public string[] Aliases => new[] { "reload", "rl" };
        public string Usage => "restart";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            var levelManager = LevelManager.I;
            if (levelManager == null)
                return "LevelManager not found.";
            
            try
            {
                levelManager.RestartLevel();
                return "Restarting level...";
            }
            catch (System.Exception e)
            {
                return $"Failed to restart level: {e.Message}";
            }
        }
    }
    
    /// <summary>
    /// Load a specific level/scene
    /// </summary>
    [ConsoleCommand]
    public class LoadLevelCommand : IConsoleCommand
    {
        public string Name => "loadlevel";
        public string Description => "Load a specific level by name";
        public string[] Aliases => new[] { "load", "scene" };
        public string Usage => "loadlevel <level_name>";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 1;
        }
        
        public string Execute(string[] parameters)
        {
            string levelName = parameters[0];
            
            // Try to find the scene in build settings
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                
                if (sceneName.ToLower() == levelName.ToLower())
                {
                    try
                    {
                        SceneManager.LoadScene(sceneName);
                        return $"Loading level: {sceneName}";
                    }
                    catch (System.Exception e)
                    {
                        return $"Failed to load level '{sceneName}': {e.Message}";
                    }
                }
            }
            
            if (!sceneExists)
            {
                // List available scenes
                string availableScenes = "Available levels: ";
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                    availableScenes += sceneName + ", ";
                }
                
                return $"Level '{levelName}' not found. {availableScenes.TrimEnd(',', ' ')}";
            }
            
            return $"Failed to load level '{levelName}'.";
        }
    }
    
    /// <summary>
    /// Show current game status
    /// </summary>
    [ConsoleCommand]
    public class GameStatusCommand : IConsoleCommand
    {
        public string Name => "status";
        public string Description => "Show current game status information";
        public string[] Aliases => new[] { "info", "gameinfo" };
        public string Usage => "status";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            string result = "=== Game Status ===\n";
            
            // Scene info
            var currentScene = SceneManager.GetActiveScene();
            result += $"Current Scene: {currentScene.name}\n";
            
            // Time info
            result += $"Time Scale: {Time.timeScale}\n";
            result += $"Game Time: {Time.time:F1}s\n";
            result += $"Real Time: {Time.realtimeSinceStartup:F1}s\n";
            
            // Pause state
            var pauseManager = PauseManager.I;
            if (pauseManager != null)
            {
                result += $"Paused: {pauseManager.IsPaused}\n";
            }
            
            // Player info
            var players = Players.I;
            if (players != null)
            {
                result += $"Active Players: {players.AllJoinedPlayers.Count}\n";
            }
            
            // Enemy info
            int enemyCount = 0;
            var enemyManager = EnemyManager.I;
            if (enemyManager != null)
            {
                // Use reflection to get enemy count
                var field = typeof(EnemyManager).GetField("_allEnemies", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                
                if (field?.GetValue(null) is System.Collections.Generic.List<Life> enemies)
                {
                    enemyCount = enemies.Count(e => e != null && !e.IsDead());
                }
            }
            result += $"Active Enemies: {enemyCount}\n";
            
            // Level manager info
            var levelManager = LevelManager.I;
            if (levelManager != null)
            {
                try
                {
                    float levelTime = levelManager.GetCurrentLevelTimeElapsed();
                    result += $"Level Time: {levelTime:F1}s\n";
                }
                catch
                {
                    // Method might not be accessible
                }
            }
            
            // Performance info
            result += $"FPS: {1f / Time.deltaTime:F0}\n";
            result += $"Frame Count: {Time.frameCount}";
            
            return result;
        }
    }
    
    /// <summary>
    /// Reload unit stats from Google Sheets
    /// </summary>
    [ConsoleCommand]
    public class ReloadStatsCommand : IConsoleCommand
    {
        public string Name => "reloadstats";
        public string Description => "Reload unit stats from Google Sheets";
        public string[] Aliases => new[] { "refreshstats", "updatestats" };
        public string Usage => "reloadstats";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
            var statsManager = UnitStatsManager.I;
            if (statsManager == null)
                return "UnitStatsManager not found.";
            
            try
            {
                statsManager.RefreshData();
                return "Unit stats reload initiated. Check console for results.";
            }
            catch (System.Exception e)
            {
                return $"Failed to reload stats: {e.Message}";
            }
        }
    }
    
    /// <summary>
    /// Quit the game
    /// </summary>
    [ConsoleCommand]
    public class QuitCommand : IConsoleCommand
    {
        public string Name => "quit";
        public string Description => "Quit the game";
        public string[] Aliases => new[] { "exit", "q" };
        public string Usage => "quit";
        
        public bool ValidateParameters(string[] parameters)
        {
            return parameters.Length == 0;
        }
        
        public string Execute(string[] parameters)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            return "Stopping play mode.";
#else
            Application.Quit();
            return "Quitting game.";
#endif
        }
    }
}