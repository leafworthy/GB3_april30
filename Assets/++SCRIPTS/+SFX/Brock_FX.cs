using UnityEngine;

public class Brock_FX : MonoBehaviour
{
	private BatAttack meleeAttack;
	private ChargeAttack chargeAttack;

	private void OnEnable()
	{
		meleeAttack = GetComponent<BatAttack>();
		meleeAttack.OnHit += MeleeAttackOnHit;
		chargeAttack = GetComponent<ChargeAttack>();
		chargeAttack.OnSpecialAttackHit += ChargeAttack_OnSpecialAttackHit;
	}

	private void MeleeAttackOnHit(Vector2 vector2)
	{ 
		ObjectMaker.Make(ASSETS.FX.hits.GetRandom(), vector2);
		CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
	}

	private void ChargeAttack_OnSpecialAttackHit()
	{
		ObjectMaker.Make(ASSETS.FX.hit2_biglongflash, transform.position);
		ObjectMaker.Make(ASSETS.FX.hit5_line_burst, transform.position);
		CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Long);
	}
}