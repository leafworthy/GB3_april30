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
		if (Menu_Pause.isPaused) return;

		var newBullet = MAKER.Make(ASSETS.FX.BulletPrefab, attack.DamageOrigin);

		var bulletScript = newBullet.GetComponent<BulletFX>();
		bulletScript.Fire(attack.DamageOrigin, attack.DamagePosition, attack.HitPosition);

		var newBulletHitAnimation = MAKER.Make(ASSETS.FX.bulletHitAnimPrefab, attack.DamagePosition);
		MAKER.Unmake(newBulletHitAnimation, 5);

		SpawnMuzzleFlash(attack);

		var newBulletShell = MAKER.Make(ASSETS.FX.bulletShellPrefab, attack.DamageOrigin);
		newBulletShell.GetComponent<FallToFloor>().Fire((attack.DamageOrigin - attack.DamagePosition).normalized);
		SHAKER.ShakeCamera(attack.DamageOrigin, SHAKER.ShakeIntensityType.normal);
		ASSETS.sounds.ak47_shoot_sounds.PlayRandom();
	}

	private void SpawnMuzzleFlash(Attack attack)
	{
		if (Menu_Pause.isPaused) return;
		if (!hasMuzzleFlash) return;
		var newMuzzleFlash = MAKER.Make(ASSETS.FX.muzzleFlashPrefab, attack.DamageOrigin);
		MAKER.Unmake(newMuzzleFlash, .05f);
	}
}
