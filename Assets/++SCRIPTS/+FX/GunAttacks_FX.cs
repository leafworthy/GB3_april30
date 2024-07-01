using __SCRIPTS._ATTACKS;
using __SCRIPTS._CAMERA;
using __SCRIPTS._COMMON;
using __SCRIPTS._OBJECT;
using UnityEngine;

namespace __SCRIPTS._FX
{
	public class GunAttacks_FX : MonoBehaviour
{
	private GunAttacks gunAttacks;
	private void OnEnable()
	{
		gunAttacks = GetComponent<GunAttacks>();
		gunAttacks.OnShoot += GunAttacks_Shoot;
	}

	private void OnDisable()
	{ 
		gunAttacks.OnShoot -= GunAttacks_Shoot;
	
	}

	private void GunAttacks_Shoot(Attack attack, Vector2 attackStartPosition)
	{
	

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
		
	}

	private void CreateBulletHitAnimation(Attack attack)
	{
		var newBulletHitAnimation = Maker.Make(FX.Assets.bulletHitAnimPrefab, attack.DestinationFloorPoint);
		var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
		bulletHitHeight.SetDistanceToGround(attack.DestinationHeight);
		Maker.Unmake(newBulletHitAnimation, 5);
	}
}
}
