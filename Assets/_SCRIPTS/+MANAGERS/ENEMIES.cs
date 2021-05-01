using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _SCRIPTS
{
	public class ENEMIES : Singleton<ENEMIES>
	{
		private static List<DefenceHandler> enemyList = new List<DefenceHandler>();

		public static void AddEnemy(DefenceHandler enemyDefence)
		{
			if (!enemyList.Contains(enemyDefence))
			{
				enemyList.Add(enemyDefence);
			}
		}

		public static DefenceHandler GetClosestEnemy(Vector3 position, float maxRange)
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

		public static int GetNumberOfEnemies()
		{
			return enemyList.Count;
		}

		public static int GetNumberOfLivingEnemies()
		{
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
}
