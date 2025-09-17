using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using UnityEngine;

public static class AttackUtilities
{
	private const float PushFactor = 10;

	public static List<Life> CircleCastForXClosestTargets(Life originLife,float range, int howMany = 2)
	{
		var result = new List<Life>();
		var circleCast = Physics2D.OverlapCircleAll(originLife.transform.position, range, Services.assetManager.LevelAssets.EnemyLayer);
		var closest2 = circleCast.OrderBy(item => Vector2.Distance(item.gameObject.transform.position, originLife.transform.position)).Take(howMany);
		foreach (var col in closest2)
		{
			if (col == null) continue;
			var _life = col.gameObject.GetComponent<Life>();
			if (_life == null) continue;
			if (!IsValidTarget(originLife, _life)) return null;
			result.Add( _life);

		}

		return result;
	}
	public static void HitTargetsWithinRange(Life attackerLife, Vector2 attackPosition, float attackRange, float attackDamage, float extraPush = .1f)
	{
		var closestHits = FindClosestHits(attackerLife, attackPosition, attackRange, attackerLife.EnemyLayer);
		if (closestHits.Count <= 0) return;

		foreach (var targetLife in closestHits)
		{
			HitTarget( attackerLife, targetLife, attackDamage);
		}
	}

	private static List<Life> FindClosestHits(Life originLife, Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{
		return Physics2D.OverlapCircleAll(attackPosition, attackRange, layerMask).Where(c => c?.gameObject != null)
		                .Select(c => c.gameObject.GetComponent<Life>()).Where(life => IsValidTarget(originLife, life)).ToList();
	}

	public static bool IsValidTarget(Life originLife, Life targetLife)
	{
		if (targetLife == null) return false;
		return originLife.IsEnemyOf(targetLife) && targetLife.IsNotInvincible && targetLife.IsPlayerAttackable && !targetLife.IsObstacle &&
		       targetLife.CanTakeDamage;
	}

	public static GameObject FindClosestHit(Life originLife, Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{
		var closestHits = FindClosestHits(originLife, attackPosition, attackRange, layerMask);
		if (closestHits.Count <= 0) return null;

		var closest = closestHits[0];
		foreach (var col in closestHits)
		{
			var colStats = col.GetComponent<Life>();
			if (colStats.IsObstacle || !colStats.IsPlayerAttackable)
			{
				Debug.Log("[ATTACK] skipping obstacle or unattackable");
				continue;
			}

			if (!colStats.IsNotInvincible)
			{
				Debug.Log("[ATTACK] skipping unattackable");
				continue;
			}

			if (Vector2.Distance(col.gameObject.transform.position, attackPosition) < Vector2.Distance(closest.transform.position, attackPosition))
				closest = col;
		}

		return closest.gameObject;
	}

	public static void HitTarget(Life originLife, Life targetLife, float attackDamage, float extraPush = .1f)
	{
		if (targetLife == null) return;
		if (!IsValidTarget(originLife, targetLife)) return;
		targetLife.TakeDamage(new Attack(originLife, targetLife, attackDamage));
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
