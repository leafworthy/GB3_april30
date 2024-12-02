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
		EnemyManager.OnEnemyDying += (x, y) => DropLoot(y.transform.position);
	}

	public static void DropLoot(Vector2 position, LootType type = LootType.Random)
	{
		GameObject lootPrefab = null;
		switch (type)
		{
			case LootType.Cash:
				lootPrefab = FX.Assets.cashPickupPrefab;
				break;
			case LootType.Ammo:
				lootPrefab = FX.Assets.ammoPickupPrefab;
				break;
			case LootType.Health:
				lootPrefab = FX.Assets.healthPickupPrefab;
				break;
			case LootType.Nades:
				lootPrefab = FX.Assets.nadesPickupPrefab;
				break;
			case LootType.Speed:
				lootPrefab = FX.Assets.speedPickupPrefab;
				break;
			case LootType.Damage:
				lootPrefab = FX.Assets.damagePickupPrefab;
				break;
			case LootType.Random:
				lootPrefab = _dropsToSpawn.GetRandom();
				break;
			case LootType.Gas:
				lootPrefab = FX.Assets.gasPickupPrefab;
				break;
		}
		var prefab = Maker.Make(lootPrefab, position);
		var fallingObject = prefab.GetComponent<FallToFloor>();
		fallingObject.FireForDrops(new Vector3((float)Random.Range(-1, 1), -1),Color.white, 5, true);
		OnLootDrop?.Invoke(position, type);
	}
	private void MakeDropList()
	{
		_dropsToSpawn.Clear();
		_dropsToSpawn.Add(FX.Assets.ammoPickupPrefab);
		_dropsToSpawn.Add(FX.Assets.ammoPickupPrefab);

		_dropsToSpawn.Add(FX.Assets.cashPickupPrefab);
		_dropsToSpawn.Add(FX.Assets.cashPickupPrefab);
		_dropsToSpawn.Add(FX.Assets.cashPickupPrefab);
		_dropsToSpawn.Add(FX.Assets.cashPickupPrefab);

		_dropsToSpawn.Add(FX.Assets.healthPickupPrefab);
		_dropsToSpawn.Add(FX.Assets.healthPickupPrefab);

		_dropsToSpawn.Add(FX.Assets.nadesPickupPrefab);
		_dropsToSpawn.Add(FX.Assets.nadesPickupPrefab);

		_dropsToSpawn.Add(FX.Assets.gasPickupPrefab);
	}

	
}