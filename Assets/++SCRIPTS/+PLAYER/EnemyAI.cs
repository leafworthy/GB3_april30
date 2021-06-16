using System;using System.Configuration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _SCRIPTS
{
	public class EnemyAI : MonoBehaviour
	{
		private bool hasAggrodBefore;
		private DefenceHandler defenceHandler;
		private IAttackHandler attackHandler;
		private AstarAI astarAI;
		private GameObject currentTarget;
		private EnemyController controller;
		private bool isOn = true;
		private bool hasRealTarget;
		public bool wandersWhenIdle = true;
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
		private void Start()
		{
			stats = GetComponent<UnitStats>();
			astarAI = GetComponent<AstarAI>();
			astarAI.OnNewDirection += AI_OnNewDirection;

			controller = GetComponent<EnemyController>();

			defenceHandler = GetComponent<DefenceHandler>();
			defenceHandler.OnDead += Defence_OnDead;

			attackHandler = GetComponent<IAttackHandler>();

			SetState(State.Idle);
		}

		private void AI_OnNewDirection(Vector3 newDir)
		{
			controller.MoveInDirection(newDir);
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
				{
					astarAI.SetTargetPosition(currentTarget.transform);
				}
			}
		}

		private bool currentTargetOutOfRange()
		{
			return Vector3.Distance(transform.position, currentTarget.transform.position) > stats.aggroRange;
		}

		private void UpdateIdle()
		{
			var potentialTargets = Physics2D.OverlapCircleAll(transform.position, stats.aggroRange, ASSETS.layers.PlayerLayer);
			if (potentialTargets.Length > 0)
			{
				var closest = potentialTargets[0];
				foreach (var target in potentialTargets)
				{
					if (target.GetComponent<IPlayerController>() is null) continue;
					var distance = Vector3.Distance(target.transform.position, transform.position);
					var currentClosestDistance = Vector3.Distance(closest.transform.position, transform.position);
					if (distance < currentClosestDistance)
					{
						closest = target;
					}
				}

				hasRealTarget = true;
				currentTarget = closest.gameObject;
				SetState(State.Aggro);
				return;
			}
			else
			{
				var potentialTargets2 =
					Physics2D.OverlapCircleAll(transform.position, stats.activeRange, ASSETS.layers.PlayerLayer);
				if (potentialTargets2.Length > 0)
				{
					var closest = potentialTargets2[0];
					foreach (var target in potentialTargets)
					{
						if (target.GetComponent<IPlayerController>() is null) continue;
						var distance = Vector3.Distance(target.transform.position, transform.position);
						var currentClosestDistance = Vector3.Distance(closest.transform.position, transform.position);
						if (distance < currentClosestDistance)
						{
							closest = target;
						}
					}

					hasRealTarget = false;
					Wander();
				}

			}

		}

		private void Wander()
		{
			if (!wandersWhenIdle || hasAggrodBefore) return;
			if (currentWanderCooldown <= 0)
			{
				TargetNewWanderPosition();
			}
			else
			{
				if (Vector3.Distance(wanderTarget.transform.position, transform.position) < stats.attackRange)
				{
					TargetNewWanderPosition();
				}
				currentWanderCooldown -= Time.deltaTime;
			}



		}

		private void TargetNewWanderPosition()
		{
			currentWanderCooldown = wanderRate;
			if (wanderTarget == null)
			{
				wanderTarget = new GameObject();
			}

			wanderTarget.transform.position =
				(Vector3) Random.insideUnitCircle * stats.aggroRange + transform.position;
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
			return attackHandler.CanAttack(currentTarget.transform.position);
		}


		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, GetComponent<UnitStats>().aggroRange);
		}
	}
}
