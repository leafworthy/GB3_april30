using UnityEngine;

namespace __SCRIPTS
{
	public class Toast_SFX : MonoBehaviour
	{
		private AnimationEvents animEvents;
		private UnitAnimations anim;

		private void OnEnable()
		{
			anim = GetComponent<UnitAnimations>();
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

		private void Anim_OnDieStart() => Services.sfx.sounds.toast_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnGetHit() => Services.sfx.sounds.toast_gethit_sounds.PlayRandomAt(transform.position);

		private void Anim_OnAttackHit(int attackType) => Services.sfx.sounds.toast_attack_sounds.PlayRandomAt(transform.position);

		private void Anim_OnStep() => Services.sfx.sounds.toast_walk_sounds.PlayRandomAt(transform.position);

		private void Anim_OnRoar() => Services.sfx.sounds.toast_roar_sounds.PlayRandomAt(transform.position);
	}
}
