using UnityEngine;

namespace _SCRIPTS
{
	public class Brock_SFXHandler : MonoBehaviour
	{
		private AnimationEvents animEvents;
		private BrockAttackHandler attackHandler;

		private void Awake()
		{
			animEvents = GetComponentInChildren<AnimationEvents>();
			attackHandler = GetComponentInChildren<BrockAttackHandler>();

			animEvents.OnStep += Anim_OnStep;
			animEvents.OnDash += Anim_OnTeleport;
			animEvents.OnAttackHit += Anim_OnAttackHit;
			animEvents.OnAttackStart += Anim_OnBatSwing;
			animEvents.OnThrow += Anim_Throw;
			animEvents.OnHitStart += Anim_HitStart;
		}

		private void Anim_HitStart()
		{
			ASSETS.sounds.brock_gethit_sounds.PlayRandom();
		}

		private void Anim_Throw()
		{
			ASSETS.sounds.brock_bat_swing_sounds.PlayRandom();
		}

		private void Anim_OnBatSwing(int obj)
		{
			ASSETS.sounds.brock_bat_swing_sounds.PlayRandom();
		}


		private void Anim_OnTeleport()
		{
			ASSETS.sounds.brock_teleport_sounds.PlayRandom();
		}

		private void Anim_OnAttackHit(int attackID)
		{
			if (attackID == 5)
			{

				ASSETS.sounds.brock_homerunhit_sounds.PlayRandom();
				return;
			}
			ASSETS.sounds.brock_bathit_sounds.PlayRandom();
		}


		private void Anim_OnStep()
		{
			ASSETS.sounds.player_walk_sounds_concrete.PlayRandom();
		}
	}


	}
