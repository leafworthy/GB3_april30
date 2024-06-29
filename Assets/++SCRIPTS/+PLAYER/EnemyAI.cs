using System;
using UnityEngine;

[RequireComponent(typeof(AstarPathfinder)), RequireComponent(typeof(Life)), RequireComponent(typeof(Targetter)),
 RequireComponent(typeof(EnemyThoughts))]
public class EnemyAI : MonoBehaviour
{
	private enum State
	{
		Wander,
		WalkToTarget,
		AttackTarget,
		AttackObstacle,
		WalkToObstacle
	}

	private State state;
	private int wanderCounter;
	private int wanderRate = 200;

	#region events

	public event Action<Life> OnAttack;
	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;
	public event Action<Life> OnAggro;

	#endregion

	#region components

	private AstarPathfinder pathmaker => GetComponent<AstarPathfinder>();
	private Life life => GetComponent<Life>();
	private Targetter targets => GetComponent<Targetter>();
	private EnemyThoughts thoughts => GetComponent<EnemyThoughts>();

	#endregion

	#region unity events

	private void OnEnable()
	{
		pathmaker.enabled = true;
		pathmaker.OnNewDirection += pathmakerOnOnNewDirection;
		life.OnDamaged += Life_OnDamaged;
		thoughts.Think("Just woke up, wandering.");
		SetState(State.Wander);
	}

	private void OnDisable()
	{
		pathmaker.OnNewDirection -= pathmakerOnOnNewDirection;
		life.OnDamaged -= Life_OnDamaged;
	}

	private void Life_OnDamaged(Attack attack)
	{
		targets.SetSpecialTarget(attack.Owner.spawnedPlayerDefence);
	}

	private void pathmakerOnOnNewDirection(Vector2 newDir)
	{
		OnMoveInDirection?.Invoke(newDir);
	}

	private void FixedUpdate()
	{
		if (Game_GlobalVariables.IsPaused || life.IsDead()) return;
		switch (state)
		{
			case State.WalkToTarget:
				UpdateWalkPathToTarget();
				break;
			case State.AttackTarget:
				UpdateAttackPlayer();
				break;
			case State.WalkToObstacle:
				UpdateWalkToObstacle();
				break;
			case State.AttackObstacle:
				UpdateAttackObstacle();
				break;
			case State.Wander:
				UpdateWander();
				break;
		}
	}

	#endregion

	#region state updates

	private void UpdateWalkToObstacle()
	{
		if (targets.GetCurrentObstacle() == null)
		{
			SetState(State.Wander);
			thoughts.Think("Lost obstacle, wandering.");
			return;
		}

		if (targets.CanAttackObstacle())
		{
			SetState(State.AttackObstacle);
			thoughts.Think("Close enough to attack obstacle.");
			return;
		}

		pathmaker.SetTargetPosition(targets.GetCurrentObstacle().transform.position);
		thoughts.Think("Walking to obstacle.");
	}

	private void UpdateWander()
	{
		
		var target = targets.GetClosestTargetWithinAggroRange(transform.position);
		if (target != null)
		{
			SetState(State.WalkToTarget);
			OnAggro?.Invoke(target);
			thoughts.Think("Found a target, walking to it.");
			return;
		}

		wanderCounter++;
		if (wanderCounter < wanderRate) return;
		pathmaker.SetTargetPosition(targets.GetWanderPosition());
		wanderCounter = 0;



	}

	private void UpdateAttackObstacle()
	{
		if (targets.HasLineOfSightWithCurrentTarget())
		{
			SetState(State.WalkToTarget);
			OnAggro?.Invoke(targets.GetCurrentTarget());
			thoughts.Think("I see a target, walking to it.");
			return;
		}

		var obstacle = targets.GetClosestObstacleWithinAttackRange();
		if (obstacle == null)
		{
			SetState(State.WalkToTarget);
			thoughts.Think("Can't reach obstacle, walking to target.");
			return;
		}

		var obstacleLife = obstacle.GetComponentInChildren<Life>();
		if (obstacleLife == null)
		{
			SetState(State.WalkToTarget);
			thoughts.Think("Obstacle has no life, walking to target.");
			return;
		}

		if (targets.CanAttackObstacle())
			StartAttack(obstacleLife);
		else
			SetState(State.WalkToTarget);
	}

	private void UpdateAttackPlayer()
	{
		if (targets.CanAttackCurrentTarget())
			StartAttack(targets.GetCurrentTarget());
		else
			SetState(State.WalkToTarget);
	}

	private void UpdateWalkPathToTarget()
	{
		if (targets.GetCurrentTarget() == null)
		{
			SetState(State.Wander);
			thoughts.Think("Lost target, wandering.");
			return;
		}

		if (!targets.HasLineOfSightWithCurrentTarget())
		{
			if (targets.CanAttackObstacle())
			{
				SetState(State.AttackObstacle);
				thoughts.Think("Can't reach target, attacking obstacle.");
			}
			else
			{
				SetState(State.Wander);
				thoughts.Think("No line of sight to target, wandering.");
			}

			return;
		}

		if (targets.CanAttackCurrentTarget())
		{
			SetState(State.AttackTarget);
			thoughts.Think("Close enough to attack target.");
			return;
		}

		pathmaker.SetTargetPosition(targets.GetCurrentTarget().transform.position);
	}

	#endregion

	private void StartAttack(Life target)
	{
		OnStopMoving?.Invoke();
		OnAttack?.Invoke(target);
	}

	private void SetState(State newState)
	{
		state = newState;
	}
}