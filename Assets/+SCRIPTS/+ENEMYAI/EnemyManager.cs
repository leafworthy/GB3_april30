using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class EnemyManager : MonoBehaviour, IService
	{
		private List<Life> _allEnemies = new();
		public event Action<Player, Life> OnPlayerKillsEnemy;
		public event Action<Life> OnEnemyDying;
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

		public void SpawnNewEnemy(GameObject enemyPrefab, Vector3 position)
		{
			var newEnemy = objectMaker.Make(enemyPrefab, position);
			ConfigureNewEnemy(newEnemy);
		}

		private void CollectEnemy(GameObject enemy)
		{
			var enemyDefence = enemy.gameObject.GetComponent<Life>();

			if (enemyDefence == null) return;
			if (_allEnemies.Contains(enemyDefence)) return;
			Debug.Log("collected enemy: " + enemyDefence.name);
			enemyDefence.OnKilled += EnemyKilled;
			enemyDefence.OnDying += EnemyDying;
			_allEnemies.Add(enemyDefence);
		}

		private void EnemyDying(Attack attack)
		{
			//wtf
			//wrong line bro
			Debug.Log("Attack properties || " + "origin life name: " +attack.OriginLife.name + " | destination life name: " + (attack.DestinationLife == null ? "null" : attack.DestinationLife.name));
			OnEnemyDying?.Invoke(attack.DestinationLife);
		}

		private void ClearEnemies(GameLevel gameLevel)
		{
			foreach (var enemy in _allEnemies)
			{
				enemy.OnKilled -= EnemyKilled;
				enemy.OnDying -= EnemyDying;
			}

			_allEnemies.Clear();
		}

		private void EnemyKilled(Player killer, Life life)
		{
			OnPlayerKillsEnemy?.Invoke(killer, life);
		}

		public void ConfigureNewEnemy(GameObject enemy)
		{
			Debug.Log("configuring new enemy");
			// Set up the Life component
			var life = enemy.GetComponent<Life>();
			if (life != null)
			{
				life.SetPlayer(players.enemyPlayer);
				life.AddHealth(life.MaxHealth);
			}

			CollectEnemy(enemy);
		}
	}
}
