using UnityEngine;

namespace __SCRIPTS._ZOMBIESPAWN
{
	public class ZombieSpawnPoint : MonoBehaviour
	{
	
		private ZombieWave currentWave;
		private float cooldownTime;
		public bool isComplete;

		private void Start()
		{
			isComplete = false;
		}

		private void FixedUpdate()
		{
			if(currentWave == null) return;
			if(currentWave.ZombiesToSpawn.Count == 0)
			{
				isComplete = true;
				// Debug.Log("spawn point complete", this);
				return;
			}

			cooldownTime -= Time.deltaTime;
			if (!(cooldownTime <= 0)) return;
		
			Spawn();
			cooldownTime = currentWave.TimeBetweenSpawns;
		}

		private void Spawn()
		{ 
			var newPrefab = currentWave.ZombiesToSpawn.GetRandom();
			currentWave.ZombiesToSpawn.Remove(newPrefab);
			var newZombie = ObjectMaker.I.Make(newPrefab, transform.position);
			var newLife = newZombie.GetComponent<Life>();
			newLife.SetPlayer(Players.EnemyPlayer);
			EnemyManager.I.CollectEnemy(newZombie);
		}

		public void StartWave(ZombieWave newWave)
		{
			currentWave = newWave;
			isComplete = false;
		}

	
	}
}