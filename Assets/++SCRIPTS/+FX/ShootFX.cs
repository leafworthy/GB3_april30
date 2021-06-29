using UnityEngine;

public class ShootFX : MonoBehaviour
{
	private IShootHandler Shooter;

	[SerializeField] private bool hasMuzzleFlash;

	private void Awake()
	{
		Shooter = GetComponent<IShootHandler>();
		Shooter.OnShootStart += GunShootHandlerOnShoot;
	}

	private void GunShootHandlerOnShoot(Attack attack)
	{
		var newBullet = MAKER.Make(ASSETS.FX.bulletPrefab, attack.DamageOrigin);

		var bulletScript = newBullet.GetComponent<BulletFX>();
		Vector2 damagePosition = attack.DamagePosition;
		bulletScript.Fire(attack.DamageOrigin, damagePosition);

		var newBulletHitAnimation = MAKER.Make(ASSETS.FX.bulletHitAnimPrefab, damagePosition);
		MAKER.Unmake(newBulletHitAnimation, 5);

		SpawnMuzzleFlash(attack);

		var newBulletShell = MAKER.Make(ASSETS.FX.bulletShellPrefab, attack.DamageOrigin);
		newBulletShell.GetComponent<FallToFloor>().Fire((attack.DamageOrigin - damagePosition).normalized);
		SHAKER.ShakeCamera(attack.DamageOrigin, SHAKER.ShakeIntensityType.normal);
		AUDIO.PlaySound(ASSETS.sounds.ak47_shoot_sounds.GetRandom());
	}

	private void SpawnMuzzleFlash(Attack attack)
	{
		if (!hasMuzzleFlash) return;
		var newMuzzleFlash = MAKER.Make(ASSETS.FX.muzzleFlashPrefab, attack.DamageOrigin);
		MAKER.Unmake(newMuzzleFlash, .05f);
	}
}
