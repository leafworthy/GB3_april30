using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableAttack : ServiceAbility
	{
		protected Life attacker => _attacker ?? GetComponent<Life>();
		private Life _attacker;
		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => false;

		protected override void DoAbility()
		{
		}


		protected void HitTarget(float attackDamage, Life targetLife, float extraPush = 0)
		{
			Debug.Log("hit!!!");
			if (targetLife == null) return;
			if (targetLife.IsInvincible) return;

			var newAttack = new Attack(attacker, targetLife, attackDamage);
			targetLife.TakeDamage(newAttack);
			if (targetLife.IsDead()) return;

			var enemyMoveAbility = targetLife.transform.gameObject.GetComponent<MoveAbility>();
			if (enemyMoveAbility == null) return;
			enemyMoveAbility.Push(newAttack.Direction, newAttack.DamageAmount * extraPush);
		}

		protected RaycastHit2D RaycastToObject(Life currentTargetLife)
		{
			var position = attacker.transform.position;
			var layer = attacker.IsPlayer ? assets.LevelAssets.EnemyLayer : assets.LevelAssets.PlayerLayer;
			var direction = (currentTargetLife.transform.position - position).normalized;
			var distance = Vector3.Distance(position, currentTargetLife.transform.position);
			return Physics2D.Raycast(position, direction, distance, layer);
		}

	}
}
