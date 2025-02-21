using UnityEngine;

public class Donut_SFX : MonoBehaviour
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

    private void Anim_OnDie() => SFX.sounds.donut_die_sounds.PlayRandomAt(transform.position);
    private void Anim_OnHit() => SFX.sounds.bloodSounds.PlayRandomAt(transform.position);
    private void Anim_OnRoar() => SFX.sounds.donut_roar_sounds.PlayRandomAt(transform.position);
    private void Anim_OnAttackHit(int attackType) => SFX.sounds.donut_hit_sounds.PlayRandomAt(transform.position);
    private void Anim_OnStep() => SFX.sounds.donut_walk_sounds.PlayRandomAt(transform.position);
}
