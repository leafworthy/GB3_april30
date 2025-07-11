using UnityEngine;

namespace __SCRIPTS
{
	public class KnifeAttacks_FX : ServiceUser
	{
		private TertiaryAttack_Knife tertiaryAttackKnife;

		void OnEnable()
		{
			tertiaryAttackKnife = GetComponent<TertiaryAttack_Knife>();
			tertiaryAttackKnife.OnHit += TertiaryAttackKnifeOnHit;

		}

		private void OnDisable()
		{
			tertiaryAttackKnife.OnHit -= TertiaryAttackKnifeOnHit;

		}

		private void TertiaryAttackKnifeOnHit(Vector2 pos)
		{
			objectMaker.Make( assets.FX.hit5_xstrike, pos);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}


	}
}
