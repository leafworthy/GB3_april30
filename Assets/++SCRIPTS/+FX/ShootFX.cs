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
		if (PAUSE.isPaused) return;
		var prefab = Shooter.IsGlocking ? ASSETS.FX.AKbulletPrefab : ASSETS.FX.GlockBulletPrefab;
		var newBullet = MAKER.Make(prefab, attack.DamageOrigin);

		var bulletScript = newBullet.GetComponent<BulletFX>();
		Vector2 damagePosition = attack.DamagePosition;
		bulletScript.Fire(attack.DamageOrigin, damagePosition, Shooter.IsGlocking);

		var newBulletHitAnimation = MAKER.Make(ASSETS.FX.bulletHitAnimPrefab, damagePosition);
		MAKER.Unmake(newBulletHitAnimation, 5);

		SpawnMuzzleFlash(attack);

		var newBulletShell = MAKER.Make(ASSETS.FX.bulletShellPrefab, attack.DamageOrigin);
		newBulletShell.GetComponent<FallToFloor>().Fire((attack.DamageOrigin - damagePosition).normalized);
		SHAKER.ShakeCamera(attack.DamageOrigin, SHAKER.ShakeIntensityType.normal);
		ASSETS.sounds.ak47_shoot_sounds.PlayRandom();
	}

	private void SpawnMuzzleFlash(Attack attack)
	{
		if (PAUSE.isPaused) return;
		if (!hasMuzzleFlash) return;
		var newMuzzleFlash = MAKER.Make(ASSETS.FX.muzzleFlashPrefab, attack.DamageOrigin);
		MAKER.Unmake(newMuzzleFlash, .05f);
	}
}
