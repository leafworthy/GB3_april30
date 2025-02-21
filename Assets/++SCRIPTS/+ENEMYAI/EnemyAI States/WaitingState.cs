public class WaitingState : IEnemyState
{
	private EnemyAI ai;
	public void OnEnterState(EnemyAI _ai)
	{
		ai = _ai;
	}

	public void OnExitState()
	{
	}

	public void UpdateState()
	{
		if (ai.FoundTargetInAggroRange())
		{
			ai.Thoughts.Think("Found target in aggro range, going aggro.");
			ai.TransitionToState(new AggroState());
			return;
		}
	}
}