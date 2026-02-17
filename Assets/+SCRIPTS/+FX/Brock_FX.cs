using __SCRIPTS;
using UnityEngine;

namespace GangstaBean.Audio
{
	public class Brock_FX : MonoBehaviour
	{
		BatAttack meleeAttack;
		ChargeAttack chargeAttack;

		void OnEnable()
		{
			meleeAttack = GetComponent<BatAttack>();
			meleeAttack.OnHitTarget += MeleeAttackOnHitTarget;
			chargeAttack = GetComponent<ChargeAttack>();
			chargeAttack.OnSpecialAttackHit += ChargeAttackOnSpecialAttackHit;
			chargeAttack.OnAttackHit += ChargeAttackOnAttackHit;
		}

		void ChargeAttackOnAttackHit(Attack attack)
		{
			var hit = Services.objectMaker.Make(Services.assetManager.FX.hits.GetRandom(), attack.DestinationFloorPoint);
			hit.transform.localScale = new Vector3(hit.transform.localScale.x * Mathf.Sign(attack.Direction.x), hit.transform.localScale.y, 0);
		}

		void MeleeAttackOnHitTarget(Attack attack)
		{
			var hit = Services.objectMaker.Make(Services.assetManager.FX.hits.GetRandom(), attack.DestinationFloorPoint);
			hit.transform.localScale = new Vector3(hit.transform.localScale.x * Mathf.Sign(attack.Direction.x), hit.transform.localScale.y, 0);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}

		void ChargeAttackOnSpecialAttackHit(Attack attack)
		{
			var hit = Services.objectMaker.Make(Services.assetManager.FX.hit2_biglongflash, attack.DestinationFloorPoint);
			hit.transform.localScale = new Vector3(hit.transform.localScale.x * Mathf.Sign(attack.Direction.x), hit.transform.localScale.y, 0);
			hit = Services.objectMaker.Make(Services.assetManager.FX.hit5_line_burst, attack.DestinationFloorPoint);
			hit.transform.localScale = new Vector3(hit.transform.localScale.x * Mathf.Sign(attack.Direction.x), hit.transform.localScale.y, 0);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Long);
		}
	}
}
