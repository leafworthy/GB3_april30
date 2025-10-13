using System;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class SimpleEnemyAI : MonoBehaviour, IMove, IAttack
	{
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public Vector2 GetMoveAimDir() => moveDir;
		private Vector2 moveDir;

		public bool IsMoving() => isMoving;
		private bool isMoving;
		public event Action<Life> OnAttack;
		private Life _life;
		private Life life => _life ??= GetComponent<Life>();

		private Life _target;
		private Targetter _targetter;

		private float minDistanceToPlayer => life.PrimaryAttackRange;

		private void Update()
		{
			_target = _targetter.GetClosestPlayerInAggroRange();
			if (_target == null) return;
			if (CloseEnoughToPlayer())
				AttackPlayer();
			else
				WalkToPlayer();
		}

		private void AttackPlayer()
		{
			isMoving = false;
			OnStopMoving?.Invoke();
			OnAttack?.Invoke(_target);
		}

		private void WalkToPlayer()
		{
			isMoving = true;
			moveDir = (_target.transform.position - transform.position).normalized * life.MoveSpeed;
			OnMoveInDirection?.Invoke(moveDir);
		}

		private void Start()
		{
			_targetter = GetComponent<Targetter>();
		}

		private bool CloseEnoughToPlayer()
		{
			var distanceToPlayer = Vector2.Distance(transform.position, _target.transform.position);
			return distanceToPlayer <= minDistanceToPlayer;
		}
	}
}
