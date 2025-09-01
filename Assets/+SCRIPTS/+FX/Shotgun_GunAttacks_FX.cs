
using UnityEngine;

namespace __SCRIPTS
{
	public class Shotgun_GunAttacks_FX : ServiceUser
	{
		private IAimableGunAttack gunAttackShotgun;
		private void OnEnable()
		{
			gunAttackShotgun = GetComponent<IAimableGunAttack>();
			gunAttackShotgun.OnShotHitTarget += GunAttackShotgunOnOnOnShotHitTarget;
			gunAttackShotgun.OnShotMissed += GunAttackShotgunOnOnOnShotHitTarget;
		}


		private void OnDisable()
		{
			gunAttackShotgun.OnShotHitTarget -= GunAttackShotgunOnOnOnShotHitTarget;
			gunAttackShotgun.OnShotMissed -= GunAttackShotgunOnOnOnShotHitTarget;
	}

		private void GunAttackShotgunOnOnOnShotHitTarget(Attack attack, Vector2 attackStartPosition)
		{

			CreateBullet(attack, attackStartPosition);

			CreateBulletHitAnimation(attack);

			MakeBulletShell(attack);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Short);
		}

		private void CreateBullet(Attack attack, Vector2 attackStartPosition)
		{
			var newBullet = objectMaker.Make( assetManager.FX.BulletPrefab, attack.OriginFloorPoint);

			var bulletScript = newBullet.GetComponent<Bullet_FX>();
			bulletScript.Fire(attack, attackStartPosition);
			CameraShaker.ShakeCamera((Vector2)attack.OriginFloorPoint, CameraShaker.ShakeIntensityType.normal);
		}

		private void MakeBulletShell(Attack attack)
		{
			var newBulletShell = objectMaker.Make( assetManager.FX.bulletShellPrefab, (Vector2) attack.OriginFloorPoint);
			newBulletShell.GetComponent<FallToFloor>()
			              .Fire(attack, true);

			objectMaker.Unmake(newBulletShell, 5);
		}

		private void CreateBulletHitAnimation(Attack attack)
		{
			var heightCorrectionForDepth = new Vector2(0,-1.25f);

			var newBulletHitAnimation = objectMaker.Make( assetManager.FX.bulletHitAnimPrefab, attack.DestinationFloorPoint + heightCorrectionForDepth);

			Debug.DrawLine( attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.magenta, 5);

			var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
			bulletHitHeight.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y);
			objectMaker.Unmake(newBulletHitAnimation, 5);
		}


	}
}
