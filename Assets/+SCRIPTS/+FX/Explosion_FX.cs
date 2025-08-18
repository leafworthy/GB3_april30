using UnityEngine;

namespace __SCRIPTS
{	[DisallowMultipleComponent]
	public class Explosion_FX: MonoBehaviour
	{

		public static void Explode(Vector3 explosionPosition, float explosionRadius, float explosionDamage, Player _owner)
		{
			var assets = ServiceLocator.Get<AssetManager>();
			var pushFactor = 10;
			var objectMaker = ServiceLocator.Get<ObjectMaker>();
			objectMaker.Make( assets.FX.explosions.GetRandom(), explosionPosition);
			objectMaker.Make( assets.FX.fires.GetRandom(), explosionPosition);

			var layer = _owner.IsPlayer() ? assets.LevelAssets.EnemyLayer : assets.LevelAssets.PlayerLayer;
			var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, layer);

			CameraShaker.ShakeCamera(explosionPosition, CameraShaker.ShakeIntensityType.high);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
			var sfx = ServiceLocator.Get<SFX>();
			sfx.sounds.bean_nade_explosion_sounds.PlayRandomAt(explosionPosition);
			if (hits == null) return;
			foreach (var hit in hits)
			{
				var defence = hit.GetComponent<Life>();
				if (defence is null) continue;
				var ratio = explosionRadius / Vector3.Distance(hit.transform.position, explosionPosition);

				var otherMove = defence.GetComponent<MoveAbility>();
				if (otherMove != null)
					otherMove.Push(explosionPosition - defence.transform.position, pushFactor * ratio);
				var newAttack = new Attack(_owner.spawnedPlayerDefence, explosionPosition, defence.transform.position,
					defence, explosionDamage * ratio);
				//newAttack.IsWounding = true;
				defence.TakeDamage(newAttack);
			}
		}

	}
}
