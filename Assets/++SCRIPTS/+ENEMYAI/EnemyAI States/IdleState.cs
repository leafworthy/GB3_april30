using UnityEngine;

public class IdleState : IEnemyState
{
	private float idleCooldown;
	private EnemyAI ai;

	public void OnEnterState(EnemyAI _ai)
	{
		ai = _ai;
		idleCooldown = ai.idleCoolDownMax;
		ai.StopMoving();
		
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

		idleCooldown -= Time.deltaTime;
		if (!(idleCooldown <= 0)) return;
		ai.Thoughts.Think("Wander time.");
		ai.TransitionToState(new WanderState());
	}
}