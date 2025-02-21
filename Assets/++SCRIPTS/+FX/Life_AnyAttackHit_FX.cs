using RisingText;
using UnityEngine;

public class Life_AnyAttackHit_FX : MonoBehaviour
{
	private void Start()
	{
		Life.OnAnyAttackHit += Life_AttackHit;
	}

	private void Life_AttackHit(Attack attack, Life hitLife)
	{
		if (attack.DestinationLife.DebrisType == DebrisType.none) return;
		SprayDebree(attack,hitLife);
		MakeHitMark(attack,hitLife);
		DamageTint(attack, hitLife);
		CreateDamageRisingText(attack);
	}

	private void DamageTint(Attack attack, Life hitLife)
	{
		if(hitLife == null) return;
		var otherTintHandler = hitLife.gameObject.GetComponentInChildren<Life_FX>(true);
		if(otherTintHandler == null) return;
		otherTintHandler.StartTint(attack.color);
	}

	private void CreateDamageRisingText(Attack attack)
	{
		if (attack.DamageAmount <= 0) return;
		if(attack.DestinationLife.cantDie) return;
		var roundedDamage = (Mathf.Round(attack.DamageAmount * 100)) / 100.0f;
		RisingTextCreator.CreateRisingText("-" + roundedDamage, attack.DestinationWithHeight, Color.red);
	}

	private void SprayDebree(Attack attack, Life hitLife)
	{
		if (attack.IsPoison) return;
		MakeDebree(attack, hitLife);
		if (hitLife.DebrisType != DebrisType.blood) return;
		CreateBloodSpray(attack);
	}


	private void MakeHitMark(Attack attack, Life hitLife)
	{
		
		var hitList = FX.Assets.GetBulletHits(attack.DestinationLife.DebrisType);

		if (hitList == null) return;

		var heightCorrectionForDepth = new Vector2(0, -1f);
		var hitMarkObject = ObjectMaker.Make(hitList.GetRandom(), (Vector2) attack.DestinationFloorPoint + heightCorrectionForDepth);
		
		var hitHeightScript = hitMarkObject.GetComponent<ThingWithHeight>();
		hitHeightScript.SetDistanceToGround(attack.DestinationHeight- heightCorrectionForDepth.y, false);
		Debug.DrawLine( attack.DestinationFloorPoint, attack.DestinationFloorPoint + new Vector2(0, attack.DestinationHeight), Color.magenta, 5);
	
		if (!(attack.Direction.x > 0))
		{
			
			var localScale = hitMarkObject.transform.localScale;
		hitMarkObject.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, 0);
		}
		ObjectMaker.Unmake(hitMarkObject, 5);
		Debug.DrawLine(attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.black, 1f);
	
	}

	private void MakeDebree(Attack attack, Life hitLife)
	{
		if (attack.DestinationLife.DebrisType == DebrisType.none) return;
		var randAmount = Random.Range(2, 4);
		var lifeFX = attack.DestinationLife.GetComponent<Life_FX>();
		for (var j = 0; j < randAmount; j++)
		{
			//----->
			var forwardDebree = ObjectMaker.Make(FX.Assets.GetDebree(attack.DestinationLife.DebrisType), attack.DestinationFloorPoint);
		
			forwardDebree.GetComponent<FallToFloor>().Fire(attack);
			ObjectMaker.Unmake(forwardDebree, 3);

			//<-----
			var flippedAttack = new Attack(hitLife, attack.OriginLife, attack.DamageAmount);
			var backwardDebree = ObjectMaker.Make(FX.Assets.GetDebree(attack.DestinationLife.DebrisType), attack.DestinationFloorPoint);
			backwardDebree.GetComponent<FallToFloor>().Fire(flippedAttack);
			ObjectMaker.Unmake(backwardDebree, 3);
			if (lifeFX != null)
			{
				var sprite = forwardDebree.GetComponentInChildren<SpriteRenderer>();
				if (sprite != null) sprite.color = lifeFX.DebreeTint;
				sprite = backwardDebree.GetComponentInChildren<SpriteRenderer>();
				if (sprite != null) sprite.color = lifeFX.DebreeTint;
			}
		}
		
	}

	private void CreateBloodSpray(Attack attack)
	{
		var blood = ObjectMaker.Make(FX.Assets.bloodspray.GetRandom(), attack.DestinationFloorPoint);
		if (attack.Direction.x < 0)
		{
			blood.transform.localScale =
				new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
		}
	}
}