using UnityEngine;

public class GunAttacks_FX : MonoBehaviour
{
	private GunAttacks gunAttacks;
	private void OnEnable()
	{
		gunAttacks = GetComponent<GunAttacks>();
		gunAttacks.OnShotHitTarget += GunAttacksOnOnOnShotHitTarget;
		gunAttacks.OnShotMissed += GunAttacksOnOnOnShotHitTarget;
	}


	private void OnDisable()
	{ 
		gunAttacks.OnShotHitTarget -= GunAttacksOnOnOnShotHitTarget;
		gunAttacks.OnShotMissed -= GunAttacksOnOnOnShotHitTarget;
	
	}

	private void GunAttacksOnOnOnShotHitTarget(Attack attack, Vector2 attackStartPosition)
	{

		Debug.Log("gun attacks on shot fired");
		CreateBullet(attack, attackStartPosition);

		CreateBulletHitAnimation(attack);

		MakeBulletShell(attack);
	}

	private static void CreateBullet(Attack attack, Vector2 attackStartPosition)
	{
		var newBullet = Maker.Make(FX.Assets.BulletPrefab, attack.OriginFloorPoint);
	
		var bulletScript = newBullet.GetComponent<Bullet_FX>();
		bulletScript.Fire(attack, attackStartPosition);
	}

	private static void MakeBulletShell(Attack attack)
	{
		var newBulletShell = Maker.Make(FX.Assets.bulletShellPrefab, (Vector2) attack.OriginFloorPoint);
		newBulletShell.GetComponent<FallToFloor>()
		              .Fire(attack, true);
		CameraShaker.ShakeCamera((Vector2) attack.OriginFloorPoint, CameraShaker.ShakeIntensityType.normal);
		Maker.Unmake(newBulletShell, 5);
	}

	private void CreateBulletHitAnimation(Attack attack)
	{
		Debug.Log("bullet hit animation");
		var heightCorrectionForDepth = new Vector2(0,-1.25f);
		
		var newBulletHitAnimation = Maker.Make(FX.Assets.bulletHitAnimPrefab, attack.DestinationFloorPoint + heightCorrectionForDepth);
		
		Debug.DrawLine( attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.magenta, 5);

		var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
		Debug.Log("bullet hit destination height " + attack.DestinationHeight);
		bulletHitHeight.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y);
		Maker.Unmake(newBulletHitAnimation, 5);
	}
}