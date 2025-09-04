using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class Attacks : MonoBehaviour, INeedPlayer, IActivity
	{
		private Life _attacker;
		protected Life attacker => _attacker?? GetComponent<Life>();
		public virtual string VerbName => "Generic-Attack";


		protected void HitTarget(float attackDamage, Life targetLife, float extraPush = 0)
		{
			if (targetLife == null) return;
			if(!targetLife.CanTakeDamage) return;

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
			var layer = attacker.IsHuman ? Services.assetManager.LevelAssets.EnemyLayer : Services.assetManager.LevelAssets.PlayerLayer;
			var direction = (currentTargetLife.transform.position - position).normalized;
			var distance = Vector3.Distance(position, currentTargetLife.transform.position);
			return Physics2D.Raycast(position, direction, distance, layer);
		}

		public virtual void SetPlayer(Player _player)
		{
		}
	}
}
