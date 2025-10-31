using System.Collections.Generic;
using __SCRIPTS;
using __SCRIPTS.Cursor;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner : SerializedMonoBehaviour
{
	private struct PrefabAndEnemyTier
	{
		public GameObject prefab;
		public int EnemyTier;
	}

	[SerializeField] private List<PrefabAndEnemyTier> enemyPrefabsDictionary = new();
	[SerializeField] private bool randomizeEnemy = true;

	[Header("Spawn Settings"), SerializeField]
	private float spawnInterval = 2f;
	private float spawnMargin = -2f;
	[SerializeField] private float spawnDuration = 30f; // 0 for infinite

	[Header("Spawn Sides"), SerializeField]
	private bool spawnLeft = true;
	[SerializeField] private bool spawnRight = true;
	[SerializeField] private bool spawnBottom = true;

	private Camera mainCamera;
	private float spawnTimer;
	private float durationTimer;
	private bool isSpawning;
	private bool isFinished;
	public bool IsFinished => isFinished;
	private EnemySpawnArea currentSpawnArea;

	protected void OnTriggerEnter2D(Collider2D other)
	{
		var otherLife = other.GetComponent<Life>();
		if (otherLife == null) return;
		if (otherLife.IsDead()) return;
		if (!otherLife.Player.IsHuman()) return;
		if (isFinished) return;
		StartSpawning();
	}

	private void Start()
	{
		currentSpawnArea = FindFirstObjectByType<EnemySpawnArea>();
		// Get the main camera reference
		mainCamera = CursorManager.GetCamera();

		if (mainCamera == null) Debug.LogError("No main camera found!");

		if (enemyPrefabsDictionary.Count == 0) Debug.LogWarning("No enemy prefabs assigned to spawner!");
	}

	private void Update()
	{
		if (!isSpawning) return;

		// Update timers
		spawnTimer += Time.deltaTime;
		durationTimer += Time.deltaTime;

		// Check if duration exceeded (if duration is set)
		if (spawnDuration > 0 && durationTimer >= spawnDuration)
		{
			StopSpawning();
			return;
		}

		// Spawn enemy at interval
		if (!(spawnTimer >= spawnInterval)) return;
		SpawnAtRandomEdge();
		spawnTimer = 0f;
	}

	// Public method to start spawning
	private void StartSpawning()
	{
		if (isSpawning) return;
		isSpawning = true;
		isFinished = false;
		spawnTimer = 0f;
		durationTimer = 0f;
		Debug.Log("Enemy spawning started");
	}

	// Public method to stop spawning
	private void StopSpawning()
	{
		isSpawning = false;
		isFinished = true;
		Debug.Log("Enemy spawning stopped");
	}

	// Public method to start with custom duration
	public void StartSpawning(float customDuration)
	{
		spawnDuration = customDuration;
		StartSpawning();
	}

	private void SpawnAtRandomEdge()
	{
		if (enemyPrefabsDictionary.Count == 0) return;
		// Get spawn position and enemy prefab
		var spawnPos = FindRandomSpawnPosition();
		if(spawnPos == default) return;
		var enemyPrefab = GetEnemyPrefab();

		SpawnEnemy(enemyPrefab, spawnPos);
	}

	private Vector3 FindRandomSpawnPosition()
	{

		 var randomPosition = GetRandomSpawnPosition();
		 int maxTries = 30;
		 for (int i = 0; i < maxTries; i++)
		 {
		 if(currentSpawnArea.AreaCollider.OverlapPoint(randomPosition)) return randomPosition;
		 }

		 return default;

	}

	private PrefabAndEnemyTier GetEnemyPrefab()
	{
		if (randomizeEnemy)
		{
			// Pick random enemy from list
			return enemyPrefabsDictionary[Random.Range(0, enemyPrefabsDictionary.Count)];
		}

		// Cycle through enemies in order
		var index = Mathf.FloorToInt(durationTimer / spawnInterval) % enemyPrefabsDictionary.Count;
		return enemyPrefabsDictionary[index];
	}

	private Vector3 GetRandomSpawnPosition()
	{
		// Calculate camera bounds in world space
		var camHeight = 2f * mainCamera.orthographicSize;
		var camWidth = camHeight * mainCamera.aspect;
		var camPos = mainCamera.transform.position;

		// Build list of possible spawn sides
		var possibleSides = new List<int>();
		if (spawnLeft) possibleSides.Add(0);
		if (spawnRight) possibleSides.Add(1);
		if (spawnBottom) possibleSides.Add(2);

		if (possibleSides.Count == 0)
		{
			Debug.LogWarning("No spawn sides enabled!");
			return camPos;
		}

		// Pick a random side
		var side = possibleSides[Random.Range(0, possibleSides.Count)];

		var spawnPosition = camPos;

		switch (side)
		{
			case 0: // Left
				spawnPosition.x = camPos.x - camWidth / 2f - spawnMargin;
				spawnPosition.y = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
				break;

			case 1: // Right
				spawnPosition.x = camPos.x + camWidth / 2f + spawnMargin;
				spawnPosition.y = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
				break;

			case 2: // Bottom
				spawnPosition.x = Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f);
				spawnPosition.y = camPos.y - camHeight / 2f - spawnMargin;
				break;
		}

		spawnPosition.z = 0f; // Assuming 2D or sprites
		return spawnPosition;
	}

	private void SpawnEnemy(PrefabAndEnemyTier prefab, Vector3 position)
	{
		Services.enemyManager.SpawnNewEnemy(prefab.prefab, position, prefab.EnemyTier);

		Debug.Log($"Spawned {prefab.prefab.name} at {position}");
	}
}
