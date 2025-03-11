using UnityEngine;

public class KnifeAttacks_FX : MonoBehaviour
{
	private KnifeAttack knifeAttack;

	void OnEnable()
	{
		knifeAttack = GetComponent<KnifeAttack>();
		knifeAttack.OnHit += KnifeAttackOnHit;
			
	}

	private void OnDisable()
	{ 
		knifeAttack.OnHit -= KnifeAttackOnHit;
			
	}

	private void KnifeAttackOnHit(Vector2 pos)
	{
		ObjectMaker.Make(ASSETS.FX.hit5_xstrike, pos);
		CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
	}

		
}