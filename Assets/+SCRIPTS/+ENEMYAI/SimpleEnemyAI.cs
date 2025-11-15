using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class SimpleEnemyAI : MonoBehaviour, ICanMoveThings, ICanAttack,INeedPlayer
	{
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public Vector2 GetMoveAimDir() => moveDir;
		private Vector2 moveDir;

		public bool IsMoving() => isMoving;
		private bool isMoving;
		public Player player => _player;
		private Player _player;
		public LayerMask EnemyLayer => Services.assetManager.LevelAssets.EnemyLayer;

		public bool IsEnemyOf(IGetAttacked targetLife)
		{
			if (targetLife == null) return false;
			if (targetLife.player == null) return true;
			return player.IsHuman() != targetLife.player.IsHuman();
		}

		public event Action OnAttackStart;
		public event Action<IGetAttacked> OnAttack;
		public event Action OnAttackStop;

		private IGetAttacked _target;
		private Targetter _targetter;

		public IHaveAttackStats stats => _stats ??= GetComponent<IHaveAttackStats>();
		private IHaveAttackStats _stats;

		private LayerMask enemyLayer;
		private float minDistanceToPlayer => stats.PrimaryAttackRange;

		private void Update()
		{
			_target = _targetter.GetClosestPlayerWithinRange(stats.AggroRange);
			if (_target == null) return;
			if (CloseEnoughToPlayer())
				AttackPlayer();
			else
				WalkToPlayer();
		}

		private void AttackPlayer()
		{
			Debug.Log("attacking player");
			isMoving = false;
			OnStopMoving?.Invoke();
			OnAttack?.Invoke(_target);
			OnAttackStart?.Invoke();
		}

		private void WalkToPlayer()
		{
			isMoving = true;
			moveDir = (_target.transform.position - transform.position).normalized * stats.MoveSpeed;
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

		public void SetPlayer(Player newPlayer)
		{
			_player	= newPlayer;
		}
	}
}
