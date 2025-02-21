using UnityEngine;

public class Explosion_FX: MonoBehaviour
{
	public static void Explode(Vector3 explosionPosition, float explosionRadius, float explosionDamage, Player _owner)
	{
		var pushFactor = 10;
		ObjectMaker.Make(FX.Assets.explosions.GetRandom(), explosionPosition);
		ObjectMaker.Make(FX.Assets.fires.GetRandom(), explosionPosition);

		var layer = _owner.IsPlayer() ? ASSETS.LevelAssets.EnemyLayer : ASSETS.LevelAssets.PlayerLayer;
		var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, layer);
		
		CameraShaker_FX.ShakeCamera(explosionPosition, CameraShaker_FX.ShakeIntensityType.high);
		CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		SFX.sounds.bean_nade_explosion_sounds.PlayRandomAt(explosionPosition);
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
			newAttack.IsDamaging = true;
			defence.TakeDamage(newAttack);
		}
	}

}