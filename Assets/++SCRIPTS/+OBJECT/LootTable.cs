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

	public class LootTable : Singleton<LootTable>
	{
		private static List<GameObject> _dropsToSpawn = new();


		private void Start()
		{
			EnemyManager.I.OnEnemyDying += DropLootFromEnemy;
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
					lootPrefab = ASSETS.FX.cashPickupPrefab;
					break;
				case LootType.Ammo:
					lootPrefab = ASSETS.FX.ammoPickupPrefab;
					break;
				case LootType.Health:
					lootPrefab = ASSETS.FX.healthPickupPrefab;
					break;
				case LootType.Nades:
					lootPrefab = ASSETS.FX.nadesPickupPrefab;
					break;
				case LootType.Speed:
					lootPrefab = ASSETS.FX.speedPickupPrefab;
					break;
				case LootType.Damage:
					lootPrefab = ASSETS.FX.damagePickupPrefab;
					break;
				case LootType.Random:
					lootPrefab = _dropsToSpawn.GetRandom();
					break;
				case LootType.Gas:
					lootPrefab = ASSETS.FX.gasPickupPrefab;
					break;
			}

			var prefab = ObjectMaker.I.Make(lootPrefab, position);
			var fallingObject = prefab.GetComponent<FallToFloor>();
			fallingObject.FireForDrops(new Vector3(Random.Range(-1, 1), -1), Color.white, 5, true);
		}

		private void MakeDropList()
		{
			_dropsToSpawn.Clear();
			_dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);

			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);

			_dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);
			_dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);

			_dropsToSpawn.Add(ASSETS.FX.nadesPickupPrefab);

			_dropsToSpawn.Add(ASSETS.FX.gasPickupPrefab);
		}
	}
}
