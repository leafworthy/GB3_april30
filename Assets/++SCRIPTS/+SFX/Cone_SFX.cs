using UnityEngine;

namespace __SCRIPTS
{
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
			animEvents.OnRecovered += Anim_OnSplat;
		}

		private void Anim_OnSplat()
		{ 
			SFX.I.sounds.cone_splat_sounds.PlayRandomAt(transform.position);
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

		private void Anim_OnDie()=> SFX.I.sounds.cone_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit()=> SFX.I.sounds.cone_gethit_sounds.PlayRandomAt(transform.position);
		private void Anim_OnRoar()=> SFX.I.sounds.cone_roar_sounds.PlayRandomAt(transform.position);
		private void Anim_OnAttackHit(int attackType)=> SFX.I.sounds.cone_attack_sounds.PlayRandomAt(transform.position);
		private void Anim_OnStep()=> SFX.I.sounds.cone_walk_sounds.PlayRandomAt(transform.position);
	
	}
}