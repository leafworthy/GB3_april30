using UnityEngine;

namespace __SCRIPTS
{
	public class Cone_SFX : MonoBehaviour
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
			Services.sfx.sounds.cone_splat_sounds.PlayRandomAt(transform.position);
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

		private void Anim_OnDie()=> Services.sfx.sounds.cone_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit()=> Services.sfx.sounds.cone_gethit_sounds.PlayRandomAt(transform.position);
		private void Anim_OnRoar()=> Services.sfx.sounds.cone_roar_sounds.PlayRandomAt(transform.position);
		private void Anim_OnAttackHit(int attackType)=> Services.sfx.sounds.cone_attack_sounds.PlayRandomAt(transform.position);
		private void Anim_OnStep()=> Services.sfx.sounds.cone_walk_sounds.PlayRandomAt(transform.position);

	}
}
