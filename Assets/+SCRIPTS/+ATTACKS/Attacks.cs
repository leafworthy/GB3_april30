using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class Attacks : ServiceUser, INeedPlayer, IActivity
	{
		private Life _attacker;
		protected Life attacker => _attacker?? GetComponent<Life>();
		public virtual string VerbName => "Generic-Attack";

		public virtual bool TryCompleteGracefully(CompletionReason reason, IActivity newActivity = null)
		{
			return false; // Default: no special completion handling
		}

		protected void HitTarget(float attackDamage, Life targetLife, float extraPush = 0)
		{
			if (targetLife == null) return;
			if(targetLife.isInvincible) return;

			var newAttack = new Attack(attacker, targetLife, attackDamage);
			targetLife.TakeDamage(newAttack);
			if (targetLife.IsDead()) return;

			var enemyMoveAbility = targetLife.transform.gameObject.GetComponent<MoveAbility>();
			if(enemyMoveAbility == null) return;
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

		public virtual void SetPlayer(Player _player)
		{
		}
	}
}
