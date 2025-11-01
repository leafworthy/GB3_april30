using System;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class SimpleEnemyAI : MonoBehaviour, ICanMoveThings, ICanAttack
	{
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public Vector2 GetMoveAimDir() => moveDir;
		private Vector2 moveDir;

		public bool IsMoving() => isMoving;
		private bool isMoving;
		public Player player => player1;
		public LayerMask EnemyLayer => Services.assetManager.LevelAssets.EnemyLayer;

		public bool IsEnemyOf(IGetAttacked targetLife) => player.IsHuman() != targetLife.player.IsHuman();

		public event Action<IGetAttacked> OnAttack;

		private IGetAttacked _target;
		private Targetter _targetter;

		public IHaveAttackStats Stats => _stats ??= GetComponent<IHaveAttackStats>();
		private IHaveAttackStats _stats;
		private Player player1;
		private LayerMask enemyLayer;
		private float minDistanceToPlayer => Stats.PrimaryAttackRange;

		private void Update()
		{
			_target = _targetter.GetClosestPlayerWithinRange(Stats.AggroRange);
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
			moveDir = (_target.transform.position - transform.position).normalized * Stats.MoveSpeed;
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
