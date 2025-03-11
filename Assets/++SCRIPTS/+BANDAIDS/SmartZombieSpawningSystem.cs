using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A smarter zombie spawning system that dynamically spawns enemies based on time of day,
/// player proximity, camera visibility, and collision checks.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class SmartZombieSpawningSystem : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Day/Night cycle reference to determine spawn rates")]
    public DayNightCycle dayNightCycle;
    
    [Header("Enemy Type Distribution")]
    [Tooltip("Relative spawn frequency for Toast enemy (0-1)")]
    [Range(0, 1)] public float toastSpawnFrequency = 0.4f;
    
    [Tooltip("Relative spawn frequency for Cone enemy (0-1)")]
    [Range(0, 1)] public float coneSpawnFrequency = 0.3f;
    
    [Tooltip("Relative spawn frequency for Donut enemy (0-1)")]
    [Range(0, 1)] public float donutSpawnFrequency = 0.2f;
    
    [Tooltip("Relative spawn frequency for Corn enemy (0-1)")]
    [Range(0, 1)] public float cornSpawnFrequency = 0.1f;
    
    [Header("Spawn Settings")]
    [Tooltip("Maximum number of enemies to spawn")]
    public int maxEnemies = 30;
    
    [Tooltip("Minimum distance from player to spawn enemies")]
    public float minPlayerDistance = 5f;
    
    [Tooltip("Maximum distance from player to spawn enemies")]
    public float maxPlayerDistance = 30f;
    
    [Tooltip("Use object pooling instead of direct instantiation")]
    public bool useObjectPooling = true;

    [Header("Time-based Spawning")]
    [Tooltip("Spawn rate curve based on time of day (0-1)")]
    public AnimationCurve spawnRateCurve = new AnimationCurve(
        new Keyframe(0.0f, 1.0f),      // Midnight (max spawning)
        new Keyframe(0.2f, 0.8f),      // Early morning (high spawning)
        new Keyframe(0.25f, 0.3f),     // Sunrise (reduced spawning)
        new Keyframe(0.3f, 0.0f),      // Morning (no spawning)
        new Keyframe(0.7f, 0.0f),      // Afternoon (no spawning)
        new Keyframe(0.75f, 0.2f),     // Sunset (starts spawning)
        new Keyframe(0.85f, 0.6f),     // Dusk (increased spawning)
        new Keyframe(1.0f, 1.0f)       // Night (max spawning)
    );
    
    [Tooltip("Base spawning interval in seconds")]
    public float baseSpawnInterval = 5f;
    
    [Tooltip("Minimum spawning interval in seconds")]
    public float minSpawnInterval = 1f;
    
    [Tooltip("Difficulty scaling curve over time (0-1)")]
    public AnimationCurve difficultyCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f),      // Game start (easiest)
        new Keyframe(0.5f, 0.5f),      // Mid-game (medium)
        new Keyframe(1.0f, 1.0f)       // Game end (hardest)
    );
    
    [Tooltip("Zombie health multiplier at maximum difficulty")]
    public float maxHealthMultiplier = 2.0f;
    
    [Tooltip("Zombie damage multiplier at maximum difficulty")]
    public float maxDamageMultiplier = 1.5f;
    
    [Tooltip("Zombie speed multiplier at maximum difficulty")]
    public float maxSpeedMultiplier = 1.3f;

    [Header("Proximity Spawning")]
    [Tooltip("Enable spawning when players are nearby")]
    public bool enableProximitySpawning = true;
    
    [Tooltip("Distance at which proximity spawners activate")]
    public float proximityActivationDistance = 15f;
    
    [Tooltip("Enable heat map spawning focused on player activity")]
    public bool useHeatMapSpawning = true;
    
    [Tooltip("Range of heat map influence")]
    public float heatMapRange = 50f;

    [Header("Off-Camera Spawning")]
    [Tooltip("Ensure enemies spawn outside camera view")]
    public bool ensureOffCameraSpawning = true;
    
    [Tooltip("Margin beyond camera edges for spawning")]
    public float offCameraMargin = 2f;

    [Header("Wandering Zombies")]
    [Tooltip("Enable wandering zombies around the map")]
    public bool enableWanderingZombies = true;
    
    [Tooltip("Maximum number of wandering zombies")]
    public int maxWanderingZombies = 15;
    
    [Tooltip("Interval for wandering zombie spawn checks")]
    public float wanderingSpawnInterval = 20f;
    
    [Tooltip("Group size for wandering zombies (1 = no groups)")]
    [Range(1, 5)]
    public int wanderingGroupSize = 1;

    [Header("Special Events")]
    [Tooltip("Enable boss zombies to spawn occasionally")]
    public bool enableBossZombies = false;
    
    [Tooltip("Chance of a boss zombie spawn (0-1)")]
    [Range(0, 1)]
    public float bossZombieChance = 0.05f;
    
    [Tooltip("Interval between special event checks")]
    public float specialEventInterval = 180f;
    
    [Tooltip("Performance-based spawn limit reduction")]
    public bool enablePerformanceScaling = true;
    
    [Tooltip("Target framerate for performance scaling")]
    public int targetFramerate = 60;

    [Header("Debug Options")]
    public bool showDebugVisuals = false;
    public Color debugColor = Color.red;

    // Private variables
    private List<SmartZombieSpawnPoint> spawnPoints = new List<SmartZombieSpawnPoint>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float nextSpawnTime = 0f;
    private float nextWanderingSpawnTime = 0f;
    private float nextSpecialEventTime = 0f;
    private float gameStartTime = 0f;
    private float currentDifficulty = 0f;
    private BoxCollider2D spawnArea;
    private Camera mainCamera;
    private bool isInitialized = false;
    
    // Heat map tracking for spawn locations
    private Dictionary<Vector2Int, float> heatMap = new Dictionary<Vector2Int, float>();
    private float heatMapUpdateInterval = 5f;
    private float nextHeatMapUpdate = 0f;
    
    // Performance monitoring
    private float[] frameRateBuffer = new float[20];
    private int frameRateBufferIndex = 0;
    private float lastPerformanceCheckTime = 0f;
    private int dynamicEnemyLimit;

    private void Awake()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        spawnArea.isTrigger = true; // Ensure it's a trigger collider
        
        // Find all spawn points in children
        SmartZombieSpawnPoint[] childSpawnPoints = GetComponentsInChildren<SmartZombieSpawnPoint>();
        spawnPoints.AddRange(childSpawnPoints);
        
        // If no daynight cycle is assigned, try to find one
        if (dayNightCycle == null)
        {
            dayNightCycle = FindObjectOfType<DayNightCycle>();
        }
        
        // Initialize buffer for performance monitoring
        for (int i = 0; i < frameRateBuffer.Length; i++)
        {
            frameRateBuffer[i] = 60f;
        }
        
        dynamicEnemyLimit = maxEnemies;
    }

    private void Start()
    {
        // Record game start time for difficulty scaling
        gameStartTime = Time.time;
        
        // Wait a frame before initializing to ensure everything else is set up
        StartCoroutine(InitializeNextFrame());
    }

    private IEnumerator InitializeNextFrame()
    {
        yield return null;
        
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("SmartZombieSpawningSystem: Main camera not found!");
        }
        
        // Check for day night cycle
        if (dayNightCycle == null)
        {
            Debug.LogError("SmartZombieSpawningSystem: DayNightCycle not found!");
        }
        
        // Verify we can access the asset system
        if (ASSETS.Players == null)
        {
            Debug.LogError("SmartZombieSpawningSystem: Cannot access ASSETS.Players!");
        }
        
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        // Update performance monitoring
        UpdatePerformanceTracking();
        
        // Clean up any destroyed enemies
        CleanupDeadEnemies();
        
        // Update current difficulty based on game time
        UpdateDifficulty();
        
        // Update heat map if enabled
        if (useHeatMapSpawning && Time.time >= nextHeatMapUpdate)
        {
            UpdateHeatMap();
            nextHeatMapUpdate = Time.time + heatMapUpdateInterval;
        }
        
        // Get current spawn rate based on time of day
        float currentTime = Time.time;
        float spawnRate = GetCurrentSpawnRate();
        
        // Skip spawning if spawn rate is zero or we've reached max enemies
        if (spawnRate <= 0 || spawnedEnemies.Count >= dynamicEnemyLimit) return;
        
        // Calculate actual spawn interval based on spawn rate
        float actualSpawnInterval = Mathf.Lerp(baseSpawnInterval, minSpawnInterval, spawnRate);
        
        // Regular spawn points
        if (currentTime >= nextSpawnTime)
        {
            TrySpawnFromSpawnPoints();
            nextSpawnTime = currentTime + actualSpawnInterval;
        }
        
        // Wandering zombies
        if (enableWanderingZombies && currentTime >= nextWanderingSpawnTime)
        {
            TrySpawnWanderingZombie();
            nextWanderingSpawnTime = currentTime + wanderingSpawnInterval;
        }
        
        // Special events
        if (enableBossZombies && currentTime >= nextSpecialEventTime)
        {
            TryTriggerSpecialEvent();
            nextSpecialEventTime = currentTime + specialEventInterval;
        }
    }

    private void UpdatePerformanceTracking()
    {
        if (!enablePerformanceScaling) return;
        
        // Update frame rate buffer
        frameRateBuffer[frameRateBufferIndex] = 1f / Time.deltaTime;
        frameRateBufferIndex = (frameRateBufferIndex + 1) % frameRateBuffer.Length;
        
        // Check performance less frequently
        if (Time.time - lastPerformanceCheckTime < 1f) return;
        
        lastPerformanceCheckTime = Time.time;
        
        // Calculate average framerate
        float sum = 0;
        foreach (var rate in frameRateBuffer)
        {
            sum += rate;
        }
        float avgFrameRate = sum / frameRateBuffer.Length;
        
        // Adjust enemy limit based on frame rate
        float performanceRatio = Mathf.Clamp01(avgFrameRate / targetFramerate);
        
        // Apply a non-linear curve to avoid drastic reductions at slightly lower framerates
        performanceRatio = Mathf.Pow(performanceRatio, 0.5f);
        
        // Update dynamic limit with some smoothing
        dynamicEnemyLimit = Mathf.RoundToInt(Mathf.Lerp(dynamicEnemyLimit, 
            Mathf.CeilToInt(maxEnemies * performanceRatio), 0.2f));
        
        // Ensure at least a few enemies can spawn
        dynamicEnemyLimit = Mathf.Max(5, dynamicEnemyLimit);
        
        if (showDebugVisuals && dynamicEnemyLimit < maxEnemies)
        {
            Debug.Log($"Performance scaling: {avgFrameRate:F1} FPS, limit: {dynamicEnemyLimit}/{maxEnemies} enemies");
        }
    }

    private void UpdateDifficulty()
    {
        // Calculate how far we are through the game (assuming a 20-minute session)
        float gameProgress = Mathf.Clamp01((Time.time - gameStartTime) / (20f * 60f));
        
        // Update current difficulty based on game progress
        currentDifficulty = difficultyCurve.Evaluate(gameProgress);
    }

    private float GetCurrentSpawnRate()
    {
        if (dayNightCycle == null) return 0;
        
        // Get time of day (0-1 value where 0 is midnight, 0.5 is noon)
        float timeOfDay = dayNightCycle.GetCurrentDayFraction();
        
        // Get spawn rate from animation curve based on time of day
        return spawnRateCurve.Evaluate(timeOfDay);
    }

    private void TrySpawnFromSpawnPoints()
    {
        if (spawnPoints.Count == 0) return;
        
        // Get active spawn points based on their criteria (proximity, etc.)
        List<SmartZombieSpawnPoint> activePoints = GetActiveSpawnPoints();
        
        if (activePoints.Count == 0) return;
        
        // Select a random spawn point from active ones
        SmartZombieSpawnPoint selectedPoint = activePoints[UnityEngine.Random.Range(0, activePoints.Count)];
        
        // Get an appropriate enemy prefab
        GameObject enemyPrefab = GetRandomEnemyPrefab();
        
        if (enemyPrefab == null)
        {
            Debug.LogWarning("No valid enemy prefab available!");
            return;
        }
        
        // Try to spawn at that point
        GameObject enemy = selectedPoint.TrySpawn(
            new GameObject[] { enemyPrefab }, 
            ensureOffCameraSpawning, 
            mainCamera, 
            offCameraMargin
        );
        
        if (enemy != null)
        {
            ConfigureNewEnemy(enemy);
            spawnedEnemies.Add(enemy);
        }
    }

    private List<SmartZombieSpawnPoint> GetActiveSpawnPoints()
    {
        List<SmartZombieSpawnPoint> activePoints = new List<SmartZombieSpawnPoint>();
        
        foreach (var point in spawnPoints)
        {
            // Skip disabled spawn points
            if (!point.gameObject.activeInHierarchy || !point.enabled) continue;
            
            // Check if proximity-based and if so, check player distance
            if (point.isProximityBased && enableProximitySpawning)
            {
                if (Players.I != null && Players.I.GetPlayers().Count > 0)
                {
                    bool isPlayerNearby = false;
                    
                    foreach (var player in Players.I.GetPlayers())
                    {
                        if (player == null || !player.gameObject.activeInHierarchy) continue;
                        
                        float distance = Vector2.Distance(point.transform.position, player.transform.position);
                        if (distance <= proximityActivationDistance)
                        {
                            isPlayerNearby = true;
                            break;
                        }
                    }
                    
                    if (!isPlayerNearby)
                    {
                        continue; // Skip this point if it's proximity-based but no player is nearby
                    }
                }
            }
            
            // Include heat map influence if enabled
            if (useHeatMapSpawning)
            {
                Vector2Int gridPos = WorldToGrid(point.transform.position);
                float heatValue = GetHeatMapValue(gridPos);
                
                // Higher heat value = higher chance of selecting this point
                if (heatValue < 0.2f && UnityEngine.Random.value > heatValue * 2f)
                {
                    continue; // Skip this point based on heat map (lower heat = higher chance to skip)
                }
            }
            
            activePoints.Add(point);
        }
        
        return activePoints;
    }

    private void TrySpawnWanderingZombie()
    {
        if (spawnedEnemies.Count >= dynamicEnemyLimit || 
            spawnedEnemies.Count >= maxWanderingZombies) return;
        
        if (Players.I == null || Players.I.GetPlayers().Count == 0) return;
        
        // Get a random player to use as a reference point
        Player randomPlayer = Players.I.GetPlayers()[UnityEngine.Random.Range(0, Players.I.GetPlayers().Count)];
        if (randomPlayer == null) return;
        
        // Determine the group size for this spawn
        int groupSize = UnityEngine.Random.Range(1, wanderingGroupSize + 1);
        groupSize = Mathf.Min(groupSize, dynamicEnemyLimit - spawnedEnemies.Count);
        
        if (groupSize <= 0) return;
        
        // For groups, find a single valid position first
        Vector2 spawnPosition = GetValidWanderingSpawnPosition(randomPlayer.transform.position);
        
        if (spawnPosition != Vector2.zero)
        {
            // Get an appropriate enemy prefab (same type for the whole group)
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            
            if (enemyPrefab == null) return;
            
            // Spawn the group
            for (int i = 0; i < groupSize; i++)
            {
                // Add some variation to positions for groups
                Vector2 offsetPosition = spawnPosition;
                if (i > 0)
                {
                    offsetPosition += new Vector2(
                        UnityEngine.Random.Range(-2f, 2f),
                        UnityEngine.Random.Range(-2f, 2f)
                    );
                    
                    // Verify it's still valid
                    if (!IsPositionValid(offsetPosition)) continue;
                }
                
                // Spawn the enemy
                GameObject enemy;
                if (useObjectPooling)
                {
                    enemy = ObjectMaker.Make(enemyPrefab, offsetPosition);
                }
                else
                {
                    enemy = Instantiate(enemyPrefab, offsetPosition, Quaternion.identity);
                }
                
                if (enemy != null)
                {
                    ConfigureNewEnemy(enemy);
                    spawnedEnemies.Add(enemy);
                }
            }
            
            // Log for debugging
            if (showDebugVisuals)
            {
                Debug.Log($"Spawned {groupSize} wandering zombie(s) at {spawnPosition}");
                Debug.DrawLine(randomPlayer.transform.position, spawnPosition, Color.red, 1f);
            }
        }
    }

    private void TryTriggerSpecialEvent()
    {
        if (!enableBossZombies) return;
        if (UnityEngine.Random.value > bossZombieChance) return;
        
        // Only spawn boss if we're below 80% of our enemy limit
        if (spawnedEnemies.Count > dynamicEnemyLimit * 0.8f) return;
        
        if (Players.I == null || Players.I.GetPlayers().Count == 0) return;
        
        // Get a random player to use as a reference point
        Player randomPlayer = Players.I.GetPlayers()[UnityEngine.Random.Range(0, Players.I.GetPlayers().Count)];
        if (randomPlayer == null) return;
        
        // Get a valid spawn position
        Vector2 spawnPosition = GetValidWanderingSpawnPosition(randomPlayer.transform.position);
        
        if (spawnPosition != Vector2.zero)
        {
            // Prefer Donut or Corn as a "boss" - they're bigger enemies
            GameObject bossEnemyPrefab = (UnityEngine.Random.value > 0.5f) 
                ? ASSETS.Players.DonutEnemyPrefab 
                : ASSETS.Players.CornEnemyPrefab;
                
            if (bossEnemyPrefab == null) return;
            
            // Spawn the boss enemy
            GameObject enemy;
            if (useObjectPooling)
            {
                enemy = ObjectMaker.Make(bossEnemyPrefab, spawnPosition);
            }
            else
            {
                enemy = Instantiate(bossEnemyPrefab, spawnPosition, Quaternion.identity);
            }
            
            if (enemy != null)
            {
                // Configure as a boss (extra health, damage, etc.)
                ConfigureNewEnemy(enemy, true);
                spawnedEnemies.Add(enemy);
                
                // Log for debugging
                if (showDebugVisuals)
                {
                    Debug.Log($"Spawned a BOSS zombie at {spawnPosition}!");
                    Debug.DrawLine(randomPlayer.transform.position, spawnPosition, Color.magenta, 3f);
                }
            }
        }
    }

    private void ConfigureNewEnemy(GameObject enemy, bool isBoss = false)
    {
        // Set up the Life component
        Life life = enemy.GetComponent<Life>();
        if (life != null)
        {
            // Apply difficulty scaling
            float healthMultiplier = Mathf.Lerp(1.0f, maxHealthMultiplier, currentDifficulty);
            float damageMultiplier = Mathf.Lerp(1.0f, maxDamageMultiplier, currentDifficulty);
            float speedMultiplier = Mathf.Lerp(1.0f, maxSpeedMultiplier, currentDifficulty);
            
            // Apply boss multipliers if applicable
            if (isBoss)
            {
                healthMultiplier *= 2.5f;
                damageMultiplier *= 1.5f;
                speedMultiplier *= 0.8f; // Bosses are slower but tougher
                
                // Make bosses visually distinct
                Transform visual = enemy.transform.Find("Visual");
                if (visual != null)
                {
                    visual.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                }
                
                // Apply a slight tint
                SpriteRenderer[] renderers = enemy.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in renderers)
                {
                    renderer.color = new Color(1.0f, 0.8f, 0.8f);
                }
            }
            
            // Apply the multipliers - need to use appropriate methods since the Life properties are read-only
            life.SetExtraMaxHealth(life.HealthMax * (healthMultiplier - 1.0f));
            
            // Apply damage multiplier
            life.SetExtraMaxDamage(life.AttackDamage * (damageMultiplier - 1.0f));
            
            // Apply speed multiplier
            life.SetExtraMaxSpeed(life.MoveSpeed * (speedMultiplier - 1.0f));
            
            // Reset health to full after modifying max
            life.AddHealth(life.HealthMax);
            
            // Set the player reference to enemy player
            life.SetPlayer(Players.EnemyPlayer);
        }
        
        // Register with EnemyManager using the static method
        EnemyManager.CollectEnemy(enemy);
    }

    private Vector2 GetValidWanderingSpawnPosition(Vector3 playerPosition)
    {
        // Try to find a valid spawn position up to 10 times
        for (int i = 0; i < 10; i++)
        {
            // Calculate random distance between min and max player distance
            float distance = UnityEngine.Random.Range(minPlayerDistance, maxPlayerDistance);
            
            // Calculate random angle around player
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            // Calculate potential spawn position
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector2 potentialPosition = (Vector2)playerPosition + offset;
            
            if (IsPositionValid(potentialPosition))
            {
                return potentialPosition;
            }
        }
        
        // No valid position found after 10 attempts
        return Vector2.zero;
    }
    
    private bool IsPositionValid(Vector2 position)
    {
        // Check if position is within spawn area
        if (spawnArea != null && !spawnArea.bounds.Contains(position))
        {
            return false;
        }
        
        // Check if position is not too close to any player
        foreach (var player in Players.I.GetPlayers())
        {
            if (player == null) continue;
            
            float playerDist = Vector2.Distance(position, player.transform.position);
            if (playerDist < minPlayerDistance)
            {
                return false;
            }
        }
        
        // Check for obstacles at the potential spawn position
        if (Physics2D.OverlapCircle(position, 0.5f, ASSETS.LevelAssets.BuildingLayer))
        {
            return false;
        }
        
        // Check for landable layer to ensure enemy is on walkable surface
        if (!Physics2D.OverlapCircle(position, 0.5f, ASSETS.LevelAssets.LandableLayer))
        {
            return false;
        }
        
        // Check if position is visible in camera (if required)
        if (ensureOffCameraSpawning && mainCamera != null)
        {
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(position);
            if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
                viewportPoint.y >= 0 && viewportPoint.y <= 1 && 
                viewportPoint.z > 0) // In camera view
            {
                return false;
            }
        }
        
        // All checks passed
        return true;
    }

    private void CleanupDeadEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }
    
    private GameObject GetRandomEnemyPrefab()
    {
        // Calculate the total frequency
        float totalFrequency = toastSpawnFrequency + coneSpawnFrequency + 
                               donutSpawnFrequency + cornSpawnFrequency;
        
        if (totalFrequency <= 0)
        {
            Debug.LogWarning("All enemy spawn frequencies are zero!");
            return null;
        }
        
        // Get a random value
        float random = UnityEngine.Random.Range(0, totalFrequency);
        
        // Determine which enemy to spawn based on the random value
        if (random < toastSpawnFrequency)
        {
            return ASSETS.Players.ToastEnemyPrefab;
        }
        random -= toastSpawnFrequency;
        
        if (random < coneSpawnFrequency)
        {
            return ASSETS.Players.ConeEnemyPrefab;
        }
        random -= coneSpawnFrequency;
        
        if (random < donutSpawnFrequency)
        {
            return ASSETS.Players.DonutEnemyPrefab;
        }
        
        // If we get here, it's Corn (or if all else fails)
        return ASSETS.Players.CornEnemyPrefab;
    }
    
    #region Heat Map System
    
    private Vector2Int WorldToGrid(Vector2 worldPos)
    {
        // Convert world position to grid coordinates (10 unit grid size)
        int gridX = Mathf.FloorToInt(worldPos.x / 10f);
        int gridY = Mathf.FloorToInt(worldPos.y / 10f);
        return new Vector2Int(gridX, gridY);
    }
    
    private void UpdateHeatMap()
    {
        // Cool down existing heat
        List<Vector2Int> cellsToRemove = new List<Vector2Int>();
        foreach (var cell in heatMap.Keys)
        {
            heatMap[cell] *= 0.9f; // Reduce heat by 10%
            
            // Remove cells that have cooled down completely
            if (heatMap[cell] < 0.01f)
            {
                cellsToRemove.Add(cell);
            }
        }
        
        foreach (var cell in cellsToRemove)
        {
            heatMap.Remove(cell);
        }
        
        // Add heat around players
        if (Players.I != null)
        {
            foreach (var player in Players.I.GetPlayers())
            {
                if (player == null || !player.gameObject.activeInHierarchy) continue;
                
                // Add heat at player position
                Vector2Int gridPos = WorldToGrid(player.transform.position);
                AddHeatToCell(gridPos, 0.5f);
                
                // Add heat to surrounding cells with falloff
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue; // Skip center cell
                        
                        Vector2Int neighborCell = gridPos + new Vector2Int(x, y);
                        AddHeatToCell(neighborCell, 0.2f);
                    }
                }
            }
        }
    }
    
    private void AddHeatToCell(Vector2Int cell, float amount)
    {
        if (!heatMap.ContainsKey(cell))
        {
            heatMap[cell] = 0;
        }
        
        heatMap[cell] = Mathf.Min(heatMap[cell] + amount, 1f);
    }
    
    private float GetHeatMapValue(Vector2Int cell)
    {
        if (heatMap.TryGetValue(cell, out float value))
        {
            return value;
        }
        return 0f;
    }
    
    #endregion

    // Draw debug gizmos for the spawn area
    private void OnDrawGizmos()
    {
        if (!showDebugVisuals) return;
        
        if (spawnArea != null)
        {
            Gizmos.color = debugColor;
            Vector3 center = spawnArea.bounds.center;
            Vector3 size = spawnArea.bounds.size;
            Gizmos.DrawWireCube(center, size);
        }
        
        // Draw active spawn points
        if (Application.isPlaying && isInitialized)
        {
            // Show spawn rate
            Gizmos.color = Color.yellow;
            float rate = GetCurrentSpawnRate();
            Vector3 infoPos = transform.position + Vector3.up * 2;
            
            // Draw heat map if enabled
            if (useHeatMapSpawning && Application.isPlaying)
            {
                foreach (var cellEntry in heatMap)
                {
                    Vector2Int cell = cellEntry.Key;
                    float heat = cellEntry.Value;
                    
                    // Skip low heat cells
                    if (heat < 0.1f) continue;
                    
                    // Convert back to world position
                    Vector3 worldPos = new Vector3(
                        cell.x * 10f + 5f, 
                        cell.y * 10f + 5f, 
                        0f
                    );
                    
                    // Draw heat as colored cube
                    Gizmos.color = new Color(1f, 0f, 0f, heat * 0.3f);
                    Gizmos.DrawCube(worldPos, new Vector3(9f, 9f, 0.1f));
                }
            }
        }
    }
}