using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class Gun_FX : MonoBehaviour
	{
		List<Gun> guns => _guns ??= GetComponents<Gun>().ToList();
		List<Gun> _guns;
		GameObject bulletPrefab => Services.assetManager.FX.BulletPrefab;
		float effectTime = 5f;
		Vector2 heightCorrectionForDepthInFrontOfWall = new(0, -1.25f);

		void Start()
		{
			foreach (var gun in guns)
			{
				gun.OnShotHitTarget += Gun_OnShoot;
				gun.OnShotMissed += Gun_OnShoot;
			}
		}

		void OnDestroy()
		{
			foreach (var gun in guns)
			{
				gun.OnShotHitTarget -= Gun_OnShoot;
				gun.OnShotMissed -= Gun_OnShoot;
			}
		}

		void Gun_OnShoot(Attack attack)
		{
			CreateBullet(attack);
			CreateBulletHitAnimation(attack);
			MakeBulletShell(attack);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Short);
		}

		void CreateBullet(Attack attack)
		{
			var newBullet = Services.objectMaker.Make(bulletPrefab, attack.OriginWithHeight);
			newBullet.GetComponent<Bullet_FX>().Fire(attack);
			CameraShaker.ShakeCamera(attack.OriginFloorPoint, CameraShaker.ShakeIntensityType.normal);
		}

		void MakeBulletShell(Attack attack)
		{
			var newBulletShell = Services.objectMaker.Make(Services.assetManager.FX.bulletShellPrefab, attack.OriginFloorPoint);
			newBulletShell.GetComponent<MoveJumpAndRotateAbility>().Fire(attack.FlippedDirection, attack.OriginHeight);
			Services.objectMaker.Unmake(newBulletShell, effectTime);
		}

		void CreateBulletHitAnimation(Attack attack)
		{
			var newBulletHitAnimation = Services.objectMaker.Make(Services.assetManager.FX.bulletHitAnimPrefab,
				attack.DestinationFloorPoint + heightCorrectionForDepthInFrontOfWall);
			var bulletHitHeight = newBulletHitAnimation.GetComponent<HeightAbility>();
			bulletHitHeight.SetHeight(attack.DestinationHeight - heightCorrectionForDepthInFrontOfWall.y);
			Services.objectMaker.Unmake(newBulletHitAnimation, effectTime);
		}
	}
}
