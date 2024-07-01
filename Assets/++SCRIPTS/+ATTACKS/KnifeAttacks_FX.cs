using __SCRIPTS._COMMON;
using __SCRIPTS._FX;
using UnityEngine;

namespace __SCRIPTS._ATTACKS
{
	public class KnifeAttacks_FX : MonoBehaviour
	{
		private KnifeAttacks knifeAttacks;

		void OnEnable()
		{
			knifeAttacks = GetComponent<KnifeAttacks>();
			knifeAttacks.OnHit += KnifeAttacks_OnHit;
			
		}

		private void OnDisable()
		{ 
			knifeAttacks.OnHit -= KnifeAttacks_OnHit;
			
		}

		private void KnifeAttacks_OnHit(Vector2 pos)
		{
			Maker.Make(FX.Assets.hit5_xstrike, pos);
		}

		
	}
}