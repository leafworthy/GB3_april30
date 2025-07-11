using UnityEngine;

namespace __SCRIPTS
{
	public class Toast_SFX : ServiceUser
	{
		private AnimationEvents animEvents;
		private Animations anim;

		private void OnEnable()
		{
			anim = GetComponent<Animations>();
			animEvents = anim.animEvents;
			animEvents.OnAttackHit += Anim_OnAttackHit;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnRoar += Anim_OnRoar;
			animEvents.OnHitStart += Anim_OnGetHit;
			animEvents.OnDieStart += Anim_OnDieStart;
		}

		private void OnDisable()
		{
			animEvents.OnAttackHit -= Anim_OnAttackHit;
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnRoar -= Anim_OnRoar;
			animEvents.OnHitStart -= Anim_OnGetHit;
			animEvents.OnDieStart -= Anim_OnDieStart;
		}

		private void Anim_OnDieStart() => sfx.sounds.toast_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnGetHit() => sfx.sounds.toast_gethit_sounds.PlayRandomAt(transform.position);

		private void Anim_OnAttackHit(int attackType) => sfx.sounds.toast_attack_sounds.PlayRandomAt(transform.position);

		private void Anim_OnStep() => sfx.sounds.toast_walk_sounds.PlayRandomAt(transform.position);

		private void Anim_OnRoar() => sfx.sounds.toast_roar_sounds.PlayRandomAt(transform.position);
	}
}
