using UnityEngine;

namespace GangstaBean.Audio
{
	public class Brock_FX : MonoBehaviour
	{
		private TertiaryAttack_BatAttack meleeAttack;
		private SecondaryAttack_ChargeAttack secondaryAttackChargeAttack;

		private void OnEnable()
		{
			meleeAttack = GetComponent<TertiaryAttack_BatAttack>();
			meleeAttack.OnHit += MeleeAttackOnHit;
			secondaryAttackChargeAttack = GetComponent<SecondaryAttack_ChargeAttack>();
			secondaryAttackChargeAttack.OnSpecialAttackHit += SecondaryAttackChargeAttackOnSpecialAttackHit;
		}

		private void MeleeAttackOnHit(Vector2 vector2)
		{ 
			ObjectMaker.I.Make(ASSETS.FX.hits.GetRandom(), vector2);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}

		private void SecondaryAttackChargeAttackOnSpecialAttackHit()
		{
			ObjectMaker.I.Make(ASSETS.FX.hit2_biglongflash, transform.position);
			ObjectMaker.I.Make(ASSETS.FX.hit5_line_burst, transform.position);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Long);
		}
	}
}