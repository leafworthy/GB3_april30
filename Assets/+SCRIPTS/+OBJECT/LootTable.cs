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

	public class LootTable : ServiceUser, IService
	{
		private static List<GameObject> _dropsToSpawn = new();


		public void StartService()
		{
			enemyManager.OnEnemyDying += DropLootFromEnemy;
			MakeDropList();
		}


		private void DropLootFromEnemy(Player player, Life life)
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
					lootPrefab = AssetManager.FX.cashPickupPrefab;
					break;
				case LootType.Ammo:
					lootPrefab = AssetManager.FX.ammoPickupPrefab;
					break;
				case LootType.Health:
					lootPrefab = AssetManager.FX.healthPickupPrefab;
					break;
				case LootType.Nades:
					lootPrefab = AssetManager.FX.nadesPickupPrefab;
					break;
				case LootType.Speed:
					lootPrefab = AssetManager.FX.speedPickupPrefab;
					break;
				case LootType.Damage:
					lootPrefab = AssetManager.FX.damagePickupPrefab;
					break;
				case LootType.Random:
					lootPrefab = _dropsToSpawn.GetRandom();
					break;
				case LootType.Gas:
					lootPrefab = AssetManager.FX.gasPickupPrefab;
					break;
			}

			var prefab = objectMaker.Make(lootPrefab, position);
			var fallingObject = prefab.GetComponent<FallToFloor>();
			fallingObject.FireForDrops(new Vector3(Random.Range(-1, 1), -1), Color.white, 5, true);
		}

		private void MakeDropList()
		{
			_dropsToSpawn.Clear();
			_dropsToSpawn.Add( AssetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.ammoPickupPrefab);

			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.cashPickupPrefab);

			_dropsToSpawn.Add( AssetManager.FX.healthPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.healthPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.healthPickupPrefab);
			_dropsToSpawn.Add( AssetManager.FX.healthPickupPrefab);

			_dropsToSpawn.Add( AssetManager.FX.nadesPickupPrefab);

			_dropsToSpawn.Add( AssetManager.FX.gasPickupPrefab);
		}
	}
}
