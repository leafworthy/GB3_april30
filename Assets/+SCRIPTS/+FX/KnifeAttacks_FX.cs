using UnityEngine;

namespace __SCRIPTS
{
	public class KnifeAttacks_FX : ServiceUser
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
			objectMaker.Make( assets.FX.hit5_xstrike, pos);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}


	}
}
