using UnityEngine;

namespace __SCRIPTS
{
	public class KnifeAttacks_FX : MonoBehaviour
	{
		KnifeAttack knifeAttack;

		void OnEnable()
		{
			knifeAttack = GetComponent<KnifeAttack>();
			if (knifeAttack == null) return;
			knifeAttack.OnHit += KnifeAttackOnHit;

		}

		void OnDisable()
		{
			if (knifeAttack == null) return;
			knifeAttack.OnHit -= KnifeAttackOnHit;

		}

		void KnifeAttackOnHit(Vector2 pos)
		{
			Services.objectMaker.Make(Services.assetManager.FX.hit5_xstrike, pos);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}


	}
}
