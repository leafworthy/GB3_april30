using System.Collections.Generic;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// Simplified zombie spawning system with time-based difficulty curve.
    /// No longer depends on day/night cycle or complex camera detection.
    /// </summary>
    public class SmartZombieSpawningSystem : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private float baseSpawnInterval = 3f;
        [SerializeField] private float minSpawnInterval = 0.5f;
        [SerializeField] private int maxActiveZombies = 20;
        [SerializeField] private float difficultyRampTime = 300f; // 5 minutes to reach max difficulty

        [Header("Enemy Types")]
        [SerializeField, Range(0, 1)] private float toastSpawnFrequency = 0.4f;
        [SerializeField] private float toastUnlockTime = 0f; // Available from start
        [SerializeField, Range(0, 1)] private float coneSpawnFrequency = 0.3f;
        [SerializeField] private float coneUnlockTime = 30f; // Unlocks after 30 seconds
        [SerializeField, Range(0, 1)] private float donutSpawnFrequency = 0.2f;
        [SerializeField] private float donutUnlockTime = 90f; // Unlocks after 1.5 minutes
        [SerializeField, Range(0, 1)] private float cornSpawnFrequency = 0.1f;
        [SerializeField] private float cornUnlockTime = 180f; // Unlocks after 3 minutes

        [Header("Difficulty Scaling")]
        [SerializeField] private AnimationCurve difficultyOverTime = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve spawnRateOverTime = AnimationCurve.EaseInOut(0f, 1f, 1f, 3f);
        [SerializeField] private float maxHealthMultiplier = 2.0f;
        [SerializeField] private float maxDamageMultiplier = 1.5f;
        [SerializeField] private float maxSpeedMultiplier = 1.3f;

        [Header("Boss Events")]
        [SerializeField] private bool enableBossZombies = false;
        [SerializeField, Range(0, 1)] private float bossZombieChance = 0.05f;
        [SerializeField] private float bossEventInterval = 180f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;

        private List<SmartZombieSpawnPoint> spawnPoints = new List<SmartZombieSpawnPoint>();
        private List<GameObject> spawnedEnemies = new List<GameObject>();
        private float gameStartTime;
        private float lastSpawnTime;
        private float lastBossEventTime;
        private int activeZombieCount = 0;

        public static SmartZombieSpawningSystem Instance { get; private set; }

        // Events
        public System.Action<int> OnZombieCountChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                gameStartTime = Time.time;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            FindAllSpawnPoints();

            if (enableDebugLogs)
            {
                Debug.Log($"SmartZombieSpawningSystem initialized with {spawnPoints.Count} spawn points");
            }
        }

        void Update()
        {
            CleanupDeadEnemies();

            if (ShouldAttemptSpawn())
            {
                AttemptSpawn();
            }

            if (enableBossZombies && ShouldAttemptBossSpawn())
            {
                AttemptBossSpawn();
            }
        }

        private void FindAllSpawnPoints()
        {
            spawnPoints.Clear();
            spawnPoints.AddRange(GetComponentsInChildren<SmartZombieSpawnPoint>());

            if (spawnPoints.Count == 0)
            {
                // Fallback: find all spawn points in scene
                SmartZombieSpawnPoint[] allSpawnPoints = FindObjectsOfType<SmartZombieSpawnPoint>();
                spawnPoints.AddRange(allSpawnPoints);
            }
        }

        private bool ShouldAttemptSpawn()
        {
            if (activeZombieCount >= maxActiveZombies)
                return false;

            float currentSpawnInterval = GetCurrentSpawnInterval();
            return Time.time - lastSpawnTime >= currentSpawnInterval;
        }

        private bool ShouldAttemptBossSpawn()
        {
            return Time.time - lastBossEventTime >= bossEventInterval;
        }

        private float GetCurrentSpawnInterval()
        {
            float gameTime = Time.time - gameStartTime;
            float difficultyProgress = Mathf.Clamp01(gameTime / difficultyRampTime);
            float spawnRateMultiplier = spawnRateOverTime.Evaluate(difficultyProgress);

            return Mathf.Lerp(baseSpawnInterval, minSpawnInterval, difficultyProgress) / spawnRateMultiplier;
        }

        private void AttemptSpawn()
        {
            if (spawnPoints.Count == 0)
                return;

            // Choose random spawn point
            SmartZombieSpawnPoint chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject enemyPrefab = GetRandomEnemyPrefab();

            if (enemyPrefab == null)
                return;

            // Try to spawn at that point
            Vector3 spawnPosition = chosenSpawnPoint.transform.position;
            GameObject enemy = ObjectMaker.I.Make(enemyPrefab, spawnPosition);

            if (enemy != null)
            {
                ConfigureNewEnemy(enemy, false);
                spawnedEnemies.Add(enemy);
                lastSpawnTime = Time.time;
                activeZombieCount++;
                OnZombieCountChanged?.Invoke(activeZombieCount);

                if (enableDebugLogs)
                {
                    Debug.Log($"Zombie spawned at {chosenSpawnPoint.name}. Active count: {activeZombieCount}");
                }
            }
        }

        private void AttemptBossSpawn()
        {
            lastBossEventTime = Time.time;

            if (Random.value > bossZombieChance)
                return;

            if (spawnPoints.Count == 0)
                return;

            // Choose random spawn point
            SmartZombieSpawnPoint chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            // Prefer Donut or Corn as boss enemies
            GameObject bossEnemyPrefab = Random.value > 0.5f ? ASSETS.Players.DonutEnemyPrefab : ASSETS.Players.CornEnemyPrefab;

            if (bossEnemyPrefab == null)
                return;

            Vector3 spawnPosition = chosenSpawnPoint.transform.position;
            GameObject enemy = ObjectMaker.I.Make(bossEnemyPrefab, spawnPosition);

            if (enemy != null)
            {
                ConfigureNewEnemy(enemy, true);
                spawnedEnemies.Add(enemy);
                activeZombieCount++;
                OnZombieCountChanged?.Invoke(activeZombieCount);

                if (enableDebugLogs)
                {
                    Debug.Log("Boss spawned!");
                }
            }
        }

        private void ConfigureNewEnemy(GameObject enemy, bool isBoss = false)
        {
            // Set up the Life component
            var life = enemy.GetComponent<Life>();
            if (life != null)
            {
                life.SetPlayer(Players.EnemyPlayer);

                // Calculate difficulty multipliers
                float gameTime = Time.time - gameStartTime;
                float difficultyProgress = Mathf.Clamp01(gameTime / difficultyRampTime);
                float currentDifficulty = difficultyOverTime.Evaluate(difficultyProgress);

                var healthMultiplier = Mathf.Lerp(1.0f, maxHealthMultiplier, currentDifficulty);
                var damageMultiplier = Mathf.Lerp(1.0f, maxDamageMultiplier, currentDifficulty);
                var speedMultiplier = Mathf.Lerp(1.0f, maxSpeedMultiplier, currentDifficulty);

                // Apply boss multipliers if applicable
                if (isBoss)
                {
                    healthMultiplier *= 2.5f;
                    damageMultiplier *= 1.5f;
                    speedMultiplier *= 0.8f; // Bosses are slower but tougher

                    enemy.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

                    // Apply a slight tint
                    var renderers = enemy.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var renderer in renderers)
                    {
                        renderer.color = new Color(1.0f, 0.8f, 0.8f);
                    }
                }

                // Apply the multipliers
                life.SetExtraMaxHealthFactor(healthMultiplier);
                life.SetExtraMaxDamageFactor(damageMultiplier);
                life.SetExtraMaxSpeedFactor(speedMultiplier);

                // Reset health to full after modifying max
                life.AddHealth(life.HealthMax);
            }

            // Set player reference for all components that need it
            foreach (var component in enemy.GetComponents<INeedPlayer>())
            {
                component.SetPlayer(Players.EnemyPlayer);
            }

            // Register with EnemyManager
            EnemyManager.I.CollectEnemy(enemy);
        }

        private void CleanupDeadEnemies()
        {
            int originalCount = spawnedEnemies.Count;
            spawnedEnemies.RemoveAll(enemy => enemy == null);

            int removedCount = originalCount - spawnedEnemies.Count;
            if (removedCount > 0)
            {
                activeZombieCount = Mathf.Max(0, activeZombieCount - removedCount);
                OnZombieCountChanged?.Invoke(activeZombieCount);
            }
        }

        private GameObject GetRandomEnemyPrefab()
        {
            float gameTime = Time.time - gameStartTime;
            
            // Build list of available enemies based on unlock times
            var availableEnemies = new System.Collections.Generic.List<(GameObject prefab, float frequency)>();
            
            // Check each enemy type and add if unlocked
            if (gameTime >= toastUnlockTime && ASSETS.Players.ToastEnemyPrefab != null)
            {
                availableEnemies.Add((ASSETS.Players.ToastEnemyPrefab, toastSpawnFrequency));
            }
            
            if (gameTime >= coneUnlockTime && ASSETS.Players.ConeEnemyPrefab != null)
            {
                availableEnemies.Add((ASSETS.Players.ConeEnemyPrefab, coneSpawnFrequency));
            }
            
            if (gameTime >= donutUnlockTime && ASSETS.Players.DonutEnemyPrefab != null)
            {
                availableEnemies.Add((ASSETS.Players.DonutEnemyPrefab, donutSpawnFrequency));
            }
            
            if (gameTime >= cornUnlockTime && ASSETS.Players.CornEnemyPrefab != null)
            {
                availableEnemies.Add((ASSETS.Players.CornEnemyPrefab, cornSpawnFrequency));
            }
            
            // If no enemies are available yet, return null
            if (availableEnemies.Count == 0)
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"No enemies unlocked yet! Game time: {gameTime:F1}s");
                }
                return null;
            }
            
            // Calculate total frequency of available enemies
            float totalFrequency = 0f;
            foreach (var enemy in availableEnemies)
            {
                totalFrequency += enemy.frequency;
            }
            
            if (totalFrequency <= 0)
            {
                Debug.LogWarning("All available enemy spawn frequencies are zero!");
                // Return first available enemy as fallback
                return availableEnemies[0].prefab;
            }
            
            // Select random enemy based on weighted probability
            float random = Random.Range(0, totalFrequency);
            float currentWeight = 0f;
            
            foreach (var enemy in availableEnemies)
            {
                currentWeight += enemy.frequency;
                if (random <= currentWeight)
                {
                    return enemy.prefab;
                }
            }
            
            // Fallback: return last available enemy
            return availableEnemies[availableEnemies.Count - 1].prefab;
        }

        // Public methods for external control
        public void SetSpawnRate(float multiplier)
        {
            baseSpawnInterval = baseSpawnInterval / multiplier;
            minSpawnInterval = minSpawnInterval / multiplier;
        }

        public void SetMaxZombies(int maxCount)
        {
            maxActiveZombies = maxCount;
        }

        public float GetCurrentDifficulty()
        {
            float gameTime = Time.time - gameStartTime;
            float difficultyProgress = Mathf.Clamp01(gameTime / difficultyRampTime);
            return difficultyOverTime.Evaluate(difficultyProgress);
        }

        public int GetActiveZombieCount()
        {
            return activeZombieCount;
        }

        public int GetMaxZombieCount()
        {
            return maxActiveZombies;
        }
        
        public string GetUnlockedEnemiesInfo()
        {
            float gameTime = Time.time - gameStartTime;
            var unlockedEnemies = new System.Collections.Generic.List<string>();
            
            if (gameTime >= toastUnlockTime) unlockedEnemies.Add("Toast");
            if (gameTime >= coneUnlockTime) unlockedEnemies.Add("Cone");
            if (gameTime >= donutUnlockTime) unlockedEnemies.Add("Donut");
            if (gameTime >= cornUnlockTime) unlockedEnemies.Add("Corn");
            
            if (unlockedEnemies.Count == 0)
                return "None unlocked yet";
                
            return string.Join(", ", unlockedEnemies);
        }

        // Debug visualization
        void OnGUI()
        {
            if (!enableDebugLogs)
                return;
                
            float gameTime = Time.time - gameStartTime;

            GUILayout.BeginArea(new Rect(10, 10, 350, 250));
            GUILayout.Label($"Active Zombies: {activeZombieCount}/{maxActiveZombies}");
            GUILayout.Label($"Current Difficulty: {GetCurrentDifficulty():F2}");
            GUILayout.Label($"Spawn Interval: {GetCurrentSpawnInterval():F2}s");
            GUILayout.Label($"Game Time: {gameTime:F1}s");
            GUILayout.Label($"Available Spawn Points: {spawnPoints.Count}");
            GUILayout.Label($"Unlocked Enemies: {GetUnlockedEnemiesInfo()}");
            
            // Show next unlock
            string nextUnlock = GetNextUnlockInfo(gameTime);
            if (!string.IsNullOrEmpty(nextUnlock))
            {
                GUILayout.Label($"Next Unlock: {nextUnlock}");
            }
            
            GUILayout.EndArea();
        }
        
        private string GetNextUnlockInfo(float currentGameTime)
        {
            var upcomingUnlocks = new System.Collections.Generic.List<(string name, float time)>();
            
            if (currentGameTime < toastUnlockTime) upcomingUnlocks.Add(("Toast", toastUnlockTime));
            if (currentGameTime < coneUnlockTime) upcomingUnlocks.Add(("Cone", coneUnlockTime));
            if (currentGameTime < donutUnlockTime) upcomingUnlocks.Add(("Donut", donutUnlockTime));
            if (currentGameTime < cornUnlockTime) upcomingUnlocks.Add(("Corn", cornUnlockTime));
            
            if (upcomingUnlocks.Count == 0)
                return "";
                
            // Sort by time and get the next one
            upcomingUnlocks.Sort((a, b) => a.time.CompareTo(b.time));
            var nextUnlock = upcomingUnlocks[0];
            float timeRemaining = nextUnlock.time - currentGameTime;
            
            return $"{nextUnlock.name} in {timeRemaining:F1}s";
        }
    }
}
