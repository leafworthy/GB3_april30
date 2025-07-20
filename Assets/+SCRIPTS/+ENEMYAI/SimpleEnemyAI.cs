using System;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class SimpleEnemyAI : MonoBehaviour, IMove, IAttack
	{
		private bool bornOnAggro;
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public event Action<Life> OnAttack;
		public bool BornOnAggro
		{
			get => bornOnAggro;
			set => bornOnAggro = value;
		}
		private Life _life;
		private Life life => _life ??= GetComponent<Life>();

		private Life _target;
		private Targetter _targetter;

		private float minDistanceToPlayer => life.unitData.attack1Range;

		private void Update()
		{
			_target = _targetter.GetClosestAttackablePlayer();
			if (_target == null) return;
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
			OnMoveInDirection?.Invoke((_target.transform.position - transform.position).normalized * life.unitData.moveSpeed);
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
