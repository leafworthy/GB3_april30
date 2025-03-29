using UnityEngine;

namespace __SCRIPTS
{
	public class Attacks : MonoBehaviour
	{
		protected Life attacker;
		private void OnEnable()
		{
			attacker = GetComponent<Life>();
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
			var layer = attacker.IsPlayer ? ASSETS.LevelAssets.EnemyLayer : ASSETS.LevelAssets.PlayerLayer;
			var direction = (currentTargetLife.transform.position - position).normalized;
			var distance = Vector3.Distance(position, currentTargetLife.transform.position);
			return Physics2D.Raycast(position, direction, distance, layer);
		}
	}
}