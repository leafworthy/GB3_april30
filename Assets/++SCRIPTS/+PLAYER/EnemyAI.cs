using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
	private bool hasAggrodBefore;
	private DefenceHandler defenceHandler;
	private IAttackHandler attackHandler;
	private AstarAI astarAI;
	private GameObject currentTarget;
	private DefenceHandler currentTargetDefence;
	private EnemyController controller;
	private bool isOn = true;
	public bool wandersWhenIdle = true;
	public bool invincibleUntilAggro;

	private enum State
	{
		Idle,
		Aggro
	}


	[SerializeField] private State state;
	private GameObject wanderTarget;
	private float currentWanderCooldown;
	private float wanderRate = 5;
	public event Action OnIdle;
	public event Action OnAggro;

	private UnitStats stats;

	private void DisableAllColliders()
	{
		var moreColliders = GetComponentsInChildren<Collider2D>();
		foreach (var col in moreColliders) col.enabled = false;
	}

	private void EnableAllColliders()
	{
		var moreColliders = GetComponentsInChildren<Collider2D>();
		foreach (var col in moreColliders) col.enabled = true;
	}

	private void Start()
	{
		stats = GetComponent<UnitStats>();
		astarAI = GetComponent<AstarAI>();
		astarAI.OnNewDirection += AI_OnNewDirection;
		astarAI.OnReachedEndOfPath += AI_OnReachedEndOfPath;

		controller = GetComponent<EnemyController>();

		defenceHandler = GetComponent<DefenceHandler>();
		defenceHandler.OnDead += Defence_OnDead;
		if (invincibleUntilAggro) DisableAllColliders();

		attackHandler = GetComponent<IAttackHandler>();

		SetState(State.Idle);
	}

	private void AI_OnNewDirection(Vector3 newDir)
	{
		controller.MoveInDirection(newDir);
	}

	private void AI_OnReachedEndOfPath()
	{
		SetState(State.Idle);
	}

	private void Defence_OnDead()
	{
		controller.Die();
		isOn = false;
	}

	private void Update()
	{
		if (!isOn || PAUSE.isPaused) return;

		switch (state)
		{
			case State.Idle:
				UpdateIdle();
				break;
			case State.Aggro:
				UpdateAggro();
				break;
		}
	}

	private void UpdateAggro()
	{
		hasAggrodBefore = true;
		if (invincibleUntilAggro) EnableAllColliders();
		if (CanAttackTarget())
		{
			controller.StopMoving();
			controller.AttackTarget(currentTarget.transform.position);
		}
		else
		{
			controller.StopAttacking();
			if (currentTargetOutOfRange())
			{
				controller.StopMoving();
				SetState(State.Idle);
			}
			else
				astarAI.SetTargetPosition(currentTarget.transform);
		}
	}

	private bool currentTargetOutOfRange()
	{
		return Vector3.Distance(transform.position, currentTarget.transform.position) >
		       stats.GetStatValue(StatType.aggroRange);
	}

	private void UpdateIdle()
	{
		var potentialTargets =
			Physics2D.OverlapCircleAll(transform.position, stats.GetStatValue(StatType.aggroRange), ASSETS.LevelAssets.PlayerLayer);

		if (potentialTargets.Length > 0)
		{
			var closest = potentialTargets[0];
			closest = GetClosestTarget(potentialTargets, closest);

			currentTarget = closest.gameObject;
			currentTargetDefence = currentTarget.GetComponent<DefenceHandler>();
			SetState(State.Aggro);
		}
		else
		{
			var potentialTargets2 =
				Physics2D.OverlapCircleAll(transform.position, stats.GetStatValue(StatType.activeRange), ASSETS.LevelAssets.PlayerLayer);
			if (potentialTargets2.Length <= 0) return;
			Wander();
		}
	}

	private Collider2D GetClosestTarget(Collider2D[] potentialTargets, Collider2D closest)
	{
		foreach (var target in potentialTargets)
		{
			if (target.GetComponent<IPlayerController>() is null) continue;
			if (target.GetComponent<DefenceHandler>().isInvincible) continue;
			var position = transform.position;
			var distance = Vector3.Distance(target.transform.position, position);
			var currentClosestDistance = Vector3.Distance(closest.transform.position, position);
			if (distance < currentClosestDistance) closest = target;
		}

		return closest;
	}

	private void Wander()
	{
		if (!wandersWhenIdle || !hasAggrodBefore) return;
		if (currentWanderCooldown <= 0)
			TargetNewWanderPosition();
		else
		{
			if (Vector3.Distance(wanderTarget.transform.position, transform.position) <
			    stats.GetStatValue(StatType.attackRange))
				TargetNewWanderPosition();
			currentWanderCooldown -= Time.deltaTime;
		}
	}

	private void TargetNewWanderPosition()
	{
		currentWanderCooldown = wanderRate;
		if (wanderTarget == null) wanderTarget = new GameObject();

		wanderTarget.transform.position =
			(Vector3) Random.insideUnitCircle * stats.GetStatValue(StatType.aggroRange) + transform.position;
		currentTarget = wanderTarget;
		astarAI.SetTargetPosition(currentTarget.transform);
	}

	private void SetState(State newState)
	{
		state = newState;
		switch (newState)
		{
			case State.Idle:
				OnIdle?.Invoke();
				break;
			case State.Aggro:
				OnAggro?.Invoke();
				break;
		}
	}

	private bool CanAttackTarget()
	{
		return !currentTargetDefence.isInvincible && attackHandler.CanAttack(currentTarget.transform.position);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, GetComponent<UnitStats>().GetStatValue(StatType.aggroRange));
	}
}
