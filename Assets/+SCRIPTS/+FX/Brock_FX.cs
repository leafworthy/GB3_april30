using __SCRIPTS;
using UnityEngine;

namespace GangstaBean.Audio
{
	[DisallowMultipleComponent]
	public class Brock_FX : ServiceUser
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
			objectMaker.Make( AssetManager.FX.hits.GetRandom(), vector2);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}

		private void SecondaryAttackChargeAttackOnSpecialAttackHit()
		{
			objectMaker.Make( AssetManager.FX.hit2_biglongflash, transform.position);
			objectMaker.Make( AssetManager.FX.hit5_line_burst, transform.position);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Long);
		}
	}
}
