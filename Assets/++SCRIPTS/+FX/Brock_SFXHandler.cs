using UnityEngine;

public class Brock_SFXHandler : MonoBehaviour
{
	private AnimationEvents animEvents;
	private BrockAttackHandler attackHandler;

	private void Awake()
	{
		animEvents = GetComponentInChildren<AnimationEvents>();
		attackHandler = GetComponentInChildren<BrockAttackHandler>();
		attackHandler.OnHitConnected += Attack_OnHitConnected;

		animEvents.OnStep += Anim_OnStep;
		animEvents.OnAttackStart += Anim_AttackStart;
		animEvents.OnAttackStop += Anim_AttackStop;
		animEvents.OnThrow += Anim_Throw;
		animEvents.OnHitStart += Anim_HitStart;
		animEvents.OnTeleport += Anim_OnTeleport;
	}

	private void Anim_AttackStop(int attackType)
	{
		if (attackType == 0)
		{

			AUDIO.I.StopSpecialSound();
		}
	}

	private void Attack_OnHitConnected(int attackType)
	{
		if (attackType == 5)
		{

			AUDIO.I.StartSpecialSound(ASSETS.sounds.brock_homerunhit_sounds.GetRandom());
			return;
		}

		ASSETS.sounds.brock_bathit_sounds.PlayRandom();
	}

	private void Anim_HitStart()
	{
		ASSETS.sounds.brock_gethit_sounds.PlayRandom();
	}

	private void Anim_Throw()
	{
		ASSETS.sounds.brock_bat_swing_sounds.PlayRandom();
	}

	private void Anim_AttackStart(int attackType)
	{
		if (attackType == 5)
		{

			ASSETS.sounds.brock_charge_sounds.PlayRandom();
		}else if (attackType == 0)
		{
			ASSETS.sounds.brock_charge_sounds.PlayRandom();
		}
		else
		{

			ASSETS.sounds.brock_bat_swing_sounds.PlayRandom();
		}

	}


	private void Anim_OnTeleport()
	{
		ASSETS.sounds.brock_teleport_sounds.PlayRandom();
	}




	private void Anim_OnStep()
	{
		ASSETS.sounds.player_walk_sounds_concrete.PlayRandom();
	}
}
