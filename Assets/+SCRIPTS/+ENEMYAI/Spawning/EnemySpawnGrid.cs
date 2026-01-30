using __SCRIPTS;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnGrid : MonoBehaviour
{
	public GridLayoutGroup gridLayoutGroup;
	public EnemySpawner Spawner => spawner ??= GetComponentInParent<EnemySpawner>();
	EnemySpawner spawner;

	[Button]
	public void GenerateGrid()
	{
		gridLayoutGroup.transform.RemoveChildren();

		foreach (EnemySpawner.SpawnData spawnData in Spawner.enemyPrefabsDictionary)
		{
			var objectMaker = ServiceLocator.Get<ObjectMaker>();
			var gridCell = objectMaker.Make(Services.assetManager.UI.SpawnCellPrefab);
			gridCell.transform.SetParent(gridLayoutGroup.transform, false);
			var spawnCellScript = gridCell.GetComponent<SpawnGridCell>();
			var sprite = EnemySpawner.GetAvatarFromType(spawnData.EnemyType);
			spawnCellScript.Set(sprite, spawnData.EnemyType, spawnData.EnemyTier, spawnData.Amount);
		}
	}
}
