using UnityEngine;

public class Cone_SFX : MonoBehaviour
{
	private Animations anim;
	private AnimationEvents animEvents;

	private void OnEnable()
	{
		anim = GetComponent<Animations>();
		animEvents = anim.animEvents;
		animEvents.OnAttackHit += Anim_OnAttackHit;
		animEvents.OnStep += Anim_OnStep;
		animEvents.OnRoar += Anim_OnRoar;
		animEvents.OnHitStart += Anim_OnHit;
		animEvents.OnDieStart += Anim_OnDie;
	}

	private void OnDisable()
	{
		animEvents.OnAttackHit -= Anim_OnAttackHit;
		animEvents.OnStep -= Anim_OnStep;
		animEvents.OnRoar -= Anim_OnRoar;
		animEvents.OnHitStart -= Anim_OnHit;
		animEvents.OnDieStart -= Anim_OnDie;
	}

	private void Anim_OnDie()=> SFX.sounds.cone_die_sounds.PlayRandom();
	private void Anim_OnHit()=> SFX.sounds.cone_gethit_sounds.PlayRandom();
	private void Anim_OnRoar()=> SFX.sounds.cone_roar_sounds.PlayRandom();
	private void Anim_OnAttackHit(int attackType)=> SFX.sounds.cone_attack_sounds.PlayRandom();
	private void Anim_OnStep()=> SFX.sounds.cone_walk_sounds.PlayRandom();
	
}