using __SCRIPTS._ENEMYAI;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteInEditMode]
	public class EnemyAIDebugVisualizer : MonoBehaviour
	{
		public Color aggroRangeColor = new Color(1f, 0f, 0f, 0.3f); // Red
		public Color attackRangeColor = new Color(1f, 1f, 0f, 0.3f); // Yellow
		public Color wanderRangeColor = new Color(0f, 0f, 1f, 0.3f); // Blue


		private EnemyAI enemyAI;

		private void Awake()
		{
			enemyAI = GetComponent<EnemyAI>();
		}

		private void OnDrawGizmos()
		{
			if (enemyAI == null)
			{
				return;
			}

			// Draw Aggro Range
			Gizmos.color = aggroRangeColor;
			Gizmos.DrawSphere(enemyAI.transform.position, enemyAI.Life.AggroRange);

			// Draw Attack Range
			Gizmos.color = attackRangeColor;
			Gizmos.DrawSphere(enemyAI.transform.position, enemyAI.Life.PrimaryAttackRange);

			// Draw Wander Range
			Gizmos.color = wanderRangeColor;
			Gizmos.DrawSphere(enemyAI.Targets.WanderPoint, enemyAI.Targets.WanderRadius);
		}
	}
}
