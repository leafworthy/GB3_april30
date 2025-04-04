using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class EnemyManager : Singleton<EnemyManager>
	{
		private static List<Life> _allEnemies = new();
		public event Action<Player, Life> OnPlayerKillsEnemy;
		public event Action<Player, Life> OnEnemyDying;
	


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

		private void OnDisable()
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

		public static int GetNumberOfLivingZombies()
		{
			return _allEnemies.Count(t => !t.IsDead());
		}


		public static void SpawnEnemy(GameObject ZombiePrefab, Vector3 position)
		{
			var newZombie = ObjectMaker.I.Make(ZombiePrefab, position);
			var newLife = newZombie.GetComponent<Life>();
			newLife.SetPlayer(Players.EnemyPlayer);
		}
	}
}