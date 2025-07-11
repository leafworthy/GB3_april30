using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class EnemyManager : MonoBehaviour, IService
	{
		private static List<Life> _allEnemies = new();
		public event Action<Player, Life> OnPlayerKillsEnemy;
		public event Action<Player, Life> OnEnemyDying;
		private LevelManager _levelManager;
		private LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();



		public void StartService()
		{
			levelManager.OnStopLevel += ClearEnemies;
		}

		public void CollectEnemy(GameObject enemy)
		{
			var enemyDefence = enemy.gameObject.GetComponent<Life>();

			if (enemyDefence == null) return;
			if (_allEnemies.Contains(enemyDefence)) return;
			enemyDefence.OnKilled += EnemyKilled;
			enemyDefence.OnDying += EnemyDying;;
			_allEnemies.Add(enemyDefence);
		}

		private void EnemyDying(Player x, Life y)
		{
			OnEnemyDying?.Invoke(x, y);
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



		private  void EnemyKilled(Player killer, Life life)
		{
			OnPlayerKillsEnemy?.Invoke(killer, life);
		}

	}
}
