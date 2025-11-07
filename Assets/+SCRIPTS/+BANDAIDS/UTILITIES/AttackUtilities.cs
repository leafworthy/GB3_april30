using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using __SCRIPTS;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class AttackUtilities
{
	private static readonly int Tint = Shader.PropertyToID("_Tint");
	private const float PushFactor = 2;
	private const float TintFadeSpeed = 6;
#if UNITY_EDITOR
	[MenuItem("Tools/Select player Unit Animator")]
	public static void SelectPlayerUnitAnimator()
	{
		var component = Object.FindFirstObjectByType(typeof(PlayerUnitController)) as PlayerUnitController;
		var unitAnimations = component.GetComponentInChildren<UnitAnimations>();
		Selection.activeObject = unitAnimations.animator.gameObject;

		// Optionally, you can also ping it in the hierarchy to highlight it
		EditorGUIUtility.PingObject(unitAnimations.animator.gameObject);
	}
#endif
	public static List<IGetAttacked> CircleCastForXClosestTargets(ICanAttack originLife, float range, int howMany = 2)
	{
		var result = new List<IGetAttacked>();
		var circleCast = Physics2D.OverlapCircleAll(originLife.transform.position, range, originLife.EnemyLayer);
		var closest2 = circleCast.OrderBy(item => Vector2.Distance(item.gameObject.transform.position, originLife.transform.position)).Take(howMany);
		foreach (var col in closest2)
		{
			if (col == null) continue;
			var _life = col.gameObject.GetComponent<IGetAttacked>();
			if (_life == null) continue;
			if (!IsValidTarget(originLife, _life)) continue;
			result.Add(_life);
		}

		return result;
	}

	public static void HitTargetsWithinRange(ICanAttack attackerLife, Vector2 attackPosition, float attackRange, float attackDamage, float extraPush = .1f)
	{
		var closestHits = FindClosestHits(attackerLife, attackPosition, attackRange, attackerLife.EnemyLayer);
		if (closestHits.Count <= 0) return;

		foreach (var targetLife in closestHits)
		{
			HitTarget(attackerLife, targetLife, attackDamage, extraPush);
		}
	}

	private static List<IGetAttacked> FindClosestHits(ICanAttack originLife, Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{
		return Physics2D.OverlapCircleAll(attackPosition, attackRange, layerMask).Where(c => c?.gameObject != null)
		                .Select(c => c.gameObject.GetComponent<IGetAttacked>()).Where(life => IsValidTarget(originLife, life)).ToList();
	}

	public static bool IsValidTarget(ICanAttack originLife, IGetAttacked targetLife)
	{
		if (targetLife == null) return false;
		return originLife.IsEnemyOf(targetLife) && targetLife.CanTakeDamage();
	}

	public static GameObject FindClosestHit(ICanAttack originLife, Vector2 attackPosition, float attackRange, LayerMask layerMask)
	{
		var closestHits = FindClosestHits(originLife, attackPosition, attackRange, layerMask);
		if (closestHits.Count <= 0) return null;

		var closest = closestHits[0];
		foreach (var col in closestHits)
		{
			if (!col.CanTakeDamage()) continue;

			if (Vector2.Distance(col.transform.position, attackPosition) < Vector2.Distance(closest.transform.position, attackPosition))
				closest = col;
		}

		return closest.transform.gameObject;
	}


	public static bool HitTarget(ICanAttack originLife, IGetAttacked targetLife, float attackDamage, float extraPush = .1f, bool causesFlying = false)
	{
		if (targetLife == null) return false;
		if (!IsValidTarget(originLife, targetLife))
		{
			return false;
		}

		Debug.Log("hit target");

		var attack = Attack.Create(originLife, targetLife).WithDamage(attackDamage).WithExtraPush(extraPush).WithFlying(causesFlying);
		targetLife.TakeDamage(attack);
		CameraShaker.ShakeCamera(targetLife.transform.position, CameraShaker.ShakeIntensityType.normal);
		return true;
	}

	public static RaycastHit2D RaycastToObject(IGetAttacked currentTargetLife, LayerMask layerMask)
	{
		var position = currentTargetLife.transform.position;
		var direction = (currentTargetLife.transform.position - position).normalized;
		var distance = Vector3.Distance(position, currentTargetLife.transform.position);
		return Physics2D.Raycast(position, direction, distance, layerMask);
	}

	public static void Explode(Vector3 explosionPosition, float explosionRadius, float explosionDamage, ICanAttack _owner)
	{
		ExplosionFX(explosionPosition);

		var layer = _owner.EnemyLayer;
		var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, layer);

		if (hits == null) return;
		foreach (var hit in hits)
		{
			var defence = hit.GetComponent<IGetAttacked>();
			if (defence is null) continue;
			var ratio = explosionRadius / Vector3.Distance(hit.transform.position, explosionPosition);

			var otherMove = defence.transform.GetComponent<MoveAbility>();
			if (otherMove != null)
				otherMove.Push(explosionPosition - defence.transform.position, PushFactor * ratio);

			var attack = Attack.Create(_owner, defence).WithDamage(explosionDamage * ratio).WithFlying();
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
		sfx.sounds.bean_nade_explosion_sounds.PlayRandomAt(explosionPosition);
	}

	public static IGetAttacked CheckForCollisions(Vector2 target, GameObject gameObject, LayerMask layer)
	{
		var lineCast = Physics2D.LinecastAll(gameObject.transform.position, target, layer);
		foreach (var hit2D in lineCast)
		{
			if (hit2D.collider == null) continue;
			if (hit2D.collider.gameObject == gameObject) continue;
			var lifeHit = hit2D.collider.GetComponent<IGetAttacked>();
			if (lifeHit == null) continue;
			return lifeHit;
		}

		Debug.DrawLine(gameObject.transform.position, target, Color.green);

		return null;
	}

	public static void StartTint(Color tintColor, List<SpriteRenderer> renderersToTint)
	{
		var materialTintColor = new Color();
		materialTintColor = tintColor;
		foreach (var r in renderersToTint)
		{
			r.material.SetColor(Tint, materialTintColor);
		}
	}

	public static void TintDebreeColor(GameObject debreeObject, Color DebreeTint)
	{

		var spriteToTint = debreeObject.GetComponentInChildren<SpriteRenderer>();
		if (spriteToTint != null)
			spriteToTint.color = DebreeTint;
	}



	public static void AttackHitFX(Attack attack, List<SpriteRenderer> renderersToTint, DebrisType debrisType, Color debrisColor)
	{
		StartTint(attack.TintColor, renderersToTint);
		CreateDamageRisingText(attack);
		SprayDebree(attack,  debrisType, debrisColor);
		MakeHitMark(attack,  debrisType);
	}

	private static void MakeHitMark(Attack attack, DebrisType debrisType)
	{
		var hitList = Services.assetManager.FX.GetBulletHits(debrisType);
		if (hitList == null) return;

		var heightCorrectionForDepth = new Vector2(0, -1f);
		var hitMarkObject = Services.objectMaker.Make(hitList.GetRandom(), attack.DestinationFloorPoint + heightCorrectionForDepth);

		var hitHeightScript = hitMarkObject.GetComponent<HeightAbility>();
		hitHeightScript.SetHeight(attack.DestinationHeight - heightCorrectionForDepth.y);

		if (!(attack.Direction.x > 0))
		{
			var localScale = hitMarkObject.transform.localScale;
			hitMarkObject.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, 0);
		}

		Services.objectMaker.Unmake(hitMarkObject, 5);
	}

	private static void CreateDamageRisingText(Attack attack)
	{
		//if (attack.DamageAmount <= 0|| !health.CanTakeDamage()) return;
		Services.risingText.CreateRisingText("-" + Mathf.Round(attack.DamageAmount), attack.DestinationWithHeight, Color.red);
	}

	private static void SprayDebree(Attack attack, DebrisType debrisType, Color debrisColor)
	{
		if (attack.MakesDebree) return;
		MakeDebree(attack  , attack.DestinationFloorPoint, debrisType, Color.red);
		if (debrisType != DebrisType.blood) return;
		CreateBloodSpray(attack);
	}

	private static void CreateBloodSpray(Attack attack)
	{
		var blood = Services.objectMaker.Make(Services.assetManager.FX.bloodspray.GetRandom(), attack.DestinationFloorPoint);
		var bloodHeightScript = blood.GetComponent<HeightAbility>();
		bloodHeightScript.SetHeight(attack.DestinationHeight);
		if (attack.Direction.x < 0) blood.transform.localScale = new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
	}

	private static void MakeDebree(Attack attack, Vector2 position, DebrisType debrisType, Color debrisColor)
	{
		if (debrisType == DebrisType.none) return;
		var randAmount = Random.Range(2, 10);
		for (var j = 0; j < randAmount; j++)
		{
			var randomAngle = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)).normalized;
			//----->
			FireDebree(attack.Direction + randomAngle, attack.OriginHeight, 1, position,debrisType,debrisColor);

			//<-----
			FireDebree(attack.FlippedDirection + randomAngle, attack.OriginHeight, 1, position, debrisType, debrisColor);
		}
	}

	private static void FireDebree(Vector2 angle, float height, float verticalSpeed, Vector2 position, DebrisType debrisType, Color debrisColor)
	{
		var forwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(debrisType), position);
		forwardDebree.GetComponent<MoveJumpAndRotateAbility>().Fire(angle, height, verticalSpeed);
		TintDebreeColor(forwardDebree, debrisColor);
		Services.objectMaker.Unmake(forwardDebree, 3);
	}

	public static void ExplodeDebreeEverywhere(float explosionSize,  Vector2 position, DebrisType debrisType, Color debrisColor, int min = 5, int max = 10)
	{
		var randAmount = Random.Range(min, max);
		for (var j = 0; j < randAmount; j++)
		{
			var randomAngle = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			FireDebree(randomAngle, 0, explosionSize,  position, debrisType, debrisColor);
		}
	}

	public static void FadeOutTintAlpha(ref Color materialTintColor, List<SpriteRenderer> renderersToTint)
	{
		if (materialTintColor.a < 0) return;
		materialTintColor.a = Mathf.Clamp01(materialTintColor.a - TintFadeSpeed * Time.deltaTime);
		foreach (var r in renderersToTint)
		{
			r.material.SetColor(Tint, materialTintColor);
		}
	}
}
