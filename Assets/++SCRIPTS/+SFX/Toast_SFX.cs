using __SCRIPTS._ABILITIES;
using __SCRIPTS._COMMON;
using UnityEngine;

namespace __SCRIPTS._SFX
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

		private void Anim_OnDieStart() => SFX.sounds.toast_die_sounds.PlayRandom();
		private void Anim_OnGetHit() => SFX.sounds.toast_gethit_sounds.PlayRandom();

		private void Anim_OnAttackHit(int attackType) => SFX.sounds.toast_attack_sounds.PlayRandom();

		private void Anim_OnStep() => SFX.sounds.toast_walk_sounds.PlayRandom();

		private void Anim_OnRoar() => SFX.sounds.toast_roar_sounds.PlayRandom();
	}
}