using UnityEngine;

namespace _SCRIPTS
{
	public class Cone_SFXHandler : MonoBehaviour
	{
		private AnimationEvents animEvents;

		private void Awake()
		{
			animEvents = GetComponentInChildren<AnimationEvents>();
			animEvents.OnAttackHit += Anim_OnAttackHit;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnRoar += Anim_OnRoar;
			animEvents.OnHitStart += Anim_OnHit;
		}

		private void Anim_OnHit()
		{

			ASSETS.sounds.cone_gethit_sounds.PlayRandom();
		}

		private void Anim_OnRoar()
		{
			ASSETS.sounds.cone_roar_sounds.PlayRandom();
		}

		private void Anim_OnAttackHit(int attackType)
		{
			ASSETS.sounds.cone_attack_sounds.PlayRandom();
		}

		private void Anim_OnStep()
		{
			ASSETS.sounds.cone_walk_sounds.PlayRandom();
		}
	}
}
