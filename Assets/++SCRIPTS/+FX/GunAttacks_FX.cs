using UnityEngine;

public class GunAttacks_FX : MonoBehaviour
{
	private GunAttack gunAttack;
	private void OnEnable()
	{
		gunAttack = GetComponent<GunAttack>();
		gunAttack.OnShotHitTarget += GunAttackOnOnOnShotHitTarget;
		gunAttack.OnShotMissed += GunAttackOnOnOnShotHitTarget;
	}


	private void OnDisable()
	{ 
		gunAttack.OnShotHitTarget -= GunAttackOnOnOnShotHitTarget;
		gunAttack.OnShotMissed -= GunAttackOnOnOnShotHitTarget;
	
	}

	private void GunAttackOnOnOnShotHitTarget(Attack attack, Vector2 attackStartPosition)
	{

		CreateBullet(attack, attackStartPosition);

		CreateBulletHitAnimation(attack);

		MakeBulletShell(attack);
		CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Short);
	}

	private static void CreateBullet(Attack attack, Vector2 attackStartPosition)
	{
		var newBullet = ObjectMaker.Make(ASSETS.FX.BulletPrefab, attack.OriginFloorPoint);
	
		var bulletScript = newBullet.GetComponent<Bullet_FX>();
		bulletScript.Fire(attack, attackStartPosition);
		CameraShaker_FX.ShakeCamera((Vector2)attack.OriginFloorPoint, CameraShaker_FX.ShakeIntensityType.normal);
	}

	private static void MakeBulletShell(Attack attack)
	{
		var newBulletShell = ObjectMaker.Make(ASSETS.FX.bulletShellPrefab, (Vector2) attack.OriginFloorPoint);
		newBulletShell.GetComponent<FallToFloor>()
		              .Fire(attack, true);
		
		ObjectMaker.Unmake(newBulletShell, 5);
	}

	private void CreateBulletHitAnimation(Attack attack)
	{
		var heightCorrectionForDepth = new Vector2(0,-1.25f);
		
		var newBulletHitAnimation = ObjectMaker.Make(ASSETS.FX.bulletHitAnimPrefab, attack.DestinationFloorPoint + heightCorrectionForDepth);
		
		Debug.DrawLine( attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.magenta, 5);

		var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
		bulletHitHeight.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y);
		ObjectMaker.Unmake(newBulletHitAnimation, 5);
	}


}