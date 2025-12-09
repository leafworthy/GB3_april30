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
				return Services.assetManager.Players.FruitEnemyPrefab;
		}

		return null;
	}

	public CinemachineCamera arenaCamera;
	[SerializeField] public List<SpawnData> enemyPrefabsDictionary = new();
	[SerializeField] bool randomizeEnemy = true;

	[Header("Spawn Settings"), SerializeField]
	float spawnInterval = 2f;
	float spawnMargin = -2f;
	[SerializeField] float spawnDuration = 30f; // 0 for infinite

	[Header("Spawn Sides"), SerializeField]
	bool spawnLeft = true;
	[SerializeField] bool spawnRight = true;
	[SerializeField] bool spawnBottom = true;

	Camera mainCamera => _mainCamera ??= CursorManager.GetCamera();
	Camera _mainCamera;
	float spawnTimer;
	float durationTimer;
	bool isSpawning;
	bool isFinishedSpawning;
	bool isComplete;
	public bool IsFinishedSpawning => isFinishedSpawning;
	SpawnableArea currentSpawnArea;
	List<GameObject> spawnedEnemies  = new();
	public int minimumLeftAliveToStillFinish = 2;
	public GameObject GoIndicator => _goIndicator ??= FindFirstObjectByType<GoIndicator>(FindObjectsInactive.Include).gameObject;
	GameObject _goIndicator;

	protected void OnTriggerEnter2D(Collider2D other)
	{
		var otherLife = other.GetComponent<Life>();
		if (otherLife == null) return;
		if (otherLife.IsDead()) return;
		if (!otherLife.player.IsHuman()) return;
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
		if (spawnPos == default)
		{
			Debug.LogWarning("Failed to find valid spawn position!");
			return;
		}
		else
		{
			Debug.Log( "Found spawn position at " + spawnPos);
		}
	}

	void Update()
	{
		if (!isSpawning) return;
		spawnTimer += Time.deltaTime;
		durationTimer += Time.deltaTime;

		if (spawnDuration > 0 && durationTimer >= spawnDuration)
		{
			StopSpawning();
			return;
		}

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
	}

	void StopSpawning()
	{
		isSpawning = false;
		isFinishedSpawning = true;
		Debug.Log("Enemy spawning stopped");
	}

	void CompleteArena()
	{
		if (isComplete) return;
		isComplete = true;
		CameraSwitcher.I.UnSoloCamera();
		ShowGo();
	}

	void ShowGo()
	{
		GoIndicator.gameObject.SetActive(true);
		StartCoroutine(nameof(HideGo));
	}

	public IEnumerable HideGo()
	{
		yield return new WaitForSeconds(2);
		GoIndicator.gameObject.SetActive(false);
	}

	void SpawnAtRandomEdge()
	{
		if (enemyPrefabsDictionary.Count == 0)
		{
			Debug.Log("No enemy prefabs to spawn!");
			return;
		}
		var spawnPos = FindRandomSpawnPosition();
		if (spawnPos == default)
		{
			Debug.LogWarning("Failed to find valid spawn position!");
			return;
		}
		var enemyPrefab = GetEnemyPrefab();

		if(enemyPrefab == null)
		{
			Debug.Log("No valid enemy prefab to spawn!");
			return;
		}
		SpawnEnemy(enemyPrefab, spawnPos);
	}

	Vector3 FindRandomSpawnPosition()
	{

		var maxTries = 5;
		for (var i = 0; i < maxTries; i++)
		{
			var randomPosition = GetRandomSpawnPosition();
			if (currentSpawnArea.AreaCollider.OverlapPoint(randomPosition))
			{
				MyDebugUtilities.DrawX(randomPosition, 12, Color.green, 2);
				Debug.Log("Found spawn position at " + randomPosition);
				return randomPosition;
			}
			else
			{
				MyDebugUtilities.DrawX(randomPosition, 12, Color.red, 2);
				Debug.Log("yo!");
			}
		}


		return default;
	}

	SpawnData GetEnemyPrefab()
	{
		if (randomizeEnemy)
		{
			var data = enemyPrefabsDictionary[Random.Range(0, enemyPrefabsDictionary.Count)];
			if (data.Amount <= 0)
			{
				enemyPrefabsDictionary.Remove(data);
				if (enemyPrefabsDictionary.Count != 0) return enemyPrefabsDictionary[Random.Range(0, enemyPrefabsDictionary.Count)];
				StopSpawning();
				return null;
			}
			else
			{
				data.Amount--;
			}
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
				//spawnPosition.y = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
				spawnPosition.y = Random.Range(camPos.y - camHeight / 2f, camPos.y);
				break;

			case 1: // Right
				spawnPosition.x = camPos.x + camWidth / 2f + spawnMargin;
				spawnPosition.y = Random.Range(camPos.y - camHeight / 2f, camPos.y);
				break;

			case 2: // Bottom
				spawnPosition.x = Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f);
				spawnPosition.y = Random.Range(camPos.y - camHeight / 2f, camPos.y);
				break;
		}

		spawnPosition.z = 0f; // Assuming 2D or sprites
		Debug.Log( "Calculated spawn position at " + spawnPosition);

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
		Debug.Log("An enemy has been killed. Remaining: " + spawnedEnemies.Count);
		if (spawnedEnemies.Count <= minimumLeftAliveToStillFinish && isFinishedSpawning)
		{
			Debug.Log( "All enemies defeated. Arena complete!");
			CompleteArena();
		}
	}
}
