using UnityEngine;

namespace __SCRIPTS._ENEMYAI.EnemyAI_States
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
			if (ai.Targets.GetClosestPlayer() != null)
			{
				ai.TransitionToState(new AggroState());
				return;
			}

			idleCooldown -= Time.deltaTime;
			if (!(idleCooldown <= 0)) return;
		}
	}
}
