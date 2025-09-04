using UnityEngine;

namespace __SCRIPTS
{
	public class KnifeAttacks_FX : MonoBehaviour
	{
		private DoableKnifeAttack knifeAttack;

		void OnEnable()
		{
			knifeAttack = GetComponent<DoableKnifeAttack>();
			if (knifeAttack == null) return;
			knifeAttack.OnHit += KnifeAttackOnHit;

		}

		private void OnDisable()
		{
			if (knifeAttack == null) return;
			knifeAttack.OnHit -= KnifeAttackOnHit;

		}

		private void KnifeAttackOnHit(Vector2 pos)
		{
			Services.objectMaker.Make(Services.assetManager.FX.hit5_xstrike, pos);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}


	}
}
