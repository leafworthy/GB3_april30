using UnityEngine;

namespace _SCRIPTS
{
	public class GunAttackFX : MonoBehaviour
	{
		private BeanAttackHandler gunAttackHandler;

		[SerializeField] private GameObject aimTargetGraphic;
		[SerializeField] private Transform aimLightTransform;
		private BeanAttackHandler aimer;
		[SerializeField] private bool hasMuzzleFlash;
		private UnitStats stats;


		private void Awake()
		{
			aimer = GetComponent<BeanAttackHandler>();
			aimer.OnAim += AimerOnAim;

			gunAttackHandler = GetComponent<BeanAttackHandler>();
			gunAttackHandler.OnAttackStart += GunAttackHandlerOnAttack;
			stats = GetComponent<UnitStats>();
		}

		private void AimerOnAim(Vector3 aimDir)
		{
			if (aimTargetGraphic != null)
			{
				aimTargetGraphic.transform.position = aimer.GetAimCenter();
			}

			if (aimLightTransform != null)
			{
				float lightRotation = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
				aimLightTransform.eulerAngles = new Vector3(0, 0, lightRotation);
			}

		}


		private void GunAttackHandlerOnAttack(Attack attack)
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
			SHAKER.ShakeCamera(attack.DamageOrigin, SHAKER.ShakeIntensityType.medium);
			AUDIO.I.PlaySound(ASSETS.sounds.shoot_sounds.GetRandom());
		}

		private void SpawnMuzzleFlash(Attack attack)
		{
			if (!hasMuzzleFlash) return;
			var newMuzzleFlash = MAKER.Make(ASSETS.FX.muzzleFlashPrefab, attack.DamageOrigin);
			MAKER.Unmake(newMuzzleFlash, .05f);
		}
	}
}
