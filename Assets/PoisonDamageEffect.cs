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
		var transformPosition = Target.transform.position;
		var origin = (int) Random.Range(0, 2) == 1 ? transformPosition+Vector3.left : transformPosition+Vector3.right;
		Target.TakeDamage(new Attack(origin, transformPosition, PoisonDamage));
	}
}
