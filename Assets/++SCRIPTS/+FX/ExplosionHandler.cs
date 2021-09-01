using System;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
	private IExplode projectile;
	private const float explosionRadius = 30;
	private const float explosionDamage = 30;
	private Player owner;

	private void Start()
	{
		projectile = GetComponent<IExplode>();
		projectile.OnExplode += Explode;
	}

	private void Explode(Vector3 explosionPosition, Player _owner)
	{
		owner = _owner;
		Debug.Log("Explode!");
		var explosion = MAKER.Make(ASSETS.FX.explosionPrefab, explosionPosition);

		var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, ASSETS.LevelAssets.EnemyLayer);
		if (hits == null) return;
		foreach (var hit in hits)
		{
			var defence = hit.GetComponent<DefenceHandler>();
			if (defence is null) continue;

			var ratio = explosionRadius / Vector3.Distance(hit.transform.position, explosionPosition);
			var newAttack = new Attack( explosionPosition, hit.transform.position, explosionDamage * ratio, false,STUNNER.StunLength.Long,
				true, owner);
			defence.TakeDamage(newAttack);
		}
		SHAKER.ShakeCamera(explosionPosition,SHAKER.ShakeIntensityType.high);
		ASSETS.sounds.bean_nade_explosion_sounds.PlayRandom();
	}
}

internal interface IExplode
{
	public event Action<Vector3, Player> OnExplode;
}
