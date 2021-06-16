using UnityEngine;

namespace _SCRIPTS
{
	public class ExplosionHandler : MonoBehaviour
	{
		private ThrownProjectile projectile;
		[SerializeField] private GameObject ExplosionPrefab;
		private float explosionRadius = 30;
		private float explosionDamage = 30;

		private void Start()
		{
			projectile = GetComponent<ThrownProjectile>();
			projectile.OnHitTarget += Explode;
			projectile.OnHitNothing += Explode;
		}

		private void Explode(DefenceHandler target, Vector3 explosionPosition)
		{
			Explode(explosionPosition);
		}

		private void Explode(Vector3 explosionPosition)
		{
			Debug.Log("Explode!");
			var explosion = MAKER.Make(ExplosionPrefab, explosionPosition);
			MAKER.Unmake(explosion, 3);

			var hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, ASSETS.layers.EnemyLayer);
			if (hits == null) return;
			foreach (var hit in hits)
			{
				var defence = hit.GetComponent<DefenceHandler>();
				if (defence is null) continue;

				var ratio = explosionRadius / Vector3.Distance(hit.transform.position, explosionPosition);
				var newAttack = new Attack( explosionPosition, hit.transform.position, explosionDamage * ratio);
				defence.TakeDamage(newAttack);
			}
		}
	}
}
