using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public enum LootType
	{
		Random,
		Cash,
		Ammo,
		Health,
		Nades,
		Speed,
		Damage,
		Gas
	}

	public class LootTable : MonoBehaviour, IService
	{
		private static List<GameObject> _dropsToSpawn = new();


		public void StartService()
		{
			Services.enemyManager.OnEnemyDying += DropLootFromEnemy;
			MakeDropList();
		}


		private void DropLootFromEnemy(Life life)
		{
			if (life.IsDead()) DropLoot(life.transform.position);
		}


		public void DropLoot(Vector2 position, LootType type = LootType.Random)
		{
			if (_dropsToSpawn.Count == 0) MakeDropList();

			GameObject lootPrefab = null;
			switch (type)
			{
				case LootType.Cash:
					lootPrefab = Services.assetManager.FX.cashPickupPrefab;
					break;
				case LootType.Ammo:
					lootPrefab = Services.assetManager.FX.ammoPickupPrefab;
					break;
				case LootType.Health:
					lootPrefab = Services.assetManager.FX.healthPickupPrefab;
					break;
				case LootType.Nades:
					lootPrefab = Services.assetManager.FX.nadesPickupPrefab;
					break;
				case LootType.Speed:
					lootPrefab = Services.assetManager.FX.speedPickupPrefab;
					break;
				case LootType.Damage:
					lootPrefab = Services.assetManager.FX.damagePickupPrefab;
					break;
				case LootType.Random:
					lootPrefab = _dropsToSpawn.GetRandom();
					break;
				case LootType.Gas:
					lootPrefab = Services.assetManager.FX.gasPickupPrefab;
					break;
			}

			var prefab = Services.objectMaker.Make(lootPrefab, position);
			var fallingObject = prefab.GetComponent<FallToFloor>();
			fallingObject.FireForDrops(new Vector3(Random.Range(-1, 1), -1), Color.white, 5, true);
		}

		private void MakeDropList()
		{
			_dropsToSpawn.Clear();
			_dropsToSpawn.Add(Services.assetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.ammoPickupPrefab);

			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.cashPickupPrefab);

			_dropsToSpawn.Add(Services.assetManager.FX.healthPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.healthPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.healthPickupPrefab);
			_dropsToSpawn.Add(Services.assetManager.FX.healthPickupPrefab);

			_dropsToSpawn.Add(Services.assetManager.FX.nadesPickupPrefab);

			_dropsToSpawn.Add(Services.assetManager.FX.gasPickupPrefab);
		}
	}
}
