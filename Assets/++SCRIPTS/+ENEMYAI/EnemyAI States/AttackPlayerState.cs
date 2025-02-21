public class AttackPlayerState : IEnemyState
{
	private Life target;
	private EnemyAI ai;
	

	public void OnEnterState(EnemyAI _ai)
	{ 
		ai = _ai;
		target = _ai.Targets.GetClosestAttackablePlayer();
		if (target == null)
		{
			_ai.TransitionToState(new AggroState());
			return;
		}

		_ai.Attack(target);
	}

	public void OnExitState()
	{
	}

	public void UpdateState()
	{
		/*
		if (!ai.FoundTargetInAggroRange())
		{
			ai.Thoughts.Think("No Target Found, going idle!");
			ai.TransitionToState(new IdleState());
			return;
		}*/

		target = ai.Targets.GetClosestAttackablePlayer();
		if (target != null)
		{
			ai.Thoughts.Think("Attacking Player!" + target.name);
			ai.Attack(target);
		}
		else
		{
			ai.Thoughts.Think("no targets, going aggro!");
			ai.TransitionToState(new AggroState());
		}
	}
}

public class AttackObstacleState : IEnemyState
{
	private Life target;
	private EnemyAI ai;
	public void OnEnterState(EnemyAI _ai)
	{
		ai = _ai;
		AttackObstacle();
	}

	private void AttackObstacle()
	{
		var player = ai.Targets.GetClosestPlayer();
		if(player == null)
		{
			ai.Thoughts.Think("No more player, wander time");
			ai.TransitionToState(new WanderState());
			return;
		}
		target = ai.Targets.GetClosestAttackableObstacle();
		if (target != null && !target.IsDead())
		{
			
			ai.Thoughts.Think("Attacking Door!");
			ai.Attack(target);
		}
		else
		{
			ai.Thoughts.Think("No more door, Going back to aggro");
			ai.TransitionToState(new AggroState());
		}
	}

	public void OnExitState()
	{
	}

	public void UpdateState()
	{
		AttackObstacle();
	}
}