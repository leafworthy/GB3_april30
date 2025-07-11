using UnityEngine;

namespace __SCRIPTS
{
    public class Donut_SFX : ServiceUser
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
             if(animEvents == null) return;
            animEvents.OnAttackHit -= Anim_OnAttackHit;
            animEvents.OnStep -= Anim_OnStep;
            animEvents.OnRoar -= Anim_OnRoar;
            animEvents.OnHitStart -= Anim_OnHit;
            animEvents.OnDieStart -= Anim_OnDie;
        }

        private void Anim_OnDie() => sfx.sounds.donut_die_sounds.PlayRandomAt(transform.position);
        private void Anim_OnHit() => sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
        private void Anim_OnRoar() => sfx.sounds.donut_roar_sounds.PlayRandomAt(transform.position);
        private void Anim_OnAttackHit(int attackType) => sfx.sounds.donut_hit_sounds.PlayRandomAt(transform.position);
        private void Anim_OnStep() => sfx.sounds.donut_walk_sounds.PlayRandomAt(transform.position);
    }
}
