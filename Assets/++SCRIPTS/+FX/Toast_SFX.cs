using UnityEngine;

public class Toast_SFX : MonoBehaviour
{
	private AnimationEvents animEvents;
	private Animations anim;
	private RecycleGameObject recycle;

	private void Awake()
	{
	
		Activate();
	}

	private void Activate()
	{
		anim = GetComponent<Animations>();
		animEvents = anim.animEvents;
		animEvents.OnAttackHit += Anim_OnAttackHit;
		animEvents.OnStep += Anim_OnStep;
		animEvents.OnRoar += Anim_OnRoar;
		animEvents.OnHitStart += Anim_OnGetHit;
		animEvents.OnDieStart += Anim_OnDieStart;
	}

	private void CleanUp()
	{
		animEvents.OnAttackHit -= Anim_OnAttackHit;
		animEvents.OnStep -= Anim_OnStep;
		animEvents.OnRoar -= Anim_OnRoar;
		animEvents.OnHitStart -= Anim_OnGetHit;
		animEvents.OnDieStart -= Anim_OnDieStart;
	}


	private void Anim_OnDieStart()
	{
		ASSETS.sounds.toast_die_sounds.PlayRandom();
	}

	private void Anim_OnGetHit()
	{
		ASSETS.sounds.toast_gethit_sounds.PlayRandom();
	}

	private void Anim_OnAttackHit(int attackType)
	{
		ASSETS.sounds.toast_attack_sounds.PlayRandom();
	}


	private void Anim_OnStep()
	{
		ASSETS.sounds.toast_walk_sounds.PlayRandom();
	}

	private void Anim_OnRoar()
	{
		ASSETS.sounds.toast_roar_sounds.PlayRandom();
	}
}
