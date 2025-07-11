
using UnityEngine;

namespace __SCRIPTS
{
	public class AK_GunAttacks_FX : ServiceUser
	{
		private GunAttack_AK_Glock gunAttackAkGlock;
		private GameObject bulletPrefab => assets.FX.BulletPrefab;

		private void OnEnable()
		{
			gunAttackAkGlock = GetComponent<GunAttack_AK_Glock>();
			gunAttackAkGlock.OnShotHitTarget += GunAttackAkGlockOnOnOnShotHitTarget;
			gunAttackAkGlock.OnShotMissed += GunAttackAkGlockOnOnOnShotHitTarget;
		}


		private void OnDisable()
		{
			gunAttackAkGlock.OnShotHitTarget -= GunAttackAkGlockOnOnOnShotHitTarget;
			gunAttackAkGlock.OnShotMissed -= GunAttackAkGlockOnOnOnShotHitTarget;

		}

		private void GunAttackAkGlockOnOnOnShotHitTarget(Attack attack, Vector2 attackStartPosition)
		{

			CreateBullet(attack, attackStartPosition);

			CreateBulletHitAnimation(attack);

			MakeBulletShell(attack);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Short);
		}

		private void CreateBullet(Attack attack, Vector2 attackStartPosition)
		{
			var newBullet = objectMaker.Make( bulletPrefab, attack.OriginFloorPoint);

			var bulletScript = newBullet.GetComponent<Bullet_FX>();
			bulletScript.Fire(attack, attackStartPosition);
			CameraShaker.ShakeCamera((Vector2)attack.OriginFloorPoint, CameraShaker.ShakeIntensityType.normal);
		}

		private void MakeBulletShell(Attack attack)
		{
			var newBulletShell = objectMaker.Make( assets.FX.bulletShellPrefab, (Vector2) attack.OriginFloorPoint);
			newBulletShell.GetComponent<FallToFloor>()
			              .Fire(attack, true);

			objectMaker.Unmake(newBulletShell, 5);
		}

		private void CreateBulletHitAnimation(Attack attack)
		{
			var heightCorrectionForDepth = new Vector2(0,-1.25f);

			var newBulletHitAnimation = objectMaker.Make( assets.FX.bulletHitAnimPrefab, attack.DestinationFloorPoint + heightCorrectionForDepth);

			Debug.DrawLine( attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.magenta, 5);

			var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
			bulletHitHeight.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y);
			objectMaker.Unmake(newBulletHitAnimation, 5);
		}


	}
}
