
using __SCRIPTS._COMMON;
using __SCRIPTS._OBJECT;
using UnityEngine;

namespace __SCRIPTS._SFX
{
	public class Explosion_SFX : MonoBehaviour
	{
		private void OnEnable()
		{
			ExplosionManager.OnExplosion += Instance_OnExplosion;
		}

		private void Instance_OnExplosion()
		{
			SFX.sounds.bean_nade_explosion_sounds.PlayRandom();
		}
	}
}