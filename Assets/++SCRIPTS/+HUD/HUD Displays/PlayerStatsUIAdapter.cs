using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adapter to connect PlayerStatsHandlerRefactored with UI elements
/// </summary>
public class PlayerStatsUIAdapter : MonoBehaviour
{
    [Header("Text UI Elements")]
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI gasText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    
    [Header("Key UI Elements")]
    [SerializeField] private GameObject keyIcon;
    
    [Header("Optional UI Elements")]
    [SerializeField] private TextMeshProUGUI attacksHitText;
    [SerializeField] private TextMeshProUGUI attacksTotalText;
    
    // Reference to player stats
    private IPlayerStats playerStats;
    
    // Dictionary to track which UI elements are linked to which stats
    private Dictionary<PlayerStat.StatType, Action<float>> uiUpdateActions;
    
    private void Awake()
    {
        // Initialize the dictionary of update actions
        InitializeUIActions();
    }
    
    private void OnEnable()
    {
        if (playerStats != null)
        {
            // Subscribe to stat changes
            if (playerStats is PlayerStatsHandlerRefactored refactoredStats)
            {
                refactoredStats.OnStatChanged += HandleStatChanged;
            }
            
            // Update all UI elements initially
            UpdateAllUI();
        }
    }
    
    private void OnDisable()
    {
        if (playerStats != null)
        {
            // Unsubscribe when disabled
            if (playerStats is PlayerStatsHandlerRefactored refactoredStats)
            {
                refactoredStats.OnStatChanged -= HandleStatChanged;
            }
        }
    }
    
    private void Start()
    {
        // Try to find player stats if not assigned
        if (playerStats == null)
        {
            FindPlayerStats();
        }
    }
    
    /// <summary>
    /// Initialize the dictionary that maps stat types to UI update actions
    /// </summary>
    private void InitializeUIActions()
    {
        uiUpdateActions = new Dictionary<PlayerStat.StatType, Action<float>>
        {
            { PlayerStat.StatType.TotalCash, UpdateCashUI },
            { PlayerStat.StatType.Gas, UpdateGasUI },
            { PlayerStat.StatType.Kills, UpdateKillsUI },
            { PlayerStat.StatType.Accuracy, UpdateAccuracyUI },
            { PlayerStat.StatType.Key, UpdateKeyUI },
            { PlayerStat.StatType.AttacksHit, UpdateAttacksHitUI },
            { PlayerStat.StatType.AttacksTotal, UpdateAttacksTotalUI }
        };
    }
    
    /// <summary>
    /// Find player stats component in the scene
    /// </summary>
    private void FindPlayerStats()
    {
        // Try to find on this object first
        playerStats = GetComponent<IPlayerStats>();
        
        // If not found, look for it on other objects
        if (playerStats == null)
        {
            var statsHandler = FindObjectOfType<PlayerStatsHandlerRefactored>();
            if (statsHandler != null)
            {
                playerStats = statsHandler;
                
                // Subscribe to stat changes
                statsHandler.OnStatChanged += HandleStatChanged;
                
                // Update all UI elements initially
                UpdateAllUI();
            }
            else
            {
                Debug.LogWarning("PlayerStatsUIAdapter: Could not find PlayerStatsHandlerRefactored in the scene");
            }
        }
    }
    
    /// <summary>
    /// Set the player stats reference manually
    /// </summary>
    public void SetPlayerStats(IPlayerStats stats)
    {
        // Unsubscribe from old stats
        if (playerStats is PlayerStatsHandlerRefactored oldRefactoredStats)
        {
            oldRefactoredStats.OnStatChanged -= HandleStatChanged;
        }
        
        // Set new stats
        playerStats = stats;
        
        // Subscribe to new stats
        if (playerStats is PlayerStatsHandlerRefactored newRefactoredStats)
        {
            newRefactoredStats.OnStatChanged += HandleStatChanged;
            
            // Update all UI elements initially
            UpdateAllUI();
        }
    }
    
    /// <summary>
    /// Handle stat changed event
    /// </summary>
    private void HandleStatChanged(PlayerStat.StatType statType, float newValue)
    {
        // Update the corresponding UI element
        if (uiUpdateActions.TryGetValue(statType, out var updateAction))
        {
            updateAction.Invoke(newValue);
        }
    }
    
    /// <summary>
    /// Update all UI elements with current stat values
    /// </summary>
    public void UpdateAllUI()
    {
        if (playerStats == null) return;
        
        // Update each UI element
        foreach (var pair in uiUpdateActions)
        {
            pair.Value.Invoke(playerStats.GetStatValue(pair.Key));
        }
    }
    
    #region UI Update Methods
    
    private void UpdateCashUI(float value)
    {
        if (cashText != null)
        {
            cashText.text = ((int)value).ToString();
        }
    }
    
    private void UpdateGasUI(float value)
    {
        if (gasText != null)
        {
            gasText.text = ((int)value).ToString();
        }
    }
    
    private void UpdateKillsUI(float value)
    {
        if (killsText != null)
        {
            killsText.text = ((int)value).ToString();
        }
    }
    
    private void UpdateAccuracyUI(float value)
    {
        if (accuracyText != null)
        {
            accuracyText.text = $"{value:F1}%";
        }
    }
    
    private void UpdateKeyUI(float value)
    {
        if (keyIcon != null)
        {
            keyIcon.SetActive(value > 0);
        }
    }
    
    private void UpdateAttacksHitUI(float value)
    {
        if (attacksHitText != null)
        {
            attacksHitText.text = ((int)value).ToString();
        }
    }
    
    private void UpdateAttacksTotalUI(float value)
    {
        if (attacksTotalText != null)
        {
            attacksTotalText.text = ((int)value).ToString();
        }
    }
    
    #endregion
}