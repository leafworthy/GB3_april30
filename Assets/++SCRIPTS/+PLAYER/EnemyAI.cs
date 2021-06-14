using System;using System.Configuration;
using UnityEngine;

namespace _SCRIPTS
{
	public class EnemyAI : MonoBehaviour
	{

		private DefenceHandler defenceHandler;
		private IAttackHandler attackHandler;
		private AstarAI astarAI;
		private DefenceHandler currentTarget;
		private EnemyController controller;
		private bool isOn = true;

		private enum State
		{
			Idle,
			Aggro
		}

		[Range(0, 200)] [SerializeField] private float aggroDistance;

		[SerializeField] private State state;
		public event Action OnIdle;
		public event Action OnAggro;

		private void Start()
		{
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
			if (CanAttackTarget())
			{
				controller.StopMoving();
				controller.AttackTarget(currentTarget.GetPosition());
			}
			else
			{
				controller.StopAttacking();
				astarAI.SetTargetPosition(currentTarget.transform);
			}
		}

		private void UpdateIdle()
		{
			currentTarget = PLAYERS.GetClosestPlayer(transform.position).SpawnedPlayerGO.GetComponent<DefenceHandler>();

			if (currentTarget is null)
			{
				Debug.Log("closest player is null");
				return;
			}

			if (IsInAggroRange())
			{
				SetState(State.Aggro);
			}
		}

		private Vector3 GetDirectionToTarget()
		{
			return currentTarget.GetPosition() - transform.position;
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
			return attackHandler.CanAttack(currentTarget.GetPosition());
		}

		private bool IsInAggroRange()
		{
			return Vector3.Distance(transform.position, currentTarget.GetPosition()) <= aggroDistance;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, aggroDistance);
		}
	}
}
