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
		if (attack.DestinationLife.DebreeType == DebreeType.none) return;
		SprayDebree(attack,hitLife);
		MakeHitMark(attack,hitLife);
		CreateDamageRisingText(attack);
	}

	private void CreateDamageRisingText(Attack attack)
	{
		if (attack.DamageAmount <= 0) return;
		if(attack.DestinationLife.cantDie) return;
		RisingTextCreator.CreateRisingText("-" + attack.DamageAmount.ToString(), attack.DestinationWithHeight, Color.red);
	}

	private void SprayDebree(Attack attack, Life hitLife)
	{
		if (attack.IsPoison) return;
		MakeDebree(attack, hitLife);
		if (hitLife.DebreeType != DebreeType.blood) return;
		CreateBloodSpray(attack);
	}


	private void MakeHitMark(Attack attack, Life hitLife)
	{
		var hitList = FX.Assets.GetBulletHits(attack.DestinationLife.DebreeType);

		if (hitList == null) return;
		var hit = hitList.GetRandom();

		var heightCorrectionForDepth = new Vector2(0,-1f);
		var hitMarkObject = Maker.Make(hit, (Vector2) attack.DestinationFloorPoint+ heightCorrectionForDepth);
		
		var hitHeightScript = hit.GetComponent<ThingWithHeight>();
		var randomHeight = Random.Range(-2, 2);
		hitHeightScript.SetDistanceToGround(attack.DestinationHeight + randomHeight);

		if (!(attack.Direction.x < 0)) return;
		var localScale = hitMarkObject.transform.localScale;
		hitMarkObject.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, 0);
		Maker.Unmake(hitMarkObject, 5);
	}

	private void MakeDebree(Attack attack, Life hitLife)
	{
		if (attack.DestinationLife.DebreeType == DebreeType.none) return;
		var randAmount = Random.Range(2, 4);
		for (var j = 0; j < randAmount; j++)
		{
			//----->
			var forwardDebree = Maker.Make(FX.Assets.GetDebree(attack.DestinationLife.DebreeType), attack.DestinationFloorPoint);
			forwardDebree.GetComponent<FallToFloor>().Fire(attack);
			Maker.Unmake(forwardDebree, 3);

			//<-----
			var flippedAttack = new Attack(hitLife, attack.OriginLife, attack.DamageAmount);
			var backwardDebree = Maker.Make(FX.Assets.GetDebree(attack.DestinationLife.DebreeType), attack.DestinationFloorPoint);
			backwardDebree.GetComponent<FallToFloor>().Fire(flippedAttack);
			Maker.Unmake(backwardDebree, 3);
		}
	}

	private void CreateBloodSpray(Attack attack)
	{
		var blood = Maker.Make(FX.Assets.bloodspray.GetRandom(), attack.DestinationFloorPoint);
		if (attack.Direction.x < 0)
		{
			blood.transform.localScale =
				new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
		}
	}
}