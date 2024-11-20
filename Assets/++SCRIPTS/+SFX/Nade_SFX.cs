using UnityEngine;

public class Nade_SFX : MonoBehaviour
{
	private void OnEnable()
	{
		ExplosionManager.OnExplosion += Instance_OnExplosion;
	}

	private void Instance_OnExplosion()
	{
		SFX.sounds.bean_nade_explosion_sounds.PlayRandomAt(transform.position);
	}
}