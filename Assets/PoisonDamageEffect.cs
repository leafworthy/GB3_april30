using _SCRIPTS;
using UnityEngine;

public class PoisonDamageEffect:MonoBehaviour
{
	private float PoisonTime;
	private float PoisonDamage;
	private float damageRate = .5f;
	private float damageRateCounter;
	private DefenceHandler Target;

	public void StartPoisonEffect(float poisonTime, float poisonDamage, DefenceHandler target)
	{
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
		Target.TakeDamage((int)Random.Range(0,2) == 1? Vector3.left: Vector3.right, PoisonDamage, Target.transform.position, true);
	}
}
