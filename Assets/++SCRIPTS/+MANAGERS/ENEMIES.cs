using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ENEMIES : Singleton<ENEMIES>
{
	private static List<DefenceHandler> enemyList = new List<DefenceHandler>();
	public static event Action<IAttackHandler> OnEnemyKilled;
	public static event Action OnAllEnemiesDead;

	private static void AddEnemy(DefenceHandler enemyDefence)
	{
		if (!enemyList.Contains(enemyDefence))
		{
			enemyDefence.OnDead += EnemyDies;
			enemyDefence.OnKilled += EnemyKilled;
			enemyList.Add(enemyDefence);
		}
	}

	private static void EnemyKilled(IAttackHandler killer)
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

	public static DefenceHandler IPlayerControllerEnemy(Vector3 position, float maxRange)
	{
		DefenceHandler closest = null;
		foreach (DefenceHandler enemy in enemyList)
		{
			if (enemy.IsDeadOrDying()) continue;
			if (Vector3.Distance(position, enemy.GetPosition()) <= maxRange)
			{
				if (closest == null)
				{
					closest = enemy;
				}
				else
				{
					if (Vector3.Distance(position, enemy.GetPosition()) <
					    Vector3.Distance(position, closest.GetPosition()))
					{
						closest = enemy;
					}
				}
			}
		}

		return closest;
	}

	public static int GetNumberOfLivingEnemies()
	{
		if (enemyList.Count <= 0)
		{
			CollectAllEnemies();
		}

		return enemyList.Where(t=>!t.IsDead()).ToList().Count;
	}

	public static void CollectAllEnemies()
	{
		var enemies = GameObject.FindObjectsOfType<EnemyController>();
		foreach (EnemyController enemy in enemies)
		{
			var enemyDefence = enemy.gameObject.GetComponent<DefenceHandler>();

			AddEnemy(enemyDefence);

		}
	}
}
