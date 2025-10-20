using UnityEngine;

namespace __SCRIPTS
{
	public class DamageOverTimeEffect : MonoBehaviour
	{
		private float duration;
		private float damageRateCounter;
		private float damageRate;
		private Life target;
		private Life owner;
		private Color color;
		private float damage;
		private GameObject particles;

		public void StartEffect(Life _owner, Life _target, float _duration, float _damageRate, float _damage, Color _color)
		{
			owner = _owner;
			target = _target;
			duration = _duration;
			damageRate = _damageRate;
			damageRateCounter = 0;
			color = _color;
			damage = _damage;
			Debug.Log("effect started");
			particles = Services.objectMaker.Make(Services.assetManager.FX.fireParticlesPrefab, transform.position);
			particles.transform.SetParent(target.transform, true);

		}

		public void FixedUpdate()
		{
			if (duration > 0)
			{
				if (damageRateCounter <= 0)
				{
					damageRateCounter = damageRate;
					ApplyDamage();
				}
				else
				{
					Debug.Log("effect tick");
					duration -=   Time.fixedDeltaTime;
					damageRateCounter -= Time.fixedDeltaTime;
				}
			}
			else
			{
				Debug.Log("destroyed effect");
				Destroy(particles);
				Destroy(this);
			}
		}



		private void ApplyDamage()
		{
			var transformPosition = target.transform.position;
			var origin = Random.Range(0, 2) == 1 ? transformPosition + new Vector3(-1, -1) : transformPosition + new Vector3(1, -1);
			var attack = Attack.Create(owner,target).WithDebree().WithTint(color).WithDamage(damage).WithOriginPoint(origin);
			target.TakeDamage(attack);
		}
	}
}
