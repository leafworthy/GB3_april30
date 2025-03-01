using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of IPlayerStats that manages player statistics
/// </summary>
public class PlayerStatsHandlerRefactored : MonoBehaviour, IPlayerStats
{
    [SerializeField] private PlayerData playerData;
    
    // Event for notifying stat changes
    public event Action<PlayerStat.StatType, float> OnStatChanged;
    
    // Dictionary to store stat values
    private Dictionary<PlayerStat.StatType, float> stats = new Dictionary<PlayerStat.StatType, float>();
    
    // Common stats properties
    public int Cash => (int)GetStatValue(PlayerStat.StatType.TotalCash);
    public int Gas => (int)GetStatValue(PlayerStat.StatType.Gas);
    public int Kills => (int)GetStatValue(PlayerStat.StatType.Kills);
    
    // Additional properties for common stats
    public bool HasKey => GetStatValue(PlayerStat.StatType.Key) > 0;
    public float Accuracy => GetStatValue(PlayerStat.StatType.Accuracy);
    public int AttacksTotal => (int)GetStatValue(PlayerStat.StatType.AttacksTotal);
    public int AttacksHit => (int)GetStatValue(PlayerStat.StatType.AttacksHit);
    
    // Reference to player context
    private IPlayerContext playerContext;
    
    private void Awake()
    {
        // Initialize stats
        InitializeStats();
        
        // Try to get player context
        playerContext = GetComponent<IPlayerContext>();
    }
    
    /// <summary>
    /// Initialize with default values from player data
    /// </summary>
    private void InitializeStats()
    {
        // Initialize all stat types with default values
        foreach (PlayerStat.StatType statType in Enum.GetValues(typeof(PlayerStat.StatType)))
        {
            // Set default value based on stat type
            float defaultValue = GetDefaultValue(statType);
            stats[statType] = defaultValue;
        }
    }
    
    /// <summary>
    /// Get default value for a stat type
    /// </summary>
    private float GetDefaultValue(PlayerStat.StatType statType)
    {
        // Use player data if available
        if (playerData != null)
        {
            switch (statType)
            {
                case PlayerStat.StatType.TotalCash:
                    return playerData.startingCash;
                case PlayerStat.StatType.Gas:
                    return playerData.startingGas;
                case PlayerStat.StatType.Key:
                    return 0; // Start with no key
            }
        }
        
        // Default values for other stats
        switch (statType)
        {
            case PlayerStat.StatType.Accuracy:
                return 100f; // Start at 100% accuracy
            case PlayerStat.StatType.AttacksHit:
            case PlayerStat.StatType.AttacksTotal:
            case PlayerStat.StatType.Kills:
                return 0; // Start with 0 for these counters
            default:
                return 0;
        }
    }
    
    /// <summary>
    /// Get the current value of a stat
    /// </summary>
    public float GetStatValue(PlayerStat.StatType type)
    {
        // Ensure the stat exists
        if (!stats.ContainsKey(type))
        {
            stats[type] = GetDefaultValue(type);
        }
        
        // Special case for accuracy which is calculated
        if (type == PlayerStat.StatType.Accuracy && GetStatValue(PlayerStat.StatType.AttacksTotal) > 0)
        {
            return (GetStatValue(PlayerStat.StatType.AttacksHit) / GetStatValue(PlayerStat.StatType.AttacksTotal)) * 100f;
        }
        
        return stats[type];
    }
    
    /// <summary>
    /// Change a stat by the specified amount
    /// </summary>
    public void ChangeStat(PlayerStat.StatType type, float amount)
    {
        // Get current value
        float currentValue = GetStatValue(type);
        
        // Calculate new value
        float newValue = currentValue + amount;
        
        // Validate new value before updating
        newValue = ValidateStatValue(type, newValue);
        
        // Update the stat
        SetStat(type, newValue);
        
        // Special handling for certain stats
        if (type == PlayerStat.StatType.AttacksHit || type == PlayerStat.StatType.AttacksTotal)
        {
            // Recalculate accuracy
            OnStatChanged?.Invoke(PlayerStat.StatType.Accuracy, GetStatValue(PlayerStat.StatType.Accuracy));
        }
    }
    
    /// <summary>
    /// Set a stat to a specific value
    /// </summary>
    public void SetStat(PlayerStat.StatType type, float value)
    {
        // Validate value before setting
        float validatedValue = ValidateStatValue(type, value);
        
        // Update the stat
        stats[type] = validatedValue;
        
        // Notify listeners
        OnStatChanged?.Invoke(type, validatedValue);
    }
    
    /// <summary>
    /// Validate stat value based on its type and constraints
    /// </summary>
    private float ValidateStatValue(PlayerStat.StatType type, float value)
    {
        switch (type)
        {
            case PlayerStat.StatType.TotalCash:
            case PlayerStat.StatType.Gas:
            case PlayerStat.StatType.Kills:
            case PlayerStat.StatType.AttacksHit:
            case PlayerStat.StatType.AttacksTotal:
                // These stats should not be negative
                return Mathf.Max(0, value);
                
            case PlayerStat.StatType.Key:
                // Key can only be 0 or 1
                return Mathf.Clamp01(value);
                
            case PlayerStat.StatType.Accuracy:
                // Accuracy should be between 0 and 100
                return Mathf.Clamp(value, 0f, 100f);
                
            default:
                return value;
        }
    }
    
    /// <summary>
    /// Reset all stats to default values
    /// </summary>
    public void ResetStats()
    {
        // Reinitialize all stats
        InitializeStats();
        
        // Notify listeners of all changes
        foreach (var stat in stats)
        {
            OnStatChanged?.Invoke(stat.Key, stat.Value);
        }
    }
    
    /// <summary>
    /// Add a key to the player inventory
    /// </summary>
    public void AddKey()
    {
        SetStat(PlayerStat.StatType.Key, 1);
    }
    
    /// <summary>
    /// Remove a key from the player inventory
    /// </summary>
    public void UseKey()
    {
        if (HasKey)
        {
            SetStat(PlayerStat.StatType.Key, 0);
        }
    }
    
    /// <summary>
    /// Check if player has enough money for a purchase
    /// </summary>
    public bool CanAfford(int amount)
    {
        return Cash >= amount;
    }
    
    /// <summary>
    /// Spend money if player has enough
    /// </summary>
    /// <returns>True if successful, false if not enough money</returns>
    public bool SpendMoney(int amount)
    {
        if (!CanAfford(amount))
        {
            return false;
        }
        
        ChangeStat(PlayerStat.StatType.TotalCash, -amount);
        return true;
    }
    
    /// <summary>
    /// Record an attack attempt
    /// </summary>
    /// <param name="hit">Whether the attack hit</param>
    public void RecordAttack(bool hit)
    {
        ChangeStat(PlayerStat.StatType.AttacksTotal, 1);
        if (hit)
        {
            ChangeStat(PlayerStat.StatType.AttacksHit, 1);
        }
    }
    
    /// <summary>
    /// Record a kill
    /// </summary>
    public void RecordKill()
    {
        ChangeStat(PlayerStat.StatType.Kills, 1);
    }
}