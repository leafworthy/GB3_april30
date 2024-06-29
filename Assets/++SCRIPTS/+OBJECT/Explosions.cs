using UnityEngine;

public class Explosions:Singleton<Explosions>
{
	public void Explode(Vector3 explosionPosition, float explosionRadius, float explosionDamage, Player _owner)
	{
		var pushFactor = 10;
		Maker.Make(ASSETS.FX.explosions.GetRandom(), explosionPosition);
		Maker.Make(ASSETS.FX.fires.GetRandom(), explosionPosition);

		var layer = _owner.IsPlayer() ? ASSETS.LevelAssets.EnemyLayer : ASSETS.LevelAssets.PlayerLayer;
		var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, layer);
		if (hits == null) return;
		foreach (var hit in hits)
		{
			var defence = hit.GetComponent<Life>();
			if (defence is null) continue;
			var ratio = explosionRadius / Vector3.Distance(hit.transform.position, explosionPosition);
			
			var otherMove = defence.GetComponent<MoveAbility>();
			if(otherMove != null)
				otherMove.Push(explosionPosition - defence.transform.position, pushFactor * ratio);
			var newAttack = new Attack(_owner.spawnedPlayerDefence, explosionPosition, defence.transform.position, defence, explosionDamage * ratio);
			newAttack.IsDamaging = true;
			defence.TakeDamage(newAttack);
		}
		CameraShaker.ShakeCamera(explosionPosition, CameraShaker.ShakeIntensityType.high);
		ASSETS.sounds.bean_nade_explosion_sounds.PlayRandom();
	}

}