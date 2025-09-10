using UnityEngine;

namespace __SCRIPTS
{
	public class UnitAttackManager: MonoBehaviour
	{
		public Life life => _life ?? GetComponent<Life>();
		private Life _life;
		private ASSETS _assetManager;
		private ASSETS assetManager => _assetManager ??= ServiceLocator.Get<ASSETS>();

		public void HitTarget(float attackDamage, Life targetLife, float extraPush = 0)
		{
			Debug.Log("hit!!!");
			if (targetLife == null) return;
			if (targetLife.CanTakeDamage) return;

			var newAttack = new Attack(life, targetLife, attackDamage);
			targetLife.TakeDamage(newAttack);
			if (targetLife.IsDead()) return;

			var enemyMoveAbility = targetLife.transform.gameObject.GetComponent<MoveAbility>();
			if (enemyMoveAbility == null) return;
			enemyMoveAbility.Push(newAttack.Direction, newAttack.DamageAmount * extraPush);
		}

		protected RaycastHit2D RaycastToObject(Life currentTargetLife)
		{
			var position = life.transform.position;
			var layer = life.IsHuman ? assetManager.LevelAssets.EnemyLayer : assetManager.LevelAssets.PlayerLayer;
			var direction = (currentTargetLife.transform.position - position).normalized;
			var distance = Vector3.Distance(position, currentTargetLife.transform.position);
			return Physics2D.Raycast(position, direction, distance, layer);
		}
	}
}
