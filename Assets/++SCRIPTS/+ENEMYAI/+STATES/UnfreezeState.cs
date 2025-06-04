using UnityEngine;

namespace GangstaBean.EnemyAI.States._ENEMYAI.EnemyAI_States
{
	public class UnfreezeState : IAIState
	{
		private IAI _ai;
		private Vector2 randomDirection;
		private float unstickTime = 1;
		public void OnEnterState(IAI ai)
		{
			_ai = ai;
			randomDirection = UnityEngine.Random.insideUnitCircle;
			_ai.MoveWithoutPathing(randomDirection);
		}

		public void OnExitState()
		{
		}

		public void UpdateState()
		{
			unstickTime -= Time.deltaTime;
			if (unstickTime <= 0)
			{
				//_ai.TransitionToState(new IdleState());
				_ai.TransitionToState(new AggroState());
				return;
			}
			_ai.MoveWithoutPathing(randomDirection);
		}
	}
}
