using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using UnityEngine;

public static class AttackUtilities
{
	public static float PushFactor = 10;

	public static void HitTargetsWithinRange(Life attackerLife, Vector2 attackPosition, float attackRange, float attackDamage, float extraPush = .1f)
	{
		var layer = attackerLife.EnemyLayer;

		var closestHits = FindClosestHits(attackPosition, attackRange, layer);
		if (closestHits.Count <= 0) return;

		foreach (var targetLife in closestHits)
		{
			HitTarget(attackDamage, attackerLife, targetLife, extraPush);
		}
	}
	private static List<Life> FindClosestHits(Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{

		var circleCast = Physics2D.OverlapCircleAll(attackPosition, attackRange, layerMask)
		                          .ToList();
		if (circleCast.Count <= 0) return new List<Life>();

		var enemies = new List<Life>();
		foreach (var targetCollider in circleCast)
		{
			if (targetCollider == null || targetCollider.gameObject == null) continue;

			var targetLife = targetCollider.gameObject.GetComponentInParent<Life>();
			if (targetLife == null) continue;
			if (targetLife.IsHuman || targetLife.CanBeAttacked || targetLife.IsObstacle) continue;
			enemies.Add(targetLife);
		}

		return enemies;
	}
	public static GameObject FindClosestHit(Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{
		var closestHits = FindClosestHits(attackPosition, attackRange, layerMask);
		if (closestHits.Count <= 0) return null;

		var closest = closestHits[0];
		foreach (var col in closestHits)
		{
			var colStats = col.GetComponentInChildren<Life>();
			if (colStats.IsObstacle || !colStats.IsPlayerAttackable) continue;
			if (!colStats.CanBeAttacked) continue;
			if (Vector2.Distance(col.gameObject.transform.position, attackPosition) < Vector2.Distance(closest.transform.position, attackPosition))
				closest = col;
		}

		return closest.gameObject;
	}
	public static void HitTarget(float attackDamage, Life originLife, Life targetLife, float extraPush = 0)
	{
		if (targetLife == null) return;
		if (!targetLife.IsEnemyOf(originLife)) return;
		if (!targetLife.CanTakeDamage) return;

		var newAttack = new Attack(originLife, targetLife, attackDamage);
		targetLife.TakeDamage(newAttack);
		if (targetLife.IsDead()) return;

		var targetMoveAbility = targetLife.transform.gameObject.GetComponent<MoveAbility>();
		if (targetMoveAbility == null) return;
		targetMoveAbility.Push(newAttack.Direction, newAttack.DamageAmount * extraPush);
	}

	public static RaycastHit2D RaycastToObject(Life currentTargetLife, LayerMask layerMask)
	{
		var position = currentTargetLife.transform.position;
		var layer = layerMask;
		var direction = (currentTargetLife.transform.position - position).normalized;
		var distance = Vector3.Distance(position, currentTargetLife.transform.position);
		return Physics2D.Raycast(position, direction, distance, layer);
	}



	public static void Explode(Vector3 explosionPosition, float explosionRadius, float explosionDamage, Player _owner)
	{
		var assets = ServiceLocator.Get<AssetManager>();
		var objectMaker = ServiceLocator.Get<ObjectMaker>();
		var sfx = ServiceLocator.Get<SFX>();

		objectMaker.Make(assets.FX.explosions.GetRandom(), explosionPosition);
		objectMaker.Make(assets.FX.fires.GetRandom(), explosionPosition);

		var layer = _owner.spawnedPlayerDefence.EnemyLayer;

		CameraShaker.ShakeCamera(explosionPosition, CameraShaker.ShakeIntensityType.high);
		CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		sfx.sounds.bean_nade_explosion_sounds.PlayRandomAt(explosionPosition);
		var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, layer);

		if (hits == null) return;
		foreach (var hit in hits)
		{
			var defence = hit.GetComponent<Life>();
			if (defence is null) continue;
			var ratio = explosionRadius / Vector3.Distance(hit.transform.position, explosionPosition);

			var otherMove = defence.GetComponent<MoveAbility>();
			if (otherMove != null)
				otherMove.Push(explosionPosition - defence.transform.position, PushFactor * ratio);
			var newAttack = new Attack(_owner.spawnedPlayerDefence, explosionPosition, defence.transform.position, defence, explosionDamage * ratio);
			defence.TakeDamage(newAttack);
		}
	}


}
