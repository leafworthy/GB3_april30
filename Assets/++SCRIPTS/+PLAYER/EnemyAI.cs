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
		WalkToObstacle,
		Idle
	}

	private State state;
	private int wanderCounter;
	private int wanderRate = 200;
	private float wanderDistance = 30;
	private Vector2 wanderPoint;
	private Vector2 currentTargetWanderPosition;
	private float closeEnoughWanderDistance = 5;
	private float idleCoolDown;
	public float idleCoolDownMax = 2;
	private bool isWandering;

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
		if (GlobalManager.IsPaused || life.IsDead()) return;
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
			case State.Idle:
				UpdateIdle();
				break;
		}
	}

	private void UpdateIdle()
	{
		var target = targets.GetCurrentTarget();
		if (target != null)
		{
			SetState(State.WalkToTarget);
			thoughts.Think("Walk to target");
			return;
		}

		if (targets.GetCurrentObstacle() != null)
		{
			Debug.DrawLine( transform.position, targets.GetCurrentObstacle().transform.position, Color.green);
			SetState(State.WalkToObstacle);
			thoughts.Think("Walk to obstacle.");
			return;
		}

		if (idleCoolDown > 0)
		{
			idleCoolDown -= Time.fixedDeltaTime;
			OnStopMoving?.Invoke();
			thoughts.Think("Idle.");
			return;
		}
		SetState(State.Wander);
		idleCoolDown = 0;
		thoughts.Think("Nothing to do, wandering.");
	}

	#endregion

	#region state updates

	private void UpdateWalkToObstacle()
	{
		
		
		if (targets.GetCurrentObstacle() == null)
		{
			SetState(State.WalkToTarget);
			thoughts.Think("Obstacle gone, walk to target.");
			return;
		}

		Debug.DrawLine(transform.position, targets.GetCurrentObstacle().transform.position, Color.green);

		if (targets.CanAttackObstacle())
		{
			SetState(State.AttackObstacle);
			thoughts.Think("Close enough to attack obstacle.");
			Debug.DrawLine(transform.position, targets.GetCurrentObstacle().transform.position, Color.red, 1);
			return;
		}

		pathmaker.SetTargetPosition(targets.GetCurrentObstacle().transform.position);
		thoughts.Think("Walking to obstacle.");
	}

	private void UpdateWander()
	{
		wanderCounter++;
		if (wanderCounter >= wanderRate)
		{
			wanderCounter = 0;
			SetState(State.Wander);
			thoughts.Think("Wonder.");
			isWandering = false;
		}
		if (!isWandering)
		{
			isWandering = true;
			currentTargetWanderPosition = targets.GetWanderPosition(wanderPoint);
			if(currentTargetWanderPosition == Vector2.zero)
			{
				StartIdle();
				isWandering = false;
				thoughts.Think("Can't find a spot to wander, idling.");
				return;
			}
			pathmaker.SetTargetPosition(currentTargetWanderPosition);
			thoughts.Think("Wandering to spot");
			
		}

	
		var target = targets.GetClosestTargetWithinAggroRange(transform.position);
		if (target != null)
		{
			isWandering = false;
			SetState(State.WalkToTarget);
			OnAggro?.Invoke(target);
			thoughts.Think("Found a target, walking to it.");
			return;
		}

		var distanceToTargetPosition = Vector2.Distance(transform.position, currentTargetWanderPosition);
		pathmaker.SetTargetPosition(currentTargetWanderPosition);
		Debug.DrawLine( transform.position, currentTargetWanderPosition, Color.red);
		
		if (!(distanceToTargetPosition < closeEnoughWanderDistance)) return;
		isWandering = false;
		StartIdle();



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

		if (targets.CanAttackObstacle())
			StartAttack(targets.GetCurrentObstacle());
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

		if (targets.CanAttackObstacle())
		{
			SetState(State.AttackObstacle);
			thoughts.Think("Attacking obstacle.");
			return;
		}
		else
		{
			Debug.Log("can't attack obstacle");
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

	private void StartIdle()
	{
		idleCoolDown = idleCoolDownMax;
		SetState(State.Idle);
		OnStopMoving?.Invoke();
	}

	private void StartAttack(Life target)
	{
		OnStopMoving?.Invoke();
		OnAttack?.Invoke(target);
	}

	private void SetState(State newState)
	{
		state = newState;
	}

	public void SetWanderPoint(Vector2 newWanderPoint)
	{
		wanderPoint = newWanderPoint;
	}
}