using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles saving and loading player stats to/from disk
/// </summary>
public class PlayerStatsSaveSystem : MonoBehaviour
{
    [Header("File Settings")]
    [SerializeField] private string saveFileName = "player_stats";
    [SerializeField] private string fileExtension = ".json";
    
    [Header("Save Options")]
    [SerializeField] private bool autoSaveOnExit = true;
    [SerializeField] private bool autosaveOnStatChange = false;

    // Reference to stats handler
    private PlayerStatsHandlerRefactored statsHandler;
    
    // Event for when data is loaded
    public event Action<bool> OnDataLoaded;
    
    private void Awake()
    {
        statsHandler = GetComponent<PlayerStatsHandlerRefactored>();
        
        if (statsHandler == null)
        {
            Debug.LogError("PlayerStatsSaveSystem requires a PlayerStatsHandlerRefactored component");
            enabled = false;
            return;
        }
    }
    
    private void OnEnable()
    {
        if (statsHandler != null && autosaveOnStatChange)
        {
            statsHandler.OnStatChanged += HandleStatChanged;
        }
    }
    
    private void OnDisable()
    {
        if (statsHandler != null && autosaveOnStatChange)
        {
            statsHandler.OnStatChanged -= HandleStatChanged;
        }
        
        if (autoSaveOnExit)
        {
            SaveStats();
        }
    }
    
    private void HandleStatChanged(PlayerStat.StatType statType, float value)
    {
        if (autosaveOnStatChange)
        {
            SaveStats();
        }
    }
    
    /// <summary>
    /// Load player stats from disk
    /// </summary>
    /// <param name="playerIndex">Player index for multi-player games</param>
    /// <returns>True if loaded successfully</returns>
    public bool LoadStats(int playerIndex = 0)
    {
        try
        {
            string filePath = GetSaveFilePath(playerIndex);
            
            if (!File.Exists(filePath))
            {
                OnDataLoaded?.Invoke(false);
                return false;
            }
            
            string json = File.ReadAllText(filePath);
            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(json);
            
            if (data == null)
            {
                Debug.LogError("Failed to deserialize player stats data");
                OnDataLoaded?.Invoke(false);
                return false;
            }
            
            ApplyLoadedData(data);
            OnDataLoaded?.Invoke(true);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading player stats: {e.Message}");
            OnDataLoaded?.Invoke(false);
            return false;
        }
    }
    
    /// <summary>
    /// Save player stats to disk
    /// </summary>
    /// <param name="playerIndex">Player index for multi-player games</param>
    /// <returns>True if saved successfully</returns>
    public bool SaveStats(int playerIndex = 0)
    {
        try
        {
            PlayerStatsData data = CreateSaveData();
            string json = JsonUtility.ToJson(data, true);
            string filePath = GetSaveFilePath(playerIndex);
            
            // Ensure directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(filePath, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving player stats: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Delete saved player stats file
    /// </summary>
    /// <param name="playerIndex">Player index for multi-player games</param>
    /// <returns>True if deleted successfully</returns>
    public bool DeleteSavedStats(int playerIndex = 0)
    {
        try
        {
            string filePath = GetSaveFilePath(playerIndex);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting player stats: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Check if saved stats exist for the player
    /// </summary>
    /// <param name="playerIndex">Player index for multi-player games</param>
    /// <returns>True if saved stats exist</returns>
    public bool HasSavedStats(int playerIndex = 0)
    {
        return File.Exists(GetSaveFilePath(playerIndex));
    }
    
    /// <summary>
    /// Get the save file path for the player
    /// </summary>
    private string GetSaveFilePath(int playerIndex)
    {
        string fileName = saveFileName;
        
        if (playerIndex > 0)
        {
            fileName += $"_{playerIndex}";
        }
        
        return Path.Combine(Application.persistentDataPath, $"{fileName}{fileExtension}");
    }
    
    /// <summary>
    /// Create save data from current stats
    /// </summary>
    private PlayerStatsData CreateSaveData()
    {
        PlayerStatsData data = new PlayerStatsData();
        
        // Convert all enum values to dictionary entries
        foreach (PlayerStat.StatType statType in Enum.GetValues(typeof(PlayerStat.StatType)))
        {
            float value = statsHandler.GetStatValue(statType);
            data.statValues.Add(new StatEntry { type = statType.ToString(), value = value });
        }
        
        // Add metadata
        data.saveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.playerName = statsHandler.GetComponent<IPlayerContext>()?.PlayerName ?? "Player";
        
        return data;
    }
    
    /// <summary>
    /// Apply loaded data to the stats handler
    /// </summary>
    private void ApplyLoadedData(PlayerStatsData data)
    {
        foreach (StatEntry entry in data.statValues)
        {
            // Try to parse the stat type from string
            if (Enum.TryParse<PlayerStat.StatType>(entry.type, out PlayerStat.StatType statType))
            {
                statsHandler.SetStat(statType, entry.value);
            }
        }
    }
}

/// <summary>
/// Serializable data structure for player stats
/// </summary>
[Serializable]
public class PlayerStatsData
{
    public string saveDateTime;
    public string playerName;
    public List<StatEntry> statValues = new List<StatEntry>();
}

/// <summary>
/// Serializable stat entry for a single stat
/// </summary>
[Serializable]
public class StatEntry
{
    public string type;
    public float value;
}