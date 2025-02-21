using UnityEngine;

public class WanderState : IEnemyState
{
	private float wanderTime;
	private int wanderRate = 4;
	private Vector3 wanderPosition;
	private EnemyAI ai;
	private float closeEnoughWanderDistance = 2;

	public void OnEnterState(EnemyAI _ai)
	{
		ai = _ai;
		StartWandering();
	}

	private void StartWandering()
	{
		wanderTime = 0;
		wanderPosition = ai.Targets.GetWanderPosition(ai.WanderPoint, ai.WanderRadius);
		ai.Pathmaker.SetTargetPosition(wanderPosition);
	}

	public void OnExitState()
	{
	}

	public void UpdateState()
	{
		wanderTime += Time.deltaTime;
		if (ai.FoundTargetInAggroRange())
		{
			ai.Thoughts.Think("Found target in aggro range, going aggro");
			ai.TransitionToState(new AggroState());
			return;
		}

		if (wanderTime >= wanderRate)
		{
			if (ai.idleCoolDownMax > 0)
			{
				ai.Thoughts.Think("Wandered long enough, going idle: " + wanderPosition);
				ai.TransitionToState(new IdleState());
				return;
			}
			else
			{
				StartWandering();
				return;
			}
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