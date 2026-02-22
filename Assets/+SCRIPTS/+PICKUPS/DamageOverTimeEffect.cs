using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DamageOverTimeEffect : MonoBehaviour
	{
		private float duration;
		private float damageRateCounter;
		private float damageRate;
		private Life target;
		private ICanAttack owner;
		private Color color;
		private float damage;
		private GameObject particles;

		public void StartEffect(ICanAttack _owner, Life _target, float _duration, float _damageRate, float _damage, Color _color)
		{
			owner = _owner;
			target = _target;
			duration = _duration;
			damageRate = _damageRate;
			damageRateCounter = 0;
			color = _color;
			damage = _damage;
			particles = Services.objectMaker.Make(Services.assetManager.FX.fireParticlesPrefab, transform.position);
			particles.transform.SetParent(target.transform, true);
			target.OnDead += Target_OnDead;
		}

		void Target_OnDead(Attack obj)
		{
			Destroy(particles);
			Destroy(this);
		}

		public void FixedUpdate()
		{
			if (target.IsDead()) return;
			if (duration > 0)
			{
				if (damageRateCounter <= 0)
				{
					damageRateCounter = damageRate;
					ApplyDamage();
				}
				else
				{
					duration -=   Time.fixedDeltaTime;
					damageRateCounter -= Time.fixedDeltaTime;
				}
			}
			else
			{
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
