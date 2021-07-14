using System;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
	private IExplode projectile;
	private float explosionRadius = 30;
	private float explosionDamage = 30;
	private IAttackHandler owner;

	private void Start()
	{
		projectile = GetComponent<IExplode>();
		projectile.OnHitTarget += Explode;
		projectile.OnHitNothing += Explode;
	}

	private void Explode(DefenceHandler target, Vector3 explosionPosition, IAttackHandler _owner)
	{
		owner = _owner;
		Explode(explosionPosition, _owner);
	}

	private void Explode(Vector3 explosionPosition, IAttackHandler _owner)
	{
		owner = _owner;
		Debug.Log("Explode!");
		var explosion = MAKER.Make(ASSETS.FX.explosionPrefab, explosionPosition);
		MAKER.Unmake(explosion, 3);

		var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, ASSETS.LevelAssets.EnemyLayer);
		if (hits == null) return;
		foreach (var hit in hits)
		{
			var defence = hit.GetComponent<DefenceHandler>();
			if (defence is null) continue;

			var ratio = explosionRadius / Vector3.Distance(hit.transform.position, explosionPosition);
			var newAttack = new Attack( explosionPosition, hit.transform.position, explosionDamage * ratio, false,HITSTUN.StunLength.Long,
				true, owner);
			defence.TakeDamage(newAttack);
		}
		ASSETS.sounds.bean_nade_explosion_sounds.PlayRandom();
	}
}

internal interface IExplode
{
	public event Action<DefenceHandler, Vector3,IAttackHandler> OnHitTarget;
	public event Action<Vector3, IAttackHandler> OnHitNothing;
}
