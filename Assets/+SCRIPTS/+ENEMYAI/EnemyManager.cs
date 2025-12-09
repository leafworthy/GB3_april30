using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class EnemyManager : MonoBehaviour, IService
	{
		private List<Life> _allEnemies = new();
		public event Action<Player, IGetAttacked> OnPlayerKillsEnemy;
		public event Action<IGetAttacked> OnEnemyDying;
		private LevelManager _levelManager;
		private LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();
		private Players _players;
		private Players players => _players ?? ServiceLocator.Get<Players>();

		private ObjectMaker objectMaker => _objectMaker ??= ServiceLocator.Get<ObjectMaker>();

		private ObjectMaker _objectMaker;

		public void StartService()
		{
			levelManager.OnStopLevel += ClearEnemies;
		}

		public GameObject SpawnNewEnemy(GameObject enemyPrefab, EnemySpawner.EnemyType enemyType, Vector3 position, int enemyTier)
		{
			var newEnemy = objectMaker.Make(enemyPrefab, position);
			ConfigureNewEnemy(newEnemy, enemyType,enemyTier);
			return newEnemy;
		}

		public void SpawnNewNPC(GameObject NPCPrefab, Vector3 position)
		{
			var newNPC = objectMaker.Make(NPCPrefab, position);
			ConfigureNewNPC(newNPC);
		}

		private void CollectEnemy(GameObject enemy)
		{
			var enemyDefence = enemy.gameObject.GetComponent<Life>();

			if (enemyDefence == null) return;
			if (_allEnemies.Contains(enemyDefence)) return;
			enemyDefence.OnKilled += EnemyKilled;
			enemyDefence.OnDead += EnemyDead;
			_allEnemies.Add(enemyDefence);
		}

		private void EnemyDead(Attack attack)
		{
			var loot = ServiceLocator.Get<LootTable>();
			loot.DropLoot(attack.DestinationLife.transform.position);
			OnEnemyDying?.Invoke(attack.DestinationLife);
		}

		private void ClearEnemies(GameLevel gameLevel)
		{
			foreach (var enemy in _allEnemies)
			{
				enemy.OnKilled -= EnemyKilled;
				enemy.OnDead -= EnemyDead;
			}

			_allEnemies.Clear();
		}

		private void EnemyKilled(Player killer, IGetAttacked life)
		{
			OnPlayerKillsEnemy?.Invoke(killer, life);
		}

		public void ConfigureNewEnemy(GameObject enemy, EnemySpawner.EnemyType enemyType, int enemyTier = 0)
		{
			// Set up the Life component
			var life = enemy.GetComponent<Life>();
			if (life != null)
			{
				life.SetPlayer(players.enemyPlayer);
				life.SetEnemyTypeAndTier(enemyType,enemyTier);

			}

			CollectEnemy(enemy);
		}

		public void ConfigureNewNPC(GameObject npc)
		{
			var life = npc.GetComponent<Life>();
			if (life != null)
			{
				life.SetPlayer(players.NPCPlayer);
			}

			CollectEnemy(npc);
		}
	}
}
