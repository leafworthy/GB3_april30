using UnityEngine;

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