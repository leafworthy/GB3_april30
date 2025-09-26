using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("Enemy Prefabs"), SerializeField]

	private List<GameObject> enemyPrefabs = new();
	[SerializeField] private bool randomizeEnemy = true;

	[Header("Spawn Settings"), SerializeField]

	private float spawnInterval = 2f;
	[SerializeField] private float spawnMargin = 1f;
	[SerializeField] private float spawnDuration = 30f; // 0 for infinite

	[Header("Spawn Sides"), SerializeField]

	private bool spawnLeft = true;
	[SerializeField] private bool spawnRight = true;
	[SerializeField] private bool spawnBottom = true;

	private Camera mainCamera;
	private float spawnTimer;
	private float durationTimer;
	private bool isSpawning;

	private void Start()
	{
		// Get the main camera reference
		mainCamera = Camera.main;

		if (mainCamera == null) Debug.LogError("No main camera found!");

		if (enemyPrefabs.Count == 0) Debug.LogWarning("No enemy prefabs assigned to spawner!");
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
		if (spawnTimer >= spawnInterval)
		{
			SpawnAtRandomEdge();
			spawnTimer = 0f;
		}
	}

	// Public method to start spawning
	public void StartSpawning()
	{
		if (enemyPrefabs.Count == 0)
		{
			Debug.LogError("Cannot start spawning - no enemy prefabs assigned!");
			return;
		}

		isSpawning = true;
		spawnTimer = 0f;
		durationTimer = 0f;
		Debug.Log("Enemy spawning started");
	}

	// Public method to stop spawning
	public void StopSpawning()
	{
		isSpawning = false;
		Debug.Log("Enemy spawning stopped");
	}

	// Public method to start with custom duration
	public void StartSpawning(float customDuration)
	{
		spawnDuration = customDuration;
		StartSpawning();
	}

	// Toggle spawning on/off
	public void ToggleSpawning()
	{
		if (isSpawning)
			StopSpawning();
		else
			StartSpawning();
	}

	// Check if currently spawning
	public bool IsSpawning() => isSpawning;

	// Get remaining spawn time
	public float GetRemainingTime()
	{
		if (spawnDuration <= 0) return -1f; // Infinite duration
		return Mathf.Max(0f, spawnDuration - durationTimer);
	}

	private void SpawnAtRandomEdge()
	{
		// Get spawn position and enemy prefab
		var spawnPos = GetRandomSpawnPosition();
		var enemyPrefab = GetEnemyPrefab();

		if (enemyPrefab != null) SpawnEnemy(enemyPrefab, spawnPos);
	}

	private GameObject GetEnemyPrefab()
	{
		if (enemyPrefabs.Count == 0) return null;

		if (randomizeEnemy)
		{
			// Pick random enemy from list
			return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
		}

		// Cycle through enemies in order
		var index = Mathf.FloorToInt(durationTimer / spawnInterval) % enemyPrefabs.Count;
		return enemyPrefabs[index];
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

	private void SpawnEnemy(GameObject prefab, Vector3 position)
	{
		var enemy = Instantiate(prefab, position, Quaternion.identity);

		// Optional: parent to a container for organization
		// enemy.transform.SetParent(transform);

		Debug.Log($"Spawned {prefab.name} at {position}");
	}

	// Optional: Clean up all spawned enemies
	public void DestroyAllEnemies()
	{
		// If enemies are parented to this object
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		// Or find by tag
		// GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		// foreach (GameObject enemy in enemies)
		// {
		//     Destroy(enemy);
		// }
	}
}
