using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class PoisonDamageEffect : MonoBehaviour
	{
		private float PoisonTime;
		private float PoisonDamage;
		private float damageRate = .5f;
		private float damageRateCounter;
		private Life Target;
		private ICanAttack Owner;

		public void StartPoisonEffect(float poisonTime, float poisonDamage, Life target, ICanAttack owner)
		{
			Owner = owner;
			Target = target;
			PoisonTime = poisonTime;
			PoisonDamage = poisonDamage;
		}

		private void FixedUpdate()
		{
			if (PoisonTime > 0)
			{
				if (damageRateCounter <= 0)
				{
					damageRateCounter = damageRate;
					ApplyPoisonDamage();
				}
				else
				{
					PoisonTime -= Time.fixedDeltaTime;
					damageRateCounter -= Time.fixedDeltaTime;
				}
			}
			else
				Destroy(this);
		}

		private void ApplyPoisonDamage()
		{
			var transformPosition = Target.transform.position;
			var origin = Random.Range(0, 2) == 1 ? transformPosition + new Vector3(-1, -1) : transformPosition + new Vector3(1, -1);
			var poisonAttack = Attack.Create(Owner, Target).WithOriginPoint(origin).WithDamage(PoisonDamage).WithDebree().WithTint(Color.green);
			Target.TakeDamage(poisonAttack);
		}
	}
}
