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
			if (ai.Targets.FoundTargetInAggroRange())
			{
				ai.Thoughts.Think("Found target in aggro range, going aggro.");
				ai.TransitionToState(new AggroState());
				return;
			}
		}
	}
}
