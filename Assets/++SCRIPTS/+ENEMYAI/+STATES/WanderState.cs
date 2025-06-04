using UnityEngine;

namespace GangstaBean.EnemyAI.States._ENEMYAI.EnemyAI_States
{
	public class WanderState : IAIState
	{
		private float wanderTime;
		private int wanderRate = 4;
		private Vector3 wanderPosition;
		private IAI ai;
		private float closeEnoughWanderDistance = 2;

		public void OnEnterState(IAI _ai)
		{
			ai = _ai;
			StartWandering();
		}

		private void StartWandering()
		{
			wanderTime = 0;
			wanderPosition = ai.Targets.GetWanderPosition(ai.Targets.WanderPoint, ai.Targets.WanderRadius);
			ai.Pathmaker.SetTargetPosition(wanderPosition);
		}

		public void OnExitState()
		{
		}

		public void UpdateState()
		{
			wanderTime += Time.deltaTime;
			if (ai.Targets.FoundTargetInAggroRange())
			{
				ai.Thoughts.Think("Found target in aggro range, going aggro");
				ai.TransitionToState(new AggroState());
				return;
			}

			if (wanderTime >= wanderRate)
			{

					StartWandering();
					return;

			}

			if (IsCloseEnoughToWanderPosition())
			{
				if (ai.idleCoolDownMax == 0)
				{
					StartWandering();
					return;
				}
				ai.Thoughts.Think("At wander point, going idle: " + wanderPosition);
				ai.TransitionToState(new IdleState());
			}
			else
			{
				ai.Thoughts.Think("Wandering to position: " + wanderPosition);
			}
		}

		private bool IsCloseEnoughToWanderPosition() => Vector2.Distance(ai.transform.position, wanderPosition) < closeEnoughWanderDistance;
	}
}
