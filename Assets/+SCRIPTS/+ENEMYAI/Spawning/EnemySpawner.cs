using System;
using System.Collections;
using System.Collections.Generic;
using __SCRIPTS;
using __SCRIPTS.Cursor;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : SerializedMonoBehaviour
{
	public enum EnemyType
	{
		Toast,
		Cone,
		Donut,
		Corn,
		Fruit,
		Crimson
	}

	[Serializable]
	public class SpawnData
	{
		public EnemyType EnemyType;
		[Range(0, 4)] public int EnemyTier;
		[Range(1, 20)] public int Amount = 1;
	}

	public static Sprite GetAvatarFromType(EnemyType type)
	{
		switch (type)
		{
			case EnemyType.Toast:
				return Services.assetManager.UI.Toast_Avatar;
			case EnemyType.Cone:
				return Services.assetManager.UI.Cone_Avatar;
			case EnemyType.Donut:
				return Services.assetManager.UI.Donut_Avatar;
			case EnemyType.Corn:
				return Services.assetManager.UI.Corn_Avatar;
			case EnemyType.Fruit:
				return Services.assetManager.UI.Fruit_Avatar;
		}

		return null;
	}

	public static GameObject GetPrefabFromType(EnemyType type)
	{
		switch (type)
		{
			case EnemyType.Toast:
				return Services.assetManager.Players.ToastEnemyPrefab;
			case EnemyType.Cone:
				return Services.assetManager.Players.ConeEnemyPrefab;
			case EnemyType.Donut:
				return Services.assetManager.Players.DonutEnemyPrefab;
			case EnemyType.Corn:
				return Services.assetManager.Players.CornEnemyPrefab;
			case EnemyType.Fruit:
				return Services.assetManager.Players.FruitEnemyPrefabs.GetRandom();
		}

		return null;
	}

	public CinemachineCamera arenaCamera;
	[SerializeField] public List<SpawnData> enemyPrefabsDictionary = new();
	[SerializeField] bool randomizeOrder = true;

	[Header("Spawn Settings"), SerializeField]
	float spawnInterval = 2f;
	float spawnMargin = -2f; // 0 for infinite


	Camera mainCamera => _mainCamera ??= CursorManager.GetCamera();
	Camera _mainCamera;
	float spawnTimer;
	float durationTimer;
	bool isSpawning;
	bool isFinishedSpawning;
	bool isComplete;
	SpawnableArea currentSpawnArea;
	List<GameObject> spawnedEnemies = new();
	int minimumLeftAliveToStillFinish = 1;
	public GameObject GoIndicator => _goIndicator ??= FindFirstObjectByType<GoIndicator>(FindObjectsInactive.Include).gameObject;
	GameObject _goIndicator;

	public event Action OnSpawningStart;
	public event Action OnSpawningStop;
	public event Action OnSpawningComplete;
	public event Action OnSpawnedEnemyDead;

	protected void OnTriggerEnter2D(Collider2D other)
	{
		var otherLife = other.GetComponent<Life>();
		if (otherLife == null) return;
		if (otherLife.IsDead()) return;
		if (!otherLife.player.IsMainPlayer()) return;
		if (isFinishedSpawning || isSpawning) return;
		StartSpawning();
		SoloCamera();
	}

	void SoloCamera()
	{
		CameraSwitcher.I.SoloCamera(arenaCamera);
	}

	void Start()
	{
		currentSpawnArea = FindFirstObjectByType<SpawnableArea>();
	}

	[Button]
	public void TestSpawnArea()
	{
		currentSpawnArea = FindFirstObjectByType<SpawnableArea>();
		Debug.Log(currentSpawnArea.AreaCollider);
		var spawnPos = FindRandomSpawnPosition();
		if (spawnPos == default) Debug.LogWarning("Failed to find valid spawn position!");
	}

	void Update()
	{
		if (!isSpawning) return;
		spawnTimer += Time.deltaTime;
		durationTimer += Time.deltaTime;
		if (!(spawnTimer >= spawnInterval)) return;
		SpawnAtRandomEdge();
		spawnTimer = 0f;
	}

	void StartSpawning()
	{
		isSpawning = true;
		isFinishedSpawning = false;
		spawnTimer = 0f;
		durationTimer = 0f;
		Debug.Log("Enemy spawning started");
		SpawnAtRandomEdge();
		OnSpawningStart?.Invoke();
	}

	void StopSpawning()
	{
		isSpawning = false;
		isFinishedSpawning = true;
		Debug.Log("Enemy spawning stopped");
		OnSpawningStop?.Invoke();
		if (spawnedEnemies.Count + enemyPrefabsDictionary.Count <= minimumLeftAliveToStillFinish) CompleteArena();
		else
		{
			Debug.Log("stopped spawning but enemies still remain: " + spawnedEnemies.Count + enemyPrefabsDictionary.Count);
		}
	}

	void CompleteArena()
	{
		if (isComplete) return;
		isComplete = true;
		CameraSwitcher.I.UnSoloCamera();
		ShowGo();
		OnSpawningComplete?.Invoke();
		Debug.Log("Enemy spawning complete");
	}

	void ShowGo()
	{
		Debug.Log("show go");
		GoIndicator.gameObject.SetActive(true);
		Invoke(nameof(HideGo), 2);
	}

	public void HideGo()
	{
		Debug.Log("hide gogogo");
		GoIndicator.gameObject.SetActive(false);
	}

	void SpawnAtRandomEdge()
	{
		if (enemyPrefabsDictionary.Count == 0)
		{
			Debug.Log("No enemy prefabs to spawn!");
			StopSpawning();
			return;
		}

		var spawnPos = FindRandomSpawnPosition();
		if (spawnPos == default)
		{
			Debug.LogWarning("Failed to find valid spawn position!");
			return;
		}

		var enemyPrefab = GetEnemyPrefab();

		if (enemyPrefab == null)
		{
			Debug.Log("No valid enemy prefab to spawn!");
			StopSpawning();
			return;
		}

		SpawnEnemy(enemyPrefab, spawnPos);
	}

	Vector3 FindRandomSpawnPosition()
	{
		var maxTries = 8;
		for (var i = 0; i < maxTries; i++)
		{
			var randomPosition = GetRandomSpawnPosition();
			if (currentSpawnArea.AreaCollider.OverlapPoint(randomPosition))
			{
				MyDebugUtilities.DrawX(randomPosition, 12, Color.green, 2);
				Debug.Log("Found spawn position at " + randomPosition);
				return randomPosition;
			}

			MyDebugUtilities.DrawX(randomPosition, 12, Color.red, 2);
			Debug.Log("yo!");
		}

		return default;
	}

	SpawnData GetEnemyPrefab()
	{
		if (randomizeOrder)
		{
			var data = enemyPrefabsDictionary[Random.Range(0, enemyPrefabsDictionary.Count)];
			if (data.Amount <= 0)
			{
				enemyPrefabsDictionary.Remove(data);
				if (enemyPrefabsDictionary.Count != 0) return enemyPrefabsDictionary[Random.Range(0, enemyPrefabsDictionary.Count)];
				StopSpawning();
				return null;
			}

			data.Amount--;
			return enemyPrefabsDictionary[Random.Range(0, enemyPrefabsDictionary.Count)];
		}

		var index = Mathf.FloorToInt(durationTimer / spawnInterval) % enemyPrefabsDictionary.Count;
		return enemyPrefabsDictionary[index];
	}

	Vector3 GetRandomSpawnPosition()
	{
		Debug.Log("get random spawn position...");
		var camHeight = 2f * mainCamera.orthographicSize;
		var camWidth = camHeight * mainCamera.aspect;
		var camPos = mainCamera.transform.position;

		var spawnPosition = new Vector3(Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f),
			Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f), 0f);


		Debug.Log("Calculated spawn position at " + spawnPosition);

		return spawnPosition;
	}

	void SpawnEnemy(SpawnData spawnData, Vector3 position)
	{
		Debug.Log("tyring to spawn enemy...");
		var prefab = GetPrefabFromType(spawnData.EnemyType);
		var newEnemy = Services.enemyManager.SpawnNewEnemy(prefab, spawnData.EnemyType, position, spawnData.EnemyTier);
		var newEnemyLife = newEnemy.GetComponent<Life>();
		if (newEnemyLife == null)
		{
			Debug.LogWarning("Spawned enemy has no Life component!");
			return;
		}

		newEnemyLife.OnDead += SpawnedEnemy_OnDead;

		Debug.Log($"Spawned enemy of type {spawnData.EnemyType} and tier {spawnData.EnemyTier} at {position}");
		spawnedEnemies.Add(newEnemy);
	}

	void SpawnedEnemy_OnDead(Attack attack)
	{
		if (!spawnedEnemies.Contains(attack.DestinationLife.transform.gameObject)) return;
		attack.DestinationLife.OnDead -= SpawnedEnemy_OnDead;
		spawnedEnemies.Remove(attack.DestinationLife.transform.gameObject);
		OnSpawnedEnemyDead?.Invoke();
		Debug.Log("An enemy has been killed. Remaining: " + spawnedEnemies.Count
		                                                  + "is finished spawning: " + isFinishedSpawning
		                                                  + " min left: " + minimumLeftAliveToStillFinish);
		if (spawnedEnemies.Count + enemyPrefabsDictionary.Count > minimumLeftAliveToStillFinish)
		{
			Debug.Log(spawnedEnemies.Count + enemyPrefabsDictionary.Count + " Enemies still remain. Arena not complete yet.");
			return;
		}
		if (!isFinishedSpawning)
		{
			Debug.Log(" Spawning not finished yet. Arena not complete yet.");
			return;
		}
		Debug.Log("All enemies defeated. Arena complete!");
		CompleteArena();
	}
}
