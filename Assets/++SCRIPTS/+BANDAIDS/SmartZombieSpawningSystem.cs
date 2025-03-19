using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A smarter zombie spawning system that dynamically spawns enemies based on time of day,
/// player proximity, camera visibility, and collision checks.
/// </summary>
public class SmartZombieSpawningSystem : MonoBehaviour
{
	#region Enemies

	[Header("Enemy Type Distribution"), Tooltip("Relative spawn frequency for Toast enemy (0-1)"), Range(0, 1)]
	public float toastSpawnFrequency = 0.4f;

	[Tooltip("Relative spawn frequency for Cone enemy (0-1)"), Range(0, 1)]
	public float coneSpawnFrequency = 0.3f;

	[Tooltip("Relative spawn frequency for Donut enemy (0-1)"), Range(0, 1)]
	public float donutSpawnFrequency = 0.2f;

	[Tooltip("Relative spawn frequency for Corn enemy (0-1)"), Range(0, 1)]
	public float cornSpawnFrequency = 0.1f;
	[Tooltip("Difficulty scaling curve over time (0-1)")]
	public AnimationCurve difficultyCurve = new(new Keyframe(0.0f, 0.0f), // Game start (easiest)
		new Keyframe(0.5f, 0.5f), // Mid-game (medium)
		new Keyframe(1.0f, 1.0f) // Game end (hardest)
	);

	[Tooltip("Zombie health multiplier at maximum difficulty")]
	public float maxHealthMultiplier = 2.0f;

	[Tooltip("Zombie damage multiplier at maximum difficulty")]
	public float maxDamageMultiplier = 1.5f;

	[Tooltip("Zombie speed multiplier at maximum difficulty")]
	public float maxSpeedMultiplier = 1.3f;

	#endregion

	#region Spawning Settings

	[Header("Spawn Settings"), Tooltip("Maximum number of enemies to spawn")]
	public int maxEnemies = 30;

	[Tooltip("Minimum distance from player to spawn enemies")]
	public float minPlayerDistance = 5f;

	[Tooltip("Maximum distance from player to spawn enemies")]
	public float maxPlayerDistance = 30f;

	[Tooltip("Use object pooling instead of direct instantiation")]
	public bool useObjectPooling = true;

	[Header("Time-based Spawning"), Tooltip("Spawn rate curve based on time of day (0-1)")]
	public AnimationCurve spawnRateCurve = new(new Keyframe(0.0f, 1.0f), // Midnight (max spawning)
		new Keyframe(0.2f, 0.8f), // Early morning (high spawning)
		new Keyframe(0.25f, 0.3f), // Sunrise (reduced spawning)
		new Keyframe(0.3f, 0.0f), // Morning (no spawning)
		new Keyframe(0.7f, 0.0f), // Afternoon (no spawning)
		new Keyframe(0.75f, 0.2f), // Sunset (starts spawning)
		new Keyframe(0.85f, 0.6f), // Dusk (increased spawning)
		new Keyframe(1.0f, 1.0f) // Night (max spawning)
	);

	[Tooltip("Base spawning interval in seconds")]
	public float baseSpawnInterval = 5f;

	[Tooltip("Minimum spawning interval in seconds")]
	public float minSpawnInterval = 1f;

	[Header("Proximity Spawning"), Tooltip("Enable spawning when players are nearby")]
	public bool enableProximitySpawning = true;

	[Tooltip("Distance at which proximity spawners activate")]
	public float proximityActivationDistance = 15f;

	[Tooltip("Enable heat map spawning focused on player activity")]
	public bool useHeatMapSpawning = true;

	[Tooltip("Range of heat map influence")]
	public float heatMapRange = 50f;

	[Header("Off-Camera Spawning"), Tooltip("Ensure enemies spawn outside camera view")]
	public bool ensureOffCameraSpawning = true;

	[Tooltip("Margin beyond camera edges for spawning")]
	public float offCameraMargin = 2f;

	#endregion

	#region Wandering Zombies

	[Header("Wandering Zombies"), Tooltip("Enable wandering zombies around the map")]
	public bool enableWanderingZombies = true;

	[Tooltip("Maximum number of wandering zombies")]
	public int maxWanderingZombies = 15;

	[Tooltip("Interval for wandering zombie spawn checks")]
	public float wanderingSpawnInterval = 20f;

	[Tooltip("Group size for wandering zombies (1 = no groups)"), Range(1, 5)]
	public int wanderingGroupSize = 1;

	#endregion

	#region Bosses

	[Header("Special Events"), Tooltip("Enable boss zombies to spawn occasionally")]
	public bool enableBossZombies;

	[Tooltip("Chance of a boss zombie spawn (0-1)"), Range(0, 1)]
	public float bossZombieChance = 0.05f;

	[Tooltip("Interval between special event checks")]
	public float bossEventInterval = 180f;

	#endregion

	
	// Private variables
	private List<SmartZombieSpawnPoint> spawnPoints = new();
	private List<GameObject> spawnedEnemies = new();
	private float nextSpawnTime;
	private float nextWanderingSpawnTime;
	private float nextSpecialEventTime;
	private float gameStartTime;
	private float currentDifficulty;
	private PolygonCollider2D spawnArea;
	private Camera mainCamera;



	private void Start()
	{
		spawnArea = GetComponent<PolygonCollider2D>();
		spawnArea.isTrigger = true; 
		spawnPoints.AddRange(GetComponentsInChildren<SmartZombieSpawnPoint>());
		gameStartTime = Time.time;
		mainCamera = CursorManager.GetCamera();
	}

	private void Update()
	{

		CleanupDeadEnemies();
		UpdateDifficulty();
		UpdateSpawnPoints();
		UpdateWanderingZombies();
		UpdateBossZombies();
	}

	private void UpdateSpawnPoints()
	{
		var spawnRate = GetCurrentSpawnRate();
		float currentTime = Time.time;
		var actualSpawnInterval = Mathf.Lerp(baseSpawnInterval, minSpawnInterval, spawnRate);
		// Regular spawn points
		if (currentTime >= nextSpawnTime)
		{
			TrySpawnFromSpawnPoints();
			nextSpawnTime = currentTime + actualSpawnInterval;
		}
	}

	private void UpdateBossZombies()
	{
		var currentTime = Time.time;
		// Special events
		if (enableBossZombies && currentTime >= nextSpecialEventTime)
		{
			TryTriggerSpecialEvent();
			nextSpecialEventTime = currentTime + bossEventInterval;
		}
	}

	private void UpdateWanderingZombies()
	{
		var currentTime = Time.time;
		// Wandering zombies
		if (enableWanderingZombies && currentTime >= nextWanderingSpawnTime)
		{
			TrySpawnWanderingZombie();
			nextWanderingSpawnTime = currentTime + wanderingSpawnInterval;
		}
	}

	private void UpdateDifficulty()
	{
		// Calculate how far we are through the game (assuming a 20-minute session)
		var gameProgress = Mathf.Clamp01((Time.time - gameStartTime) / (20f * 60f));

		// Update current difficulty based on game progress
		currentDifficulty = difficultyCurve.Evaluate(gameProgress);
	}

	private float GetCurrentSpawnRate() => spawnRateCurve.Evaluate(DayNightCycle.I.GetCurrentDayFraction());



	private void TrySpawnFromSpawnPoints()
	{
		// Get active spawn points based on their criteria (proximity, etc.)
		var activePoints = GetActiveSpawnPoints();

		if (activePoints.Count == 0)
		{
			Debug.Log("No active spawn points available!");
			return;
		}

		// Select a random spawn point from active ones
		var selectedPoint = activePoints[Random.Range(0, activePoints.Count)];

		// Get an appropriate enemy prefab
		var enemyPrefab = GetRandomEnemyPrefab();

		if (enemyPrefab == null)
		{
			Debug.LogWarning("No valid enemy prefab available!");
			return;
		}

		// Try to spawn at that point
		var enemy = selectedPoint.TrySpawn(enemyPrefab, ensureOffCameraSpawning, mainCamera, offCameraMargin);

		if (enemy != null)
		{
			ConfigureNewEnemy(enemy);
			spawnedEnemies.Add(enemy);
		}
	}

	private List<SmartZombieSpawnPoint> GetActiveSpawnPoints()
	{
		var activePoints = new List<SmartZombieSpawnPoint>();

		foreach (var point in spawnPoints)
		{
			// Skip disabled spawn points
			if (!point.gameObject.activeInHierarchy || !point.enabled) continue;

			// Check if proximity-based and if so, check player distance
			if (point.isProximityBased && enableProximitySpawning)
			{
				if (Players.I != null && Players.AllJoinedPlayers.Count > 0)
				{
					var isPlayerNearby = false;

					foreach (var player in Players.AllJoinedPlayers)
					{
						if (player == null || !player.gameObject.activeInHierarchy) continue;

						var distance = Vector2.Distance(point.transform.position, player.transform.position);
						if (distance <= proximityActivationDistance)
						{
							isPlayerNearby = true;
							break;
						}
					}

					if (!isPlayerNearby) continue; // Skip this point if it's proximity-based but no player is nearby
				}
			}

			activePoints.Add(point);
		}

		return activePoints;
	}

	private void TrySpawnWanderingZombie()
	{

		Debug.Log("trying to spawn wander style");
		// Get a random player to use as a reference point
		var randomPlayer = Players.AllJoinedPlayers[Random.Range(0, Players.AllJoinedPlayers.Count)];
		if (randomPlayer == null) return;

		// Determine the group size for this spawn
		var groupSize = Random.Range(1, wanderingGroupSize + 1);

		if (groupSize <= 0) return;

		// For groups, find a single valid position first
		var spawnPosition = GetValidWanderingSpawnPosition(randomPlayer.transform.position);

		if (spawnPosition != Vector2.zero)
		{
			// Get an appropriate enemy prefab (same type for the whole group)
			var enemyPrefab = GetRandomEnemyPrefab();

			if (enemyPrefab == null) return;

			// Spawn the group
			for (var i = 0; i < groupSize; i++)
			{
				// Add some variation to positions for groups
				var offsetPosition = spawnPosition;
				if (i > 0)
				{
					offsetPosition += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

					// Verify it's still valid
					if (!IsPositionValid(offsetPosition)) continue;
				}

				// Spawn the enemy
				GameObject enemy;
				if (useObjectPooling)
					enemy = ObjectMaker.Make(enemyPrefab, offsetPosition);
				else
					enemy = Instantiate(enemyPrefab, offsetPosition, Quaternion.identity);

				if (enemy != null)
				{
					ConfigureNewEnemy(enemy);
					spawnedEnemies.Add(enemy);
				}
			}

			
		}
	}

	private void TryTriggerSpecialEvent()
	{
		if (!enableBossZombies) return;
		if (Random.value > bossZombieChance) return;


		if (Players.I == null || Players.AllJoinedPlayers.Count == 0) return;

		// Get a random player to use as a reference point
		var randomPlayer = Players.AllJoinedPlayers[Random.Range(0, Players.AllJoinedPlayers.Count)];
		if (randomPlayer == null) return;

		// Get a valid spawn position
		var spawnPosition = GetValidWanderingSpawnPosition(randomPlayer.transform.position);

		if (spawnPosition != Vector2.zero)
		{
			// Prefer Donut or Corn as a "boss" - they're bigger enemies
			var bossEnemyPrefab = Random.value > 0.5f ? ASSETS.Players.DonutEnemyPrefab : ASSETS.Players.CornEnemyPrefab;

			if (bossEnemyPrefab == null) return;

			// Spawn the boss enemy
			GameObject enemy;
			if (useObjectPooling)
				enemy = ObjectMaker.Make(bossEnemyPrefab, spawnPosition);
			else
				enemy = Instantiate(bossEnemyPrefab, spawnPosition, Quaternion.identity);

			if (enemy != null)
			{
				// Configure as a boss (extra health, damage, etc.)
				ConfigureNewEnemy(enemy, true);
				spawnedEnemies.Add(enemy);

			}
		}
	}

	private void ConfigureNewEnemy(GameObject enemy, bool isBoss = false)
	{
		// Set up the Life component
		var life = enemy.GetComponent<Life>();
		if (life != null)
		{
			// Apply difficulty scaling
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
		Debug.Log("trying to spawn wandering zombie");
		// Try to find a valid spawn position up to 10 times
		for (var i = 0; i < 10; i++)
		{
			Debug.Log( "trying point");
			var potentialPosition = GetRandomPointInPolygon();

			if (IsPositionValid(potentialPosition)) return potentialPosition;
		}

		Debug.Log("no valid point");
		// No valid position found after 10 attempts
		return Vector2.zero;
	}

	Vector2 GetRandomPointInPolygon()
	{
		// Get polygon bounds
		Bounds bounds = spawnArea.bounds;

		// Keep trying until we find a point inside
		while (true)
		{
			// Generate random point within bounds
			Vector2 randomPoint = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));

			// Check if point is inside polygon
			if (IsPositionValid(randomPoint))
			{
				return randomPoint;
			}
		}
	}
	private bool IsPositionValid(Vector2 position)
	{
		if (!spawnArea.OverlapPoint(position))
		{
			Debug.Log("Position is outside spawn area");
			return false;
		}

		// Check if position is not too close to any player
		foreach (var player in Players.AllJoinedPlayers)
		{
			if (player == null) continue;

			var playerDist = Vector2.Distance(position, player.transform.position);
			if (playerDist < minPlayerDistance) return false;
		}

		// Check for obstacles at the potential spawn position
		if (Physics2D.OverlapCircle(position, 0.5f, ASSETS.LevelAssets.BuildingLayer)) return false;

		// Check for landable layer to ensure enemy is on walkable surface
		if (!Physics2D.OverlapCircle(position, 0.5f, ASSETS.LevelAssets.LandableLayer)) return false;

		// Check if position is visible in camera (if required)
		if (ensureOffCameraSpawning && mainCamera != null)
		{
			var viewportPoint = mainCamera.WorldToViewportPoint(position);
			if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0) // In camera view
				return false;
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
		var totalFrequency = toastSpawnFrequency + coneSpawnFrequency + donutSpawnFrequency + cornSpawnFrequency;

		if (totalFrequency <= 0)
		{
			Debug.LogWarning("All enemy spawn frequencies are zero!");
			return null;
		}

		// Get a random value
		var random = Random.Range(0, totalFrequency);

		// Determine which enemy to spawn based on the random value
		if (random < toastSpawnFrequency)
		{
			Debug.Log("toast");
			return ASSETS.Players.ToastEnemyPrefab;
		}

		random -= toastSpawnFrequency;

		if (random < coneSpawnFrequency)
		{
			Debug.Log("cone");
			return ASSETS.Players.ConeEnemyPrefab;
		}

		random -= coneSpawnFrequency;

		if (random < donutSpawnFrequency)
		{
			Debug.Log("donut");
			return ASSETS.Players.DonutEnemyPrefab;
		}

		Debug.Log("corn");

		// If we get here, it's Corn (or if all else fails)
		return ASSETS.Players.CornEnemyPrefab;
	}

}