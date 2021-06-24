using UnityEngine;

namespace _SCRIPTS
{
	public class Bean_SFXHandler : MonoBehaviour
	{
		private AnimationEvents animEvents;
		private BeanAttackHandler attackHandler;

		private void Awake()
		{
			animEvents = GetComponentInChildren<AnimationEvents>();
			attackHandler = GetComponentInChildren<BeanAttackHandler>();

			attackHandler.OnShootStart += AnimOnShootStart;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnDash += Anim_OnRoll;
			animEvents.OnAttackHit += Anim_OnKnifeHit;
			animEvents.OnThrow += Anim_OnNadeThrow;
			animEvents.OnReload += Anim_OnReload;
		}

		private void Anim_OnReload()
		{
			ASSETS.sounds.bean_reload_sounds.PlayRandom();
		}

		private void Anim_OnNadeThrow()
		{
			ASSETS.sounds.bean_nade_throw_sounds.PlayRandom();
		}

		private void Anim_OnRoll()
		{
			ASSETS.sounds.bean_roll_sounds.PlayRandom();
		}


		private void Anim_OnKnifeHit(int i)
		{
			ASSETS.sounds.bean_knifehit_sounds.PlayRandom();
		}

		private void AnimOnShootStart(Attack attack)
		{

			ASSETS.sounds.ak47_shoot_sounds.PlayRandom();
		}

		private void Anim_OnStep()
		{
			ASSETS.sounds.player_walk_sounds_concrete.PlayRandom();
		}
	}
}
