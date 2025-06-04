using UnityEngine;

namespace GangstaBean.Pickups
{
	public class PoisonDamageEffect:MonoBehaviour
	{
		private float PoisonTime;
		private float PoisonDamage;
		private float damageRate = .5f;
		private float damageRateCounter;
		private Life Target;
		private Life Owner;

		public void StartPoisonEffect(float poisonTime, float poisonDamage, Life target, Life owner)
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
			{
				Destroy(this);
			}
		}

		private void ApplyPoisonDamage()
		{
			var transformPosition = Target.transform.position;
			var origin = (int) Random.Range(0, 2) == 1 ? transformPosition+new Vector3(-1,-1) : transformPosition+ new Vector3(1, -1);
			var poisonAttack = new Attack(Owner, origin, transformPosition,Target, PoisonDamage);
			poisonAttack.IsPoison = true;
			poisonAttack.color = Color.green;
			Target.TakeDamage(poisonAttack);
		}
	}
}