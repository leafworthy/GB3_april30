
using UnityEngine;

namespace __SCRIPTS
{
	public class AK_GunAttacks_FX : MonoBehaviour
	{
		private GunAttack_AK_Glock gunAttackAkGlock;
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

		private static void CreateBullet(Attack attack, Vector2 attackStartPosition)
		{
			var newBullet = ObjectMaker.I.Make(ASSETS.FX.BulletPrefab, attack.OriginFloorPoint);

			var bulletScript = newBullet.GetComponent<Bullet_FX>();
			bulletScript.Fire(attack, attackStartPosition);
			CameraShaker.ShakeCamera((Vector2)attack.OriginFloorPoint, CameraShaker.ShakeIntensityType.normal);
		}

		private static void MakeBulletShell(Attack attack)
		{
			var newBulletShell = ObjectMaker.I.Make(ASSETS.FX.bulletShellPrefab, (Vector2) attack.OriginFloorPoint);
			newBulletShell.GetComponent<FallToFloor>()
			              .Fire(attack, true);

			ObjectMaker.I.Unmake(newBulletShell, 5);
		}

		private void CreateBulletHitAnimation(Attack attack)
		{
			var heightCorrectionForDepth = new Vector2(0,-1.25f);

			var newBulletHitAnimation = ObjectMaker.I.Make(ASSETS.FX.bulletHitAnimPrefab, attack.DestinationFloorPoint + heightCorrectionForDepth);

			Debug.DrawLine( attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.magenta, 5);

			var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
			bulletHitHeight.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y);
			ObjectMaker.I.Unmake(newBulletHitAnimation, 5);
		}


	}
}
