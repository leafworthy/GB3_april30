using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
public class LevelDrops : MonoBehaviour
{
	private static List<GameObject> _dropsToSpawn = new();
	public static event Action<Vector2, LootType> OnLootDrop;
	private void Start()
	{
		MakeDropList();
		Enemies.OnEnemyDying += (x, y) => DropLoot(y.transform.position);
	}

	public static void DropLoot(Vector2 position, LootType type = LootType.Random)
	{
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
		var prefab = Maker.Make(lootPrefab, position);
		var fallingObject = prefab.GetComponent<FallToFloor>();
		fallingObject.Fire(new Vector3((float)Random.Range(-1, 1), 1),Color.white, 5);
		OnLootDrop?.Invoke(position, type);
	}
	private void MakeDropList()
	{
		_dropsToSpawn.Clear();
		_dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
		_dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);

		_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
		_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
		_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
		_dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);

		_dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);
		_dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);

		_dropsToSpawn.Add(ASSETS.FX.nadesPickupPrefab);
		_dropsToSpawn.Add(ASSETS.FX.nadesPickupPrefab);

		_dropsToSpawn.Add(ASSETS.FX.gasPickupPrefab);
	}

	
}