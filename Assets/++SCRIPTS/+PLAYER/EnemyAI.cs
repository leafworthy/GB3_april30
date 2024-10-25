using System;
using UnityEngine;

[RequireComponent(typeof(AstarPathfinder)), RequireComponent(typeof(Life)), RequireComponent(typeof(Targetter)),
 RequireComponent(typeof(EnemyThoughts))]
public class EnemyAI : MonoBehaviour
{
	private enum State
	{
		Idle,
		Wandering,
		Aggro
	}

	private int wanderCounter;
	private int wanderRate = 200;
	private float wanderDistance = 10;
	private Vector2 wanderPoint;
	private Vector2 currentWanderPosition;
	private float closeEnoughWanderDistance = 3;
	private float idleCoolDown;
	public float idleCoolDownMax = 2;

	private State state;
	private AstarPathfinder pathmaker => GetComponent<AstarPathfinder>();
	private Life life => GetComponent<Life>();
	private Targetter targets => GetComponent<Targetter>();
	private EnemyThoughts thoughts => GetComponent<EnemyThoughts>();

	public event Action<Life> OnAttack;
	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;

	private void OnEnable()
	{
		pathmaker.enabled = true;
		pathmaker.OnNewDirection += Pathmaker_NewDirection;
		thoughts.Think("Just woke up, wandering.");
		SetState(State.Idle);
	}

	private void OnDisable()
	{
		pathmaker.OnNewDirection -= Pathmaker_NewDirection;
	}

	private void Pathmaker_NewDirection(Vector2 obj) => OnMoveInDirection?.Invoke(obj);

	private void SetState(State newState)
	{
		state = newState;
	}

	private void FixedUpdate()
	{
		if (GlobalManager.IsPaused || life.IsDead()) return;
		switch (state)
		{
			case State.Idle:
				UpdateIdle();
				break;
			case State.Wandering:
				UpdateWandering();
				break;
			case State.Aggro:
				UpdateAggro();
				break;
		}
	}

	private void UpdateAggro()
	{
	}

	private void UpdateWandering()
	{
	}

	private void UpdateIdle()
	{
	}

	private void StartAttack(Life target)
	{
		OnStopMoving?.Invoke();
		OnAttack?.Invoke(target);
	}

	//WANDER

	private void UpdateWander()
	{
		if (FoundATarget())
		{
			StartAggro();

			return;
		}

		if (IsWanderTime())
		{
			if (CanFindWanderPosition())
			{
				StartWandering();
				return;
			}

			StartIdle();
			return;
		}

		if (IsCloseEnoughToWanderPosition()) StartIdle();
	}

	private void StartAggro()
	{
		SetState(State.Aggro);
	}

	private void StartIdle()
	{
		idleCoolDown = idleCoolDownMax;
		SetState(State.Idle);
		OnStopMoving?.Invoke();
	}

	private bool IsCloseEnoughToWanderPosition()
	{
		var distanceToWanderPosition = Vector2.Distance(transform.position, currentWanderPosition);
		return distanceToWanderPosition < closeEnoughWanderDistance;
	}

	private bool IsWanderTime()
	{
		wanderCounter++;
		return wanderCounter >= wanderRate;
	}

	private void StartWandering()
	{
		SetState(State.Wandering);

		pathmaker.SetTargetPosition(currentWanderPosition);
		thoughts.Think("Wandering");
	}

	private bool FoundATarget()
	{
		var target = targets.GetClosestPlayerInAggroRange();
		return target != null;
	}

	private bool CanFindWanderPosition()
	{
		currentWanderPosition = targets.GetWanderPosition(wanderPoint);
		return currentWanderPosition != Vector2.zero;
	}
}