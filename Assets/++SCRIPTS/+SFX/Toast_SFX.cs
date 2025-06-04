using UnityEngine;

namespace GangstaBean.Audio
{
	public class Toast_SFX : MonoBehaviour
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

		private void Anim_OnDieStart() => SFX.I.sounds.toast_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnGetHit() => SFX.I.sounds.toast_gethit_sounds.PlayRandomAt(transform.position);

		private void Anim_OnAttackHit(int attackType) => SFX.I.sounds.toast_attack_sounds.PlayRandomAt(transform.position);

		private void Anim_OnStep() => SFX.I.sounds.toast_walk_sounds.PlayRandomAt(transform.position);

		private void Anim_OnRoar() => SFX.I.sounds.toast_roar_sounds.PlayRandomAt(transform.position);
	}
}