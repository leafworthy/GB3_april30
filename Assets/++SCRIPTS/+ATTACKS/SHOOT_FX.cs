using UnityEngine;

public class SHOOT_FX : Singleton<SHOOT_FX>
{
	private void Start()
	{
		GunAttacks.OnAnyGunShoot += GunAttacks_Shoot;
	}

	private void GunAttacks_Shoot(Attack attack, Vector2 attackStartPosition)
	{
		ASSETS.sounds.ak47_shoot_sounds.PlayRandom();
		ASSETS.sounds.bean_gun_miss_sounds.PlayRandom();

		CreateBullet(attack, attackStartPosition);

		CreateBulletHitAnimation(attack);

		MakeBulletShell(attack);
	}

	private static void CreateBullet(Attack attack, Vector2 attackStartPosition)
	{
		var newBullet = Maker.Make(ASSETS.FX.BulletPrefab, attack.OriginFloorPoint);
		var bulletScript = newBullet.GetComponent<Bullet>();
		bulletScript.Fire(attack, attackStartPosition);
	}

	private static void MakeBulletShell(Attack attack)
	{
		var newBulletShell = Maker.Make(ASSETS.FX.bulletShellPrefab, (Vector2) attack.OriginFloorPoint);
		newBulletShell.GetComponent<FallToFloor>()
		              .Fire(attack, true);
		CameraShaker.ShakeCamera((Vector2) attack.OriginFloorPoint, CameraShaker.ShakeIntensityType.normal);
		ASSETS.sounds.ak47_shoot_sounds.PlayRandom();
	}

	private void CreateBulletHitAnimation(Attack attack)
	{
		var newBulletHitAnimation = Maker.Make(ASSETS.FX.bulletHitAnimPrefab, attack.DestinationFloorPoint);
		var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
		bulletHitHeight.SetDistanceToGround(attack.DestinationHeight);
		Maker.Unmake(newBulletHitAnimation, 5);
	}
}