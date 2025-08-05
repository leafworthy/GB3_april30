using UnityEngine;

namespace __SCRIPTS
{
	public class Cone_SFX : ServiceUser
	{
		private UnitAnimations anim;
		private AnimationEvents animEvents;


		private void OnEnable()
		{
			anim = GetComponent<UnitAnimations>();
			animEvents = anim.animEvents;
			animEvents.OnAttackHit += Anim_OnAttackHit;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnRoar += Anim_OnRoar;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDieStart += Anim_OnDie;
			animEvents.OnRecovered += Anim_OnSplat;
		}

		private void Anim_OnSplat()
		{
			sfx.sounds.cone_splat_sounds.PlayRandomAt(transform.position);
		}

		private void OnDisable()
		{
			animEvents.OnAttackHit -= Anim_OnAttackHit;
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnRoar -= Anim_OnRoar;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnDieStart -= Anim_OnDie;
			animEvents.OnRecovered -= Anim_OnSplat;

		}

		private void Anim_OnDie()=> sfx.sounds.cone_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit()=> sfx.sounds.cone_gethit_sounds.PlayRandomAt(transform.position);
		private void Anim_OnRoar()=> sfx.sounds.cone_roar_sounds.PlayRandomAt(transform.position);
		private void Anim_OnAttackHit(int attackType)=> sfx.sounds.cone_attack_sounds.PlayRandomAt(transform.position);
		private void Anim_OnStep()=> sfx.sounds.cone_walk_sounds.PlayRandomAt(transform.position);

	}
}
