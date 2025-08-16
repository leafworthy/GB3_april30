using System;
using __SCRIPTS._ENEMYAI.EnemyAI_States;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public interface IAI
	{
		void GetBornIfBornOnAggro();
		Targetter Targets { get; }
		AstarPathfinder Pathmaker { get; }
		Transform transform { get; }
		Life Life { get; }
		float idleCoolDownMax { get; }
		void TransitionToState(IAIState state);
		void Attack(Life target);
		void StopMoving();
		void MoveWithoutPathing(Vector2 randomDirection);
	}

	public interface IAIState
	{
		void OnEnterState(IAI ai);
		void OnExitState();
		void UpdateState();
	}

		public class EnemyAI : ServiceUser, IAI, IPoolable, IMove, IAttack
	{
		public bool stopMovingOnAttack = true;
		private IAIState currentState;

		private Animator _animator;

		public AstarPathfinder Pathmaker { get; set; }

		public Life Life { get; set; }
		public float idleCoolDownMax { get; set; } = 2f;

		public Targetter Targets { get; set; }


		public event Action<Life> OnAttack;
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public bool BornOnAggro { get; set; }

		private void Awake()
		{
			Pathmaker = GetComponent<AstarPathfinder>();
			Life = GetComponent<Life>();
			Targets = GetComponent<Targetter>();

			_animator = GetComponentInChildren<Animator>();
		}

		private void Start()
		{
			InitializeAI();
		}

		private void InitializeAI()
		{
			if (Pathmaker != null) Pathmaker.OnNewDirection += HandleNewDirection;
			TransitionToState(new AggroState());
		}

		private void OnDestroy()
		{
			if (Pathmaker != null) Pathmaker.OnNewDirection -= HandleNewDirection;
		}

		private void FixedUpdate()
		{
			if (pauseManager.IsPaused || Life.IsDead())
				return;

			currentState?.UpdateState();
		}

		public void TransitionToState(IAIState newState)
		{
			currentState?.OnExitState();
			currentState = newState;
			currentState.OnEnterState(this);
		}

		public void StopMoving()
		{
			if (!stopMovingOnAttack) return;
			OnStopMoving?.Invoke();
			Pathmaker.StopPathing();
		}

		public void Attack(Life targetsCurrentTarget)
		{
			Debug.DrawLine(transform.position, targetsCurrentTarget.transform.position, Color.red, 1f);
			StopMoving();
			OnAttack?.Invoke(targetsCurrentTarget);
		}

		public void GetBornIfBornOnAggro()
		{
			if (BornOnAggro)
			{
				// Use a constant (or cached) trigger name to avoid allocating strings.
				_animator.SetTrigger(UnitAnimations.AggroTrigger);
			}
		}

		public void MoveWithoutPathing(Vector2 randomDirection)
		{
			OnMoveInDirection?.Invoke(randomDirection);
		}

		// Cached event handler for new direction events.
		private void HandleNewDirection(Vector2 direction)
		{
			OnMoveInDirection?.Invoke(direction);
		}

		public void OnPoolSpawn()
		{
			// Reinitialize AI when spawned from pool
			InitializeAI();
		}

		public void OnPoolDespawn()
		{
			// Clean up when returning to pool
			if (Pathmaker != null) Pathmaker.OnNewDirection -= HandleNewDirection;
			currentState?.OnExitState();
			currentState = null;
		}
	}
}
