using UnityEngine;

namespace GangstaBean.EnemyAI.States._ENEMYAI.EnemyAI_States
{
	public class IdleState : IAIState
	{
		private float idleCooldown;
		private IAI ai;
		private float idleCoolDownMax = 2;

		public void OnEnterState(IAI _ai)
		{
			ai = _ai;
			idleCooldown = idleCoolDownMax;
			ai.StopMoving();

		}

		public void OnExitState()
		{
		}

		public void UpdateState()
		{
			if (ai.Targets.GetClosestPlayerInAggroRange() != null)
			{
				ai.Thoughts.Think("Found target in aggro range, going aggro.");
				ai.TransitionToState(new AggroState());
				return;
			}

			idleCooldown -= Time.deltaTime;
			if (!(idleCooldown <= 0)) return;
			ai.Thoughts.Think("Wander time.");
			ai.TransitionToState(new WanderState());
		}
	}
}
