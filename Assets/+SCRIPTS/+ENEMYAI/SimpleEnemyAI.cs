using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class SimpleEnemyAI : MonoBehaviour, ICanMoveThings, ICanAttack,INeedPlayer
	{
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public event Action OnAttackStart;
		public event Action<IGetAttacked> OnAttack;
		public event Action OnAttackStop;
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
			return player?.IsHuman() != targetLife.player?.IsHuman();
		}


		private IGetAttacked _target;
		private Targetter targetter =>  _targetter ??= GetComponent<Targetter>();
		private Targetter _targetter;

		public IHaveAttackStats stats => _stats ??= GetComponent<IHaveAttackStats>();
		private IHaveAttackStats _stats;

		private void FixedUpdate()
		{
			_target = targetter.GetClosestPlayerWithinRange(stats.AggroRange);
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
			OnAttackStart?.Invoke();
		}

		private void WalkToPlayer()
		{
			isMoving = true;
			moveDir = (_target.transform.position - transform.position).normalized * stats.MoveSpeed;
			OnMoveInDirection?.Invoke(moveDir);
		}



		private bool CloseEnoughToPlayer()
		{
			var distanceToPlayer = Vector2.Distance(transform.position, _target.transform.position);
			return distanceToPlayer <= stats.PrimaryAttackRange;
		}

		public void SetPlayer(Player newPlayer)
		{
			_player	= newPlayer;
		}

	}
}
