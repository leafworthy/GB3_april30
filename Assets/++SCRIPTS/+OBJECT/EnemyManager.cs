using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	private static List<Life> _allEnemies = new();
	public static event Action<Player, Life> OnPlayerKillsEnemy;
	public static event Action<Attack> OnPlayerDamagesEnemy;
	public static event Action<Player, Life> OnEnemyDying;
	


	public static void CollectEnemy(GameObject enemy)
	{
		var enemyDefence = enemy.gameObject.GetComponent<Life>();

		if (enemyDefence == null) return;
		if (_allEnemies.Contains(enemyDefence)) return;
		enemyDefence.OnKilled += EnemyKilled;
		enemyDefence.OnDamaged += EnemyDamaged;
		enemyDefence.OnDying += (x,y) => OnEnemyDying?.Invoke(x,y);
		_allEnemies.Add(enemyDefence);
	}

	private static void EnemyDamaged(Attack attack)
	{
		OnPlayerDamagesEnemy?.Invoke(attack);
	}

	private static void EnemyKilled(Player killer, Life life)
	{
		OnPlayerKillsEnemy?.Invoke(killer, life);
	}

	public static int GetNumberOfLivingZombies()
	{
		return _allEnemies.Count(t => !t.IsDead());
	}


	public static void SpawnEnemy(GameObject ZombiePrefab, Vector3 position)
	{
		var newZombie = ObjectMaker.Make(ZombiePrefab, position);
		var newLife = newZombie.GetComponent<Life>();
		newLife.SetPlayer(Players.EnemyPlayer);
	}
}