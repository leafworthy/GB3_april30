using UnityEngine;

namespace __SCRIPTS
{
    public class Donut_SFX : MonoBehaviour
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

        private void Anim_OnDie() => Services.sfx.sounds.donut_die_sounds.PlayRandomAt(transform.position);
        private void Anim_OnHit() => Services.sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
        private void Anim_OnRoar() => Services.sfx.sounds.dig_sounds.PlayRandomAt(transform.position);
        private void Anim_OnAttackHit(int attackType) => Services.sfx.sounds.donut_hit_sounds.PlayRandomAt(transform.position);
        private void Anim_OnStep() => Services.sfx.sounds.donut_walk_sounds.PlayRandomAt(transform.position);
    }
}
