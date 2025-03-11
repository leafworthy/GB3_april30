using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility for testing and debugging the Smart Zombie Spawning System.
/// Provides controls and information display during runtime.
/// </summary>
public class SmartZombieSpawningTester : MonoBehaviour
{
    [Header("References")]
    public SmartZombieSpawningSystem spawningSystem;
    public DayNightCycle dayNightCycle;
    
    [Header("Test Controls")]
    public bool showTestControls = true;
    public KeyCode toggleSystemKey = KeyCode.F1;
    public KeyCode forceDawnKey = KeyCode.F2;
    public KeyCode forceNoonKey = KeyCode.F3;
    public KeyCode forceSunsetKey = KeyCode.F4;
    public KeyCode forceNightKey = KeyCode.F5;
    public KeyCode forceSpawnKey = KeyCode.F6;
    
    [Header("Display Settings")]
    public bool showInfoOverlay = true;
    public Color textColor = Color.white;
    public Vector2 displayOffset = new Vector2(10, 10);
    
    // Private variables
    private int totalSpawnAttempts = 0;
    private int successfulSpawns = 0;
    private int failedSpawns = 0;
    private float lastSpawnTime = 0;
    private GUIStyle guiStyle;
    
    private void Start()
    {
        // Find references if not set
        if (spawningSystem == null)
        {
            spawningSystem = FindObjectOfType<SmartZombieSpawningSystem>();
        }
        
        if (dayNightCycle == null && spawningSystem != null)
        {
            dayNightCycle = spawningSystem.dayNightCycle;
        }
        
        // Create GUI style
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = textColor;
        guiStyle.fontSize = 14;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.wordWrap = true;
    }
    
    private void Update()
    {
        if (!showTestControls) return;
        
        // Toggle system on/off
        if (Input.GetKeyDown(toggleSystemKey) && spawningSystem != null)
        {
            spawningSystem.enabled = !spawningSystem.enabled;
            Debug.Log($"Spawning system {(spawningSystem.enabled ? "enabled" : "disabled")}");
        }
        
        // Time of day controls
        if (dayNightCycle != null)
        {
            if (Input.GetKeyDown(forceDawnKey))
            {
                dayNightCycle.SkipToTime(0.25f);
                Debug.Log("Time set to dawn");
            }
            else if (Input.GetKeyDown(forceNoonKey))
            {
                dayNightCycle.SkipToTime(0.5f);
                Debug.Log("Time set to noon");
            }
            else if (Input.GetKeyDown(forceSunsetKey))
            {
                dayNightCycle.SkipToTime(0.75f);
                Debug.Log("Time set to sunset");
            }
            else if (Input.GetKeyDown(forceNightKey))
            {
                dayNightCycle.SkipToTime(0.0f);
                Debug.Log("Time set to midnight");
            }
        }
        
        // Force spawn
        if (Input.GetKeyDown(forceSpawnKey) && spawningSystem != null)
        {
            ForceSpawn();
        }
    }
    
    private void ForceSpawn()
    {
        if (spawningSystem == null) return;
        
        totalSpawnAttempts++;
        lastSpawnTime = Time.time;
        
        // Get spawn points
        SmartZombieSpawnPoint[] spawnPoints = spawningSystem.GetComponentsInChildren<SmartZombieSpawnPoint>();
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points available");
            failedSpawns++;
            return;
        }
        
        // Select a random spawn point
        SmartZombieSpawnPoint selectedPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        
        // Get prefabs from ASSETS
        GameObject[] enemyPrefabs = new GameObject[] {
            ASSETS.Players.ToastEnemyPrefab,
            ASSETS.Players.ConeEnemyPrefab,
            ASSETS.Players.DonutEnemyPrefab,
            ASSETS.Players.CornEnemyPrefab
        };
        
        // Try to spawn
        GameObject enemy = selectedPoint.TrySpawn(
            enemyPrefabs,
            spawningSystem.ensureOffCameraSpawning,
            Camera.main,
            spawningSystem.offCameraMargin
        );
        
        if (enemy != null)
        {
            successfulSpawns++;
            Debug.Log($"Force spawned enemy at {enemy.transform.position}");
        }
        else
        {
            failedSpawns++;
            Debug.LogWarning("Failed to force spawn enemy");
        }
    }
    
    private void OnGUI()
    {
        if (!showInfoOverlay || spawningSystem == null) return;
        
        float y = displayOffset.y;
        float x = displayOffset.x;
        float lineHeight = 20;
        
        // Title
        GUI.Label(new Rect(x, y, 300, 30), "Smart Zombie Spawning System", guiStyle);
        y += lineHeight * 1.5f;
        
        // System status
        string status = spawningSystem.enabled ? "ACTIVE" : "DISABLED";
        Color statusColor = spawningSystem.enabled ? Color.green : Color.red;
        GUIStyle statusStyle = new GUIStyle(guiStyle);
        statusStyle.normal.textColor = statusColor;
        GUI.Label(new Rect(x, y, 300, lineHeight), $"Status: {status}", statusStyle);
        y += lineHeight;
        
        // Time info
        if (dayNightCycle != null)
        {
            float timeOfDay = dayNightCycle.GetCurrentDayFraction();
            string timeLabel = GetTimeLabel(timeOfDay);
            GUI.Label(new Rect(x, y, 300, lineHeight), $"Time: {timeLabel} ({timeOfDay:F2})", guiStyle);
            y += lineHeight;
            
            float spawnRate = spawningSystem.spawnRateCurve.Evaluate(timeOfDay);
            GUIStyle rateStyle = new GUIStyle(guiStyle);
            rateStyle.normal.textColor = Color.Lerp(Color.red, Color.green, spawnRate);
            GUI.Label(new Rect(x, y, 300, lineHeight), $"Spawn Rate: {spawnRate:F2}", rateStyle);
            y += lineHeight;
        }
        
        // Spawn stats
        GUI.Label(new Rect(x, y, 300, lineHeight), $"Total Spawn Attempts: {totalSpawnAttempts}", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"Successful Spawns: {successfulSpawns}", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"Failed Spawns: {failedSpawns}", guiStyle);
        y += lineHeight;
        
        if (lastSpawnTime > 0)
        {
            GUI.Label(new Rect(x, y, 300, lineHeight), $"Last Spawn: {Time.time - lastSpawnTime:F1}s ago", guiStyle);
            y += lineHeight;
        }
        
        // Controls help
        y += lineHeight;
        GUI.Label(new Rect(x, y, 300, lineHeight), "Controls:", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"{toggleSystemKey}: Toggle System", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"{forceDawnKey}: Set Dawn", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"{forceNoonKey}: Set Noon", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"{forceSunsetKey}: Set Sunset", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"{forceNightKey}: Set Midnight", guiStyle);
        y += lineHeight;
        
        GUI.Label(new Rect(x, y, 300, lineHeight), $"{forceSpawnKey}: Force Spawn", guiStyle);
        y += lineHeight;
    }
    
    private string GetTimeLabel(float timeOfDay)
    {
        if (timeOfDay < 0.05f) return "Midnight";
        if (timeOfDay < 0.2f) return "Night";
        if (timeOfDay < 0.25f) return "Early Morning";
        if (timeOfDay < 0.3f) return "Sunrise";
        if (timeOfDay < 0.45f) return "Morning";
        if (timeOfDay < 0.55f) return "Noon";
        if (timeOfDay < 0.7f) return "Afternoon";
        if (timeOfDay < 0.8f) return "Sunset";
        if (timeOfDay < 0.95f) return "Evening";
        return "Night";
    }
}