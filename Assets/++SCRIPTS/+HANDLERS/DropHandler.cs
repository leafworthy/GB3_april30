using System.Collections.Generic;
using UnityEngine;

public class DropHandler : MonoBehaviour
{

	private DefenceHandler playerHealth;
	private List<GameObject> dropsToSpawn = new List<GameObject>();
	private void Start()
	{
		playerHealth = GetComponent<DefenceHandler>();
		playerHealth.OnDying += Drop;


		dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.ammoPickupPrefab);

		dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.cashPickupPrefab);

		dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.healthPickupPrefab);

		dropsToSpawn.Add(ASSETS.FX.nadesPickupPrefab);
		dropsToSpawn.Add(ASSETS.FX.nadesPickupPrefab);

		dropsToSpawn.Add(ASSETS.FX.damagePickupPrefab);

		dropsToSpawn.Add(ASSETS.FX.speedPickupPrefab);
	}

	private void Drop()
	{
		var rand = dropsToSpawn[Random.Range(0, dropsToSpawn.Count)];
		var prefab = MAKER.Make(rand, transform.position);
		var fallingObject = prefab.AddComponent<FallToFloor>();
		fallingObject.rotationRate = 0f;
		fallingObject.hasDeathTime = false;
		fallingObject.Fire(new Vector3((float)Random.Range(-1,1), 1),.5f, 5);
	}
}
