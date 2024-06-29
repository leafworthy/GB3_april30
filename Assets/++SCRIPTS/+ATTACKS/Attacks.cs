using UnityEngine;

public class Attacks : MonoBehaviour
{
	protected Life attacker;

	private void OnEnable()
	{
		attacker = GetComponent<Life>();
	}

	protected void HitTarget(float attackDamage, Collider2D hit2D, bool connect)
	{
		var enemy = hit2D.transform.gameObject.GetComponent<Life>();
		if (enemy == null) return;
		if (enemy.IsPlayer || enemy.cantDie || enemy.IsObstacle) return;
		if (connect)
		{
			ASSETS.sounds.brock_bathit_sounds.PlayRandom();
			Maker.Make(ASSETS.FX.hits.GetRandom(), hit2D.transform.position);
		}



		var newAttack = new Attack(attacker, enemy, attackDamage);
		enemy.TakeDamage(newAttack);
		if (!enemy.IsDead()) return;
		
		var enemyMoveAbility = hit2D.transform.gameObject.GetComponent<MoveAbility>();
		if(enemyMoveAbility == null) return;
		enemyMoveAbility.Push(newAttack.Direction, newAttack.DamageAmount * .2f);
	}

	protected RaycastHit2D GetAttackHitObject(Vector3 targetPosition)
	{
		var position = attacker.transform.position;
		var layer = attacker.IsPlayer ? ASSETS.LevelAssets.EnemyLayer : ASSETS.LevelAssets.PlayerLayer;
		var direction = (targetPosition - position).normalized;
		var distance = Vector3.Distance(position, targetPosition);
		return Physics2D.Raycast(position, direction, distance, layer);
	}
}