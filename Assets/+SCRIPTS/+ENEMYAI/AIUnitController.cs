using System;
using __SCRIPTS._ENEMYAI.EnemyAI_States;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class AIUnitController : MonoBehaviour, IAI, IPoolable, IAttack, IMove
	{
		public bool stopMovingOnAttack = true;
		private IAIState currentState;

		private Animator _animator;

		public AstarPathfinder Pathmaker { get; set; }

		public Life Life { get; private set; }
		public float idleCoolDownMax { get; set; } = 2f;

		public Targetter Targets { get; private set; }


		public event Action<Life> OnAttack;
		public bool BornOnAggro { get; set; }

		private PauseManager pauseManager => _pauseManager ??= ServiceLocator.Get<PauseManager>();
		private PauseManager _pauseManager;
		private bool isMoving;

		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public Vector2 GetMoveAimDir() => default;

		public bool IsMoving() => isMoving;



		private void Start()
		{
			Pathmaker = GetComponent<AstarPathfinder>();
			Life = GetComponent<Life>();
			Targets = GetComponent<Targetter>();
			_animator = GetComponentInChildren<Animator>();
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
			isMoving = false;
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
				_animator.SetTrigger(UnitAnimations.AggroTrigger);
			}
		}

		public void MoveWithoutPathing(Vector2 randomDirection)
		{
			isMoving = true;
			OnMoveInDirection?.Invoke(randomDirection);
		}

		// Cached event handler for new direction events.
		private void HandleNewDirection(Vector2 direction)
		{
			isMoving = true;
			OnMoveInDirection?.Invoke(direction);
		}

		public void OnPoolSpawn()
		{
			// Reinitialize AI when spawned from pool
			isMoving = false;
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
