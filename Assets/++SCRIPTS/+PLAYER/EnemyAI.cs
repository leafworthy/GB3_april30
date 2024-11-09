using System;
using UnityEngine;

public interface IEnemyState
{
	void OnEnterState(EnemyAI enemy);
	void OnExitState();
	void UpdateState(EnemyAI enemy);
}

public class IdleState : IEnemyState
{
	private float idleCooldown;

	public void OnEnterState(EnemyAI enemy)
	{
		idleCooldown = enemy.idleCoolDownMax;
		enemy.StopMoving();
		enemy.Thoughts.Think("Entering Idle State.");
	}

	public void OnExitState()
	{
		// Nothing specific to do on exit for now
	}

	public void UpdateState(EnemyAI enemy)
	{
		if (enemy.FoundAnAggroTarget())
		{
			enemy.TransitionToState(new AggroState());
			return;
		}

		idleCooldown -= Time.deltaTime;
		if (idleCooldown <= 0) enemy.TransitionToState(new WanderState());
	}
}

public class WanderState : IEnemyState
{
	private int wanderCounter;
	private int wanderRate = 200;
	private Vector3 wanderPosition;
	private EnemyAI _enemy;
	private float closeEnoughWanderDistance = 2;

	public void OnEnterState(EnemyAI enemy)
	{
		_enemy = enemy;
		wanderCounter = 0;
		wanderPosition = _enemy.Targets.GetWanderPosition(enemy.WanderPoint , enemy.WanderRadius);
		_enemy.Pathmaker.SetTargetPosition(wanderPosition);
		_enemy.Thoughts.Think("Wandering...");
	}

	public void OnExitState()
	{
	}

	public void UpdateState(EnemyAI enemy)
	{
		Debug.Log("wandering");
		wanderCounter++;
		if (enemy.FoundAnAggroTarget())
		{
			Debug.Log("HERE!!!!!!!");
			enemy.TransitionToState(new AggroState());
			return;
		}

		if (wanderCounter >= wanderRate)
		{
			enemy.TransitionToState(new IdleState());
			return;
		}

		if (IsCloseEnoughToWanderPosition()) enemy.TransitionToState(new IdleState());
	}

	private bool IsCloseEnoughToWanderPosition()
	{
		
			var distanceToWanderPosition = Vector2.Distance(_enemy.transform.position, wanderPosition);
			return distanceToWanderPosition < closeEnoughWanderDistance; // closeEnoughWanderDistance
		
	}
}

public class AggroState : IEnemyState
{
	private Life target;

	public void OnEnterState(EnemyAI enemy)
	{
		if (HasNoTarget(enemy))
		{
			Debug.Log("no target");
			enemy.TransitionToState(new IdleState());
			return;
		}
		enemy.IsAggro = true;
		enemy.Thoughts.Think("Aggro!");
		enemy.Pathmaker.SetTargetPosition(target.transform.position);
	}

	private bool HasNoTarget(EnemyAI enemy)
	{
		target = enemy.Targets.GetClosestPlayerInAggroRange();
		return target == null;
	}

	public void OnExitState()
	{
		// Nothing specific to do on exit for now
	}

	public void UpdateState(EnemyAI enemy)
	{
		if (HasNoTarget(enemy))
		{
			enemy.TransitionToState(new IdleState());
			return;
		}

		if(CloseEnoughToAttack(enemy)) enemy.TransitionToState(new AttackState());

		enemy.Pathmaker.SetTargetPosition(target.transform.position);
	}

	private bool CloseEnoughToAttack(EnemyAI enemy) => (Vector2.Distance(enemy.transform.position, target.transform.position) <= enemy.Life.AttackRange);
}

public class AttackState : IEnemyState
{
	private Life target;
	public void OnEnterState(EnemyAI enemy)
	{
		target = enemy.Targets.GetClosestPlayerInAttackRange();
		if (target == null)
		{
			enemy.TransitionToState( new AggroState());
			return;
		}
		enemy.Thoughts.Think("Attacking!");
		enemy.StopMoving();
	}

	public void OnExitState()
	{
		// Nothing specific to do on exit for now
	}

	public void UpdateState(EnemyAI enemy)
	{
		if (!enemy.FoundAnAggroTarget())
		{
			enemy.TransitionToState(new IdleState());
			return;
		}
		target = enemy.Targets.GetClosestPlayerInAttackRange();
		if (target == null)
		{
			enemy.TransitionToState(new AggroState());
			return;
		}
		// Attack Logic
		enemy.Attack(target);
	}
}

public class EnemyAI : MonoBehaviour
{
	private IEnemyState currentState;

	// Public Properties and Components (keeping your existing structure)
	public float idleCoolDownMax = 2;
	private Vector2 wanderPoint;
	public float WanderRadius = 20;
	public bool IsAggro { get; set; }

	public AstarPathfinder Pathmaker => GetComponent<AstarPathfinder>();
	public Life Life => GetComponent<Life>();
	public Targetter Targets => GetComponent<Targetter>();
	public EnemyThoughts Thoughts => GetComponent<EnemyThoughts>();
	public Vector2 WanderPoint
	{
		get => wanderPoint;
		private set => wanderPoint = value;
	}
	

	public event Action<Life> OnAttack;
	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;

	private void Start()
	{
		WanderPoint = transform.position;
		Pathmaker.OnNewDirection += (direction) => OnMoveInDirection?.Invoke(direction);
		TransitionToState(new IdleState()); // Start in Idle State
	}

	private void Update()
	{
		if (GlobalManager.IsPaused || Life.IsDead()) return;

		currentState?.UpdateState(this);
	}

	public void TransitionToState(IEnemyState newState)
	{
		currentState?.OnExitState();
		currentState = newState;
		currentState.OnEnterState(this);
	}

	// Additional helper methods can be exposed for states


	public bool FoundAnAggroTarget()
	{
		var target = Targets.GetClosestPlayerInAggroRange();
		return target != null;
	}



	public void StopMoving()
	{
		OnStopMoving?.Invoke();
		Pathmaker.StopPathing();
		Thoughts.Think("Stopping movement");
	}

	public void Attack(Life targetsCurrentTarget)
	{
		OnAttack?.Invoke(targetsCurrentTarget);
		Thoughts.Think("Attacking");
	}
}