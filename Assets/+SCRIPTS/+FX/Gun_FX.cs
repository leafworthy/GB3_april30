using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class Gun_FX : MonoBehaviour
	{
		private List<Gun> guns => _guns ??= GetComponents<Gun>().ToList();
		private List<Gun> _guns;
		private GameObject bulletPrefab => Services.assetManager.FX.BulletPrefab;
		private float effectTime = 5f;
		private Vector2 heightCorrectionForDepth = new(0, -1.25f);

		private void Start()
		{
			foreach (var gun in guns)
			{
				gun.OnShotHitTarget += Gun_OnShoot;
				gun.OnShotMissed += Gun_OnShoot;
			}
		}

		private void OnDestroy()
		{
			foreach (var gun in guns)
			{
				gun.OnShotHitTarget -= Gun_OnShoot;
				gun.OnShotMissed -= Gun_OnShoot;
			}
		}

		private void Gun_OnShoot(Attack attack)
		{
			CreateBullet(attack);
			CreateBulletHitAnimation(attack);
			MakeBulletShell(attack);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Short);
		}

		private void CreateBullet(Attack attack)
		{
			var newBullet = Services.objectMaker.Make(bulletPrefab, attack.OriginWithHeight);
			newBullet.GetComponent<Bullet_FX>().Fire(attack);
			CameraShaker.ShakeCamera(attack.OriginFloorPoint, CameraShaker.ShakeIntensityType.normal);
		}

		private void MakeBulletShell(Attack attack)
		{
			var newBulletShell = Services.objectMaker.Make(Services.assetManager.FX.bulletShellPrefab, attack.OriginFloorPoint);
			newBulletShell.GetComponent<IDebree>().Fire(Attack.GetFlippedAttack(attack));
			Services.objectMaker.Unmake(newBulletShell, effectTime);
		}

		private void CreateBulletHitAnimation(Attack attack)
		{
			var newBulletHitAnimation = Services.objectMaker.Make(Services.assetManager.FX.bulletHitAnimPrefab, attack.DestinationFloorPoint + heightCorrectionForDepth);
			var bulletHitHeight = newBulletHitAnimation.GetComponent<ThingWithHeight>();
			bulletHitHeight.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y);
			Services.objectMaker.Unmake(newBulletHitAnimation, effectTime);
		}
	}
}
