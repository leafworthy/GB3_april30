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


		private void GunAttackHandlerOnAttack(OnAttackEventArgs e)
		{
			var newBullet = MAKER.Make(ASSETS.FX.bulletPrefab, e.AttackStartPosition);

			var bulletScript = newBullet.GetComponent<BulletFX>();
			bulletScript.Fire(e.AttackStartPosition, e.AttackEndPosition);

			var newBulletHitAnimation = MAKER.Make(ASSETS.FX.bulletHitAnimPrefab, e.AttackEndPosition);
			MAKER.Unmake(newBulletHitAnimation, 5);

			if (hasMuzzleFlash)
			{
				var newMuzzleFlash = MAKER.Make(ASSETS.FX.muzzleFlashPrefab, e.AttackStartPosition);
				MAKER.Unmake(newMuzzleFlash, .05f);
			}

			var newBulletShell = MAKER.Make(ASSETS.FX.bulletShellPrefab, e.AttackStartPosition);
			newBulletShell.GetComponent<FallToFloor>().Fire((e.AttackStartPosition - e.AttackEndPosition).normalized);
			SHAKER.ShakeCamera(e.AttackStartPosition, stats.attackDamage/30);
			AUDIO.I.PlaySound(ASSETS.sounds.shoot_sounds.GetRandom());
		}

	}
}
