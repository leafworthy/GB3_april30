﻿using System;
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
					lootPrefab = assets.FX.cashPickupPrefab;
					break;
				case LootType.Ammo:
					lootPrefab = assets.FX.ammoPickupPrefab;
					break;
				case LootType.Health:
					lootPrefab = assets.FX.healthPickupPrefab;
					break;
				case LootType.Nades:
					lootPrefab = assets.FX.nadesPickupPrefab;
					break;
				case LootType.Speed:
					lootPrefab = assets.FX.speedPickupPrefab;
					break;
				case LootType.Damage:
					lootPrefab = assets.FX.damagePickupPrefab;
					break;
				case LootType.Random:
					lootPrefab = _dropsToSpawn.GetRandom();
					break;
				case LootType.Gas:
					lootPrefab = assets.FX.gasPickupPrefab;
					break;
			}

			var prefab = objectMaker.Make(lootPrefab, position);
			var fallingObject = prefab.GetComponent<FallToFloor>();
			fallingObject.FireForDrops(new Vector3(Random.Range(-1, 1), -1), Color.white, 5, true);
		}

		private void MakeDropList()
		{
			_dropsToSpawn.Clear();
			_dropsToSpawn.Add( assets.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( assets.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( assets.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( assets.FX.ammoPickupPrefab);
			_dropsToSpawn.Add( assets.FX.ammoPickupPrefab);

			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);
			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);
			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);
			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);
			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);
			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);
			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);
			_dropsToSpawn.Add( assets.FX.cashPickupPrefab);

			_dropsToSpawn.Add( assets.FX.healthPickupPrefab);
			_dropsToSpawn.Add( assets.FX.healthPickupPrefab);
			_dropsToSpawn.Add( assets.FX.healthPickupPrefab);
			_dropsToSpawn.Add( assets.FX.healthPickupPrefab);

			_dropsToSpawn.Add( assets.FX.nadesPickupPrefab);

			_dropsToSpawn.Add( assets.FX.gasPickupPrefab);
		}
	}
}
