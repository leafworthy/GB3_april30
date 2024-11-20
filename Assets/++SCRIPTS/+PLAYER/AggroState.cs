using UnityEngine;

public class AggroState : IEnemyState
{
	private Life target;
	private EnemyAI ai;

	public void OnEnterState(EnemyAI _ai)
	{
		ai = _ai;
		if (IdleIfNoTargetInAggroRange()) return;
	
	}

	private bool IdleIfNoTargetInAggroRange()
	{
		target = ai.Targets.GetClosestPlayerInAggroRange();
		if (target != null) return false;
		ai.Thoughts.Think("idling, no target in aggro range");
		ai.TransitionToState(new IdleState());
		return true;

	}

	private bool CanAttackObstacleWithinAttackRange()
	{
		var closestObstacle = ai.Targets.GetClosestAttackableObstacle();
		Debug.Log("closest obstacle: " + closestObstacle, closestObstacle);
		if (closestObstacle == null) return false;
		if(ai.Targets.HasLineOfSightWith(target.transform.position))
		{
			Debug.Log("not attacking because target in sight");
			 ai.Thoughts.Think("Not attacking because target in sight");
			return false;
		}

		ai.Thoughts.Think("attacking obstacle " + closestObstacle.name);
		ai.TransitionToState(new AttackObstacleState());
		return true;
	}

	

	public void OnExitState()
	{
	}

	public void UpdateState()
	{
		if (IdleIfNoTargetInAggroRange()) return;
		if (ai.Targets.HasLineOfSightWith(target.transform.position))
		{
			WalkDirectlyToTarget();
			ai.Thoughts.Think("Walking to target position " + target.transform.position);
			if (CloseEnoughToAttack()) ai.TransitionToState(new AttackPlayerState());
			return;
		}
		if (CanAttackObstacleWithinAttackRange()) return;
		if (CloseEnoughToAttack()) ai.TransitionToState(new AttackPlayerState());
		ai.Thoughts.Think("Pathing to target position " + target.transform.position);
		ai.Pathmaker.SetTargetPosition(target.transform.position);
	}

	private void WalkDirectlyToTarget()
	{
		ai.Pathmaker.WalkInDirectionOfTarget(target.transform.position);
	}

	private bool CloseEnoughToAttack() =>
		Vector2.Distance(ai.transform.position, target.transform.position) <= ai.Life.AttackRange;
}