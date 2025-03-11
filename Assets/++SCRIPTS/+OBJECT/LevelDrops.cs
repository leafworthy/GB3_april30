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
	private static bool _isInitialized;


	private void Start()
	{
		if (_isInitialized) return;
		_isInitialized = true;
		EnemyManager.OnEnemyDying += (x, y) => DropLoot(y.transform.position);
		
		// Check if ASSETS is initialized
		if (ASSETS.I == null || ASSETS.FX == null)
		{
			// Delay initialization if assets aren't ready yet
			Invoke(nameof(DelayedInit), 0.1f);
		}
		else
		{
			MakeDropList();
		}
	}
	
	private void DelayedInit()
	{
		MakeDropList();
	}

	public static void DropLoot(Vector2 position, LootType type = LootType.Random)
	{
		// Check if ASSETS is initialized
		if (ASSETS.I == null || ASSETS.FX == null)
		{
			Debug.LogWarning("Can't drop loot: ASSETS not initialized");
			return;
		}
		
		if (_dropsToSpawn.Count == 0) 
		{
			MakeDropList();
		}
		
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
		var prefab = ObjectMaker.Make(lootPrefab, position);
		var fallingObject = prefab.GetComponent<FallToFloor>();
		fallingObject.FireForDrops(new Vector3((float)Random.Range(-1, 1), -1),Color.white, 5, true);

	}
	private static void MakeDropList()
	{
		// Ensure ASSETS and FX are initialized
		if (ASSETS.I == null || ASSETS.FX == null)
		{
			// We can't make the drop list now, it will be tried again later
			return;
		}
		
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