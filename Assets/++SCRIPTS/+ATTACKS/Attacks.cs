using __SCRIPTS.HUD_Displays;
using UnityEngine;

namespace __SCRIPTS
{
	public class Attacks : MonoBehaviour, INeedPlayer, IActivity
	{
		protected Life attacker;
		public virtual string VerbName => "Generic-Attack";

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
			var layer = attacker.IsPlayer ? ASSETS.LevelAssets.EnemyLayer : ASSETS.LevelAssets.PlayerLayer;
			var direction = (currentTargetLife.transform.position - position).normalized;
			var distance = Vector3.Distance(position, currentTargetLife.transform.position);
			return Physics2D.Raycast(position, direction, distance, layer);
		}

		public virtual void SetPlayer(Player _player)
		{
			attacker = GetComponent<Life>();
		}


	}
}

public interface IActivity
{
	public string VerbName { get; }
}
