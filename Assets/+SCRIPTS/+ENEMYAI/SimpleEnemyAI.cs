using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class SimpleEnemyAI : MonoBehaviour, ICanMoveThings, ICanAttack, INeedPlayer
	{
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public event Action<IGetAttacked> OnAttack;
		public Vector2 GetMoveAimDir() => moveDir;
		Vector2 moveDir;

		public bool IsMoving() => isMoving;
		bool isMoving;
		public Player player => _player;
		Player _player;
		public LayerMask EnemyLayer => Services.assetManager.LevelAssets.EnemyLayer;

		public bool IsEnemyOf(IGetAttacked targetLife)
		{
			if (targetLife == null) return false;
			if (targetLife.player == null) return true;
			return player?.IsHuman() != targetLife.player?.IsHuman();
		}

		IGetAttacked _target;
		Targetter targetter => _targetter ??= GetComponent<Targetter>();
		Targetter _targetter;

		public IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
		IHaveUnitStats _stats;

		void FixedUpdate()
		{
			_target = targetter.GetClosestPlayerWithinRange(stats.Stats.AggroRange);
			if (_target == null) return;
			if (CloseEnoughToPlayer())
				AttackPlayer();
			else
				WalkToPlayer();
		}

		void AttackPlayer()
		{
			isMoving = false;
			OnStopMoving?.Invoke();
			Debug.Log("attack player");
			OnAttack?.Invoke(_target);
		}

		void WalkToPlayer()
		{
			isMoving = true;
			moveDir = (_target.transform.position - transform.position).normalized * stats.Stats.MoveSpeed;
			OnMoveInDirection?.Invoke(moveDir);
		}

		bool CloseEnoughToPlayer()
		{
			var distanceToPlayer = Vector2.Distance(transform.position, _target.transform.position);
			return distanceToPlayer <= stats.Stats.Range(1);
		}

		public void SetPlayer(Player newPlayer)
		{
			_player = newPlayer;
		}
	}
}
