using __SCRIPTS;
using UnityEngine;

namespace GangstaBean.Audio
{
	public class Brock_FX : MonoBehaviour
	{
		private BatAttack meleeAttack;
		private ChargeAttack chargeAttack;

		private void OnEnable()
		{
			meleeAttack = GetComponent<BatAttack>();
			meleeAttack.OnHitTarget += MeleeAttackOnHitTarget;
			chargeAttack = GetComponent<ChargeAttack>();
			chargeAttack.OnSpecialAttackHit += ChargeAttackOnSpecialAttackHit;
		}

		private void MeleeAttackOnHitTarget(Vector2 vector2)
		{
			Services.objectMaker.Make(Services.assetManager.FX.hits.GetRandom(), vector2);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}

		private void ChargeAttackOnSpecialAttackHit()
		{
			Services.objectMaker.Make(Services.assetManager.FX.hit2_biglongflash, transform.position);
			Services.objectMaker.Make(Services.assetManager.FX.hit5_line_burst, transform.position);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Long);
		}
	}
}
