using UnityEngine;

namespace _SCRIPTS
{
	public class OnAttackHitFX : MonoBehaviour
	{
		private AnimationEvents animEvents;

		private void Awake()
		{
			animEvents = GetComponentInChildren<AnimationEvents>();
			animEvents.OnAttackHit += Anim_OnAttackHit;
		}

		private void Anim_OnAttackHit(int attackType)
		{
			//SHAKER.ShakeCamera(transform.position, 1);

			AUDIO.I.PlaySound(ASSETS.sounds.PlayRandom(AudioAssets.SoundType.knifehit));
		}
	}
}
