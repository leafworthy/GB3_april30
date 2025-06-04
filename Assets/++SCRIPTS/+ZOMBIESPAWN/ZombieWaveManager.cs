using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GangstaBean.ZombieSpawn._ZOMBIESPAWN
{
	public class ZombieWaveManager : MonoBehaviour
	{
		private  int currentWaveIndex;
		private  bool isComplete;
		public static  Action OnWaveEnd;
		private  float timeTillNextWave;

		public  bool BetweenWaves;
		private  ZombieSpawnPoint[] ZombieSpawnPoints;

		public List<ZombieWave> Waves = new();

		public bool isActive;
		//what?
		private void Start()
		{
			if(!isActive) return;
			isComplete = false;
		
			currentWaveIndex = 0;
			ZombieSpawnPoints = FindObjectsByType<ZombieSpawnPoint>(FindObjectsSortMode.None);
			SpawnNextWave();
		}
	
		public  float GetTimeTillNextWave()
		{
			return timeTillNextWave;
		}

		public  int GetCurrentWaveIndex()
		{
			return currentWaveIndex;
		}

		private void FixedUpdate()
		{
			if (!isActive) return;
			if(isComplete) return;
			if (!BetweenWaves)
			{
				var incompleteSpawnPoints = ZombieSpawnPoints.Where(x => x.isComplete == false).ToList();
				if (incompleteSpawnPoints.Count == 0)
				{
					//Debug.Log("all spawn points complete");
					if (EnemyManager.GetNumberOfLivingZombies() <= 0)
					{
						//Debug.Log("wave end");
						OnWaveEnd?.Invoke();
						BetweenWaves = true;
						timeTillNextWave = 10f;
					}
				}
			}
			else
			{

				if (timeTillNextWave > 0) timeTillNextWave -= Time.fixedDeltaTime;
				if (timeTillNextWave < 0)
				{
					//Debug.Log("Time's up");
					SpawnNextWave();
					timeTillNextWave = 0;
					BetweenWaves = false;
				}
			}
		}


		public void SpawnNextWave()
		{
			if (!isActive) return;
			if(isComplete) return;
		
			//Debug.Log("Spawn next wave");
			BetweenWaves = false;
			currentWaveIndex++;
			foreach (var spawnPoint in ZombieSpawnPoints)
			{
				//Debug.Log("starting wave" + currentWaveIndex + " at spawnpoint " + spawnPoint.name);

				spawnPoint.StartWave(Waves[currentWaveIndex - 1]);
			}

		
		

			return;
		}

		public  int GetWavesRemaining()
		{
			return  Waves.Count - currentWaveIndex;
		}
	}
}