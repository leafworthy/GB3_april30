namespace __SCRIPTS._ENEMYAI.EnemyAI_States
{
	public class WaitingState : IAIState
	{
		private IAI ai;
		public void OnEnterState(IAI _ai)
		{
			ai = _ai;
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
		}
	}
}
