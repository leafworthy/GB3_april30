using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ENEMIES : Singleton<ENEMIES>
{
	private static List<DefenceHandler> Enemies = new List<DefenceHandler>();
	public static event Action<Player> OnEnemyKilled;
	public static event Action OnAllEnemiesDead;

	private void Start()
	{
		LEVELS.OnLevelStart += OnLevelStart;
	}

	private void OnLevelStart(List<Player> obj)
	{
		CollectAllEnemies();
	}

	private static void CollectEnemy(DefenceHandler enemyDefence)
	{
		if (Enemies.Contains(enemyDefence)) return;
		enemyDefence.OnDead += EnemyDies;
		enemyDefence.OnKilled += EnemyKilled;
		Enemies.Add(enemyDefence);
	}

	public static void EnemyKilled(Player killer)
	{
		OnEnemyKilled?.Invoke(killer);
	}

	private static void EnemyDies()
	{
		if (GetNumberOfLivingEnemies() <= 0)
		{
			OnAllEnemiesDead?.Invoke();
		}
	}



	public static int GetNumberOfLivingEnemies()
	{
		if (Enemies.Count <= 0)
		{
			CollectAllEnemies();
		}

		return Enemies.Where(t=>!t.IsDead()).ToList().Count;
	}

	private static void CollectAllEnemies()
	{
		var enemies = GameObject.FindObjectsOfType<EnemyController>();
		foreach (EnemyController enemy in enemies)
		{
			var enemyDefence = enemy.gameObject.GetComponent<DefenceHandler>();

			CollectEnemy(enemyDefence);

		}
	}
}
