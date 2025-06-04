using UnityEngine;

namespace GangstaBean.Effects
{
	public class KnifeAttacks_FX : MonoBehaviour
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
			ObjectMaker.I.Make(ASSETS.FX.hit5_xstrike, pos);
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Normal);
		}

		
	}
}