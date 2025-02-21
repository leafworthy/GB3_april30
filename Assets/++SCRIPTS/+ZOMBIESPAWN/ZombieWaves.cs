using System.Collections.Generic;
using UnityEngine;

public class ZombieWaves : Singleton<ZombieWaves>
{
	public int SpawnsLeft = 10;
	public int FramesBetweenSpawns = 100;
	public List<GameObject> EnemyPrefabs = new();
	public List<Life> SpawnedZombies = new();
	private int currentPrefabIndex;
	private int counter;
	public List<GameObject> spawnPoints;
	public  bool isOn;
	private List<ZombieAttractor> attractors = new();
	private List<GameObject> attractorGOs = new();


	public void UpdateWave()
	{
		if(SpawnsLeft <= 0) return;
		counter++;
		if (counter <= FramesBetweenSpawns) return;
		SpawnsLeft--;
		Spawn();
		counter = 0;
	}

	private void Spawn()
	{
		var newEnemy = ObjectMaker.Make(EnemyPrefabs[currentPrefabIndex], ZombieWaves.I.spawnPoints.GetRandom().transform.position);
		var enemyLife = newEnemy.GetComponent<Life>();
		SpawnedZombies.Add(newEnemy.GetComponentInChildren<Life>());
		enemyLife.SetPlayer(Players.EnemyPlayer);
		EnemyManager.CollectEnemy(newEnemy);
		currentPrefabIndex++;
		if (currentPrefabIndex >= EnemyPrefabs.Count)
			currentPrefabIndex = 0;
	}




	public void Start()
	{
		RefreshAttractorGOs();
	}

	private void FixedUpdate()
	{
		if (!isOn) return;
		UpdateWave();
	}

	public List<Life> FindAllZombieAttractors()
	{
		RefreshAttractorGOs();
		var attractorLives = new List<Life>();
		foreach (var attractorGO in attractorGOs)
		{
			var life = attractorGO.GetComponentInChildren<Life>();
			if (life == null) continue;
			attractorLives.Add(life);
		}

		return attractorLives;
	}

	private void RefreshAttractorGOs()
	{
		attractorGOs.Clear();
		foreach (var zombieAttractor in attractors)
		{
			if (zombieAttractor == null) continue;
			if (!zombieAttractor.gameObject.activeInHierarchy) continue;
			attractorGOs.Add(zombieAttractor.gameObject);
		}
	}
}