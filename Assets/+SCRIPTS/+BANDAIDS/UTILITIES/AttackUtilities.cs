using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class AttackUtilities
{
	private const float PushFactor = 2;
#if UNITY_EDITOR
	[MenuItem("Tools/Select Player Unit Animator")]
	public static void SelectPlayerUnitAnimator()
	{
		var component = Object.FindFirstObjectByType(typeof(PlayerUnitController)) as PlayerUnitController;
		var unitAnimations = component.GetComponentInChildren<UnitAnimations>();
		Selection.activeObject = unitAnimations.animator.gameObject;

		// Optionally, you can also ping it in the hierarchy to highlight it
		EditorGUIUtility.PingObject(unitAnimations.animator.gameObject);
	}
#endif
	public static List<Life> CircleCastForXClosestTargets(Life originLife, float range, int howMany = 2)
	{
		var result = new List<Life>();
		var circleCast = Physics2D.OverlapCircleAll(originLife.transform.position, range, originLife.EnemyLayer);
		var closest2 = circleCast.OrderBy(item => Vector2.Distance(item.gameObject.transform.position, originLife.transform.position)).Take(howMany);
		foreach (var col in closest2)
		{
			if (col == null) continue;
			var _life = col.gameObject.GetComponent<Life>();
			if (_life == null) continue;
			if (!IsValidTarget(originLife, _life)) continue;
			result.Add(_life);
		}

		return result;
	}

	public static void HitTargetsWithinRange(Life attackerLife, Vector2 attackPosition, float attackRange, float attackDamage, float extraPush = .1f)
	{
		var closestHits = FindClosestHits(attackerLife, attackPosition, attackRange, attackerLife.EnemyLayer);
		if (closestHits.Count <= 0) return;

		foreach (var targetLife in closestHits)
		{
			HitTarget(attackerLife, targetLife, attackDamage, extraPush);
		}
	}

	private static List<Life> FindClosestHits(Life originLife, Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{
		return Physics2D.OverlapCircleAll(attackPosition, attackRange, layerMask).Where(c => c?.gameObject != null)
		                .Select(c => c.gameObject.GetComponent<Life>()).Where(life => IsValidTarget(originLife, life)).ToList();
	}

	public static bool IsValidTarget(Life originLife, Life targetLife)
	{
		//took out && targetLife.IsPlayerAttackable
		if (targetLife == null) return false;
		  Debug.Log(originLife.IsEnemyOf(targetLife).ToString() + " enemy of" + targetLife.IsNotInvincible +  "is not invincible" + targetLife.CanTakeDamage() + "can take damage");
		return originLife.IsEnemyOf(targetLife) && targetLife.IsNotInvincible && !targetLife.IsObstacle && targetLife.CanTakeDamage();
	}

	public static GameObject FindClosestHit(Life originLife, Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{
		var closestHits = FindClosestHits(originLife, attackPosition, attackRange, layerMask);
		if (closestHits.Count <= 0) return null;

		var closest = closestHits[0];
		foreach (var col in closestHits)
		{
			var colStats = col.GetComponent<Life>();
			if (colStats.IsObstacle || !colStats.IsPlayerAttackable) continue;

			if (!colStats.IsNotInvincible) continue;

			if (Vector2.Distance(col.gameObject.transform.position, attackPosition) < Vector2.Distance(closest.transform.position, attackPosition))
				closest = col;
		}

		return closest.gameObject;
	}


	public static bool HitTarget(Life originLife, Life targetLife, float attackDamage, float extraPush = .1f)
	{
		if (targetLife == null) return false;
		if (!IsValidTarget(originLife, targetLife))
		{
			Debug.LogWarning("invalid target", targetLife);
			return false;
		}

		Debug.Log("hit target");

		var attack = AttackBuilder.Create().FromLife(originLife).ToLife(targetLife).WithDamage(attackDamage).WithExtraPush(extraPush).Build();
		targetLife.TakeDamage(attack);
		return true;
	}

	public static RaycastHit2D RaycastToObject(Life currentTargetLife, LayerMask layerMask)
	{
		var position = currentTargetLife.transform.position;
		var layer = layerMask;
		var direction = (currentTargetLife.transform.position - position).normalized;
		var distance = Vector3.Distance(position, currentTargetLife.transform.position);
		return Physics2D.Raycast(position, direction, distance, layer);
	}

	public static void Explode(Vector3 explosionPosition, float explosionRadius, float explosionDamage, Life _owner)
	{
		ExplosionFX(explosionPosition);

		var layer = _owner.EnemyLayer;
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

			var attack = AttackBuilder.Create().FromLife(_owner).ToLife(defence).WithDamage(explosionDamage * ratio).WithFlying(true).Build();
			defence.TakeDamage(attack);
		}
	}

	public static void ExplosionFX(Vector3 explosionPosition, float scale = 1)
	{
		var assets = Services.assetManager;
		var objectMaker = Services.objectMaker;
		var sfx = Services.sfx;

		var obj = objectMaker.Make(assets.FX.explosions.GetRandom(), explosionPosition);
		obj.transform.localScale = new Vector3(scale, scale, scale);
		obj = objectMaker.Make(assets.FX.fires.GetRandom(), explosionPosition);
		obj.transform.localScale = new Vector3(scale, scale, scale);
		CameraShaker.ShakeCamera(explosionPosition, CameraShaker.ShakeIntensityType.high);
		CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		Debug.Log("explosion sound");
		sfx.sounds.bean_nade_explosion_sounds.PlayRandomAt(explosionPosition);
	}

	public static Life CheckForCollisions(Vector2 target, GameObject gameObject, LayerMask layer)
	{
		var lineCast = Physics2D.LinecastAll(gameObject.transform.position, target, layer);
		foreach (var hit2D in lineCast)
		{
			if (hit2D.collider == null) continue;
			if (hit2D.collider.gameObject == gameObject) continue;
			var lifeHit = hit2D.collider.GetComponent<Life>();
			if (lifeHit == null) continue;
			Debug.Log("hit", lifeHit);
			return lifeHit;
		}

		Debug.DrawLine(gameObject.transform.position, target, Color.green);

		return null;
	}
}
