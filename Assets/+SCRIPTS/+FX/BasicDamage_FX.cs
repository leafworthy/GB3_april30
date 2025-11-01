using System;
using __SCRIPTS;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BasicDamage_FX : MonoBehaviour,IExplodeOnDeath
{
	//Handles visual effects when taking damage
	private BasicTint tintEffect => _tintEffect ??= GetComponent<BasicTint>();
	private BasicTint _tintEffect;
	private BasicHealth health => _damageTaker ??= GetComponent<BasicHealth>();
	private BasicHealth _damageTaker;
	private BasicStats stats => _stats ??= GetComponent<BasicStats>();
	private BasicStats _stats;

	public void Start()
	{
		if (health == null)
		{
			Debug.Log("returned here");
			return;
		}
		health.OnAttackHit += Life_AttackHit;
		health.OnShielded += Life_Shielded;
	}

	private void Life_AttackHit(Attack attack)
	{
		if (!health.IsDead()) return;
		tintEffect.StartTint(attack.TintColor);
		CreateDamageRisingText(attack);
		SprayDebree(attack);
		MakeHitMark(attack);
	}

	private void MakeHitMark(Attack attack)
	{
		var hitList = Services.assetManager.FX.GetBulletHits(stats.DebrisType);
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

	private void CreateDamageRisingText(Attack attack)
	{
		if (attack.DamageAmount <= 0|| !health.CanTakeDamage()) return;
		Services.risingText.CreateRisingText("-" + Mathf.Round(attack.DamageAmount), attack.DestinationWithHeight, Color.red);
	}

	private void SprayDebree(Attack attack)
	{
		if (attack.MakesDebree) return;
		MakeDebree(attack);
		if (stats.DebrisType != DebrisType.blood) return;
		CreateBloodSpray(attack);
	}

	private void CreateBloodSpray(Attack attack)
	{
		var blood = Services.objectMaker.Make(Services.assetManager.FX.bloodspray.GetRandom(), attack.DestinationFloorPoint);
		var bloodHeightScript = blood.GetComponent<HeightAbility>();
		bloodHeightScript.SetHeight(attack.DestinationHeight);
		if (attack.Direction.x < 0) blood.transform.localScale = new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
	}

	private void MakeDebree(Attack attack)
	{
		if (stats.DebrisType == DebrisType.none) return;
		var randAmount = Random.Range(2, 10);
		for (var j = 0; j < randAmount; j++)
		{
			var randomAngle = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)).normalized;
			//----->
			FireDebree(attack.Direction + randomAngle, attack.OriginHeight, 1);

			//<-----
			FireDebree(attack.FlippedDirection + randomAngle, attack.OriginHeight, 1);
		}
	}

	private void FireDebree(Vector2 angle, float height, float verticalSpeed)
	{
		var forwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(stats.DebrisType), transform.position);
		forwardDebree.GetComponent<MoveJumpAndRotateAbility>().Fire(angle, height, verticalSpeed);
		tintEffect.TintDebreeColor(forwardDebree);
		Services.objectMaker.Unmake(forwardDebree, 3);
	}

	public void ExplodeDebreeEverywhere(float explosionSize, int min = 5, int max = 10)
	{
		if (stats.DebrisType == DebrisType.none) return;
		var randAmount = Random.Range(min, max);
		for (var j = 0; j < randAmount; j++)
		{
			var randomAngle = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			FireDebree(randomAngle, 0, explosionSize);
		}
	}
	private void Life_Shielded(Attack obj)
	{
		tintEffect.StartTint(Color.yellow);
	}

}
