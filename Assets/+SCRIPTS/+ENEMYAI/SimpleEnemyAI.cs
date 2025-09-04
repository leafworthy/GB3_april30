using System;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class SimpleEnemyAI : MonoBehaviour, IMove, IAttack
	{
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public event Action<Life> OnAttack;
		public bool BornOnAggro { get; set; } = false;
		private Life _life;
		private Life life => _life ??= GetComponent<Life>();

		private Life _target;
		private Targetter _targetter;
		private float minDistanceToPlayer => life.PrimaryAttackRange;

		private void Update()
		{
			_target = _targetter.GetClosestPlayer();
			if (_target == null)
			{
				Debug.Log("no target");
				return;
			}
			if (CloseEnoughToPlayer())
				AttackPlayer();
			else
				WalkToPlayer();
		}

		private void AttackPlayer()
		{
			OnAttack?.Invoke(_target);
		}

		private void WalkToPlayer()
		{
			OnMoveInDirection?.Invoke((_target.transform.position - transform.position).normalized * life.MoveSpeed);
		}

		private void Start()
		{
			Services.enemyManager.ConfigureNewEnemy(gameObject);
			_targetter = GetComponent<Targetter>();
		}

		private bool CloseEnoughToPlayer()
		{
			var distanceToPlayer = Vector2.Distance(transform.position, _target.transform.position);
			return distanceToPlayer <= minDistanceToPlayer;
		}
	}
}
