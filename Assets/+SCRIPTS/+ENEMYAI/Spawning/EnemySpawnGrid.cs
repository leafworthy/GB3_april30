using System.Collections.Generic;
using __SCRIPTS;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnGrid : MonoBehaviour
{
	public GridLayoutGroup gridLayoutGroup;
	EnemySpawner spawner => _spawner ??= GetComponent<EnemySpawner>();
	EnemySpawner _spawner;

	void Start()
	{
		gridLayoutGroup.transform.RemoveChildren();
		spawner.OnSpawningStart += GenerateGrid;
		spawner.OnSpawnedEnemyDead += GenerateGrid;
		spawner.OnSpawningComplete += SpawnerSpawningComplete;
	}

	void SpawnerSpawningComplete()
	{
		gameObject.SetActive(false);
		gridLayoutGroup.transform.RemoveChildren();
		spawner.OnSpawningStart -= GenerateGrid;
		spawner.OnSpawnedEnemyDead -= GenerateGrid;
		spawner.OnSpawningComplete -= SpawnerSpawningComplete;
	}

	void FixedUpdate()
	{
		GenerateGrid();
	}

	[Button]
	public void GenerateGrid()
	{
		gridLayoutGroup.transform.RemoveChildren();

		foreach (var spawnData in spawner.enemyPrefabsDictionary)
		{
			var gridCell = Services.objectMaker.Make(Services.assetManager.UI.SpawnCellPrefab);
			gridCell.transform.SetParent(gridLayoutGroup.transform, false);
			var spawnCellScript = gridCell.GetComponent<SpawnGridCell>();
			var sprite = EnemySpawner.GetAvatarFromType(spawnData.EnemyType);
			spawnCellScript.Set(sprite, spawnData.EnemyType, spawnData.EnemyTier, spawnData.Amount);
		}
	}
}
