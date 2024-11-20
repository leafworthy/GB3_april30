using UnityEngine;

public class Bean_SFX : MonoBehaviour
{
	private Animations anim;
	private AnimationEvents animEvents;
	private Life life;
	private JumpAbility jump;
	private GunAttacks gunAttacks;
	private KnifeAttacks knifeAttacks;
	private NadeAbility nadeAbility;

	private void OnEnable()
	{
		anim = GetComponent<Animations>();
		life = GetComponent<Life>();
		jump = GetComponent<JumpAbility>();
		gunAttacks = GetComponent<GunAttacks>();
		knifeAttacks = GetComponent<KnifeAttacks>();
		nadeAbility = GetComponent<NadeAbility>();
		
		animEvents = anim.animEvents;
		animEvents.OnAttackHit += Anim_OnAttackHit;
		animEvents.OnStep += Anim_OnStep;
		animEvents.OnRoar += Anim_OnRoar;
		animEvents.OnHitStart += Anim_OnHit;
		animEvents.OnDieStart += Anim_OnDie;
		animEvents.OnDash += Anim_Dash;
		animEvents.OnTeleport += Anim_Teleport;
		life.OnDamaged += Life_OnDamaged;
		life.OnAttackHit += Life_AttackHit;
		jump.OnJump += Jump_OnJump;
		gunAttacks.OnShotFired += GunAttacksOnShotFired;
		gunAttacks.OnShotMissed += GunAttacksOnShotMissed;
		gunAttacks.OnReload += GunAttacks_OnReload;
		knifeAttacks.OnMiss += KnifeAttacks_OnMiss;
		knifeAttacks.OnHit += KnifeAttacks_OnHit;
		nadeAbility.OnThrow += NadeAbilityOnThrow;
	}

	

	private void OnDisable()
	{
		animEvents.OnAttackHit -= Anim_OnAttackHit;
		animEvents.OnStep -= Anim_OnStep;
		animEvents.OnRoar -= Anim_OnRoar;
		animEvents.OnHitStart -= Anim_OnHit;
		animEvents.OnDieStart -= Anim_OnDie;
		animEvents.OnDash -= Anim_Dash;
		animEvents.OnTeleport -= Anim_Teleport;
		life.OnDamaged -= Life_OnDamaged;
		life.OnAttackHit -= Life_AttackHit;
		jump.OnJump -= Jump_OnJump;
		gunAttacks.OnShotFired -= GunAttacksOnShotFired;
		gunAttacks.OnShotMissed -= GunAttacksOnShotMissed;
		gunAttacks.OnReload -= GunAttacks_OnReload;
		knifeAttacks.OnMiss -= KnifeAttacks_OnMiss;
		knifeAttacks.OnHit -= KnifeAttacks_OnHit;
		nadeAbility.OnThrow -= NadeAbilityOnThrow;
	}

	private void GunAttacksOnShotMissed(Attack arg1, Vector3 arg2) => SFX.sounds.bean_gun_miss_sounds.PlayRandomAt(transform.position);

	private void GunAttacksOnShotFired(Attack arg1, Vector2 arg2) => SFX.sounds.ak47_shoot_sounds.PlayRandomAt(transform.position);
	private void NadeAbilityOnThrow(Vector2 vector2, Vector2 vector3, float arg3, Player arg4) => SFX.sounds.bean_nade_throw_sounds.PlayRandomAt(transform.position);
	private void Life_AttackHit(Attack attack, Life hitLife) => SFX.sounds.GetHitSounds((int)hitLife.DebreeType).PlayRandomAt(attack.DestinationFloorPoint);
	private void KnifeAttacks_OnMiss() => SFX.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
	private void GunAttacks_OnReload() => SFX.sounds.bean_reload_sounds.PlayRandomAt(transform.position);

	private void KnifeAttacks_OnHit(Vector2 vector2) => SFX.sounds.bean_knifehit_sounds.PlayRandomAt(transform.position);

	private void Jump_OnJump(Vector2 obj) => SFX.sounds.jump_sound.PlayRandomAt(transform.position);
	private void Life_OnDamaged(Attack obj) => SFX.sounds.jump_sound.PlayRandomAt(transform.position);
	private void Anim_Dash() => SFX.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
	private void Anim_Teleport() => SFX.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
	private void Anim_OnDie() => SFX.sounds.cone_die_sounds.PlayRandomAt(transform.position);
	private void Anim_OnHit() => SFX.sounds.cone_gethit_sounds.PlayRandomAt(transform.position);
	private void Anim_OnRoar() => SFX.sounds.cone_roar_sounds.PlayRandomAt(transform.position);
	private void Anim_OnAttackHit(int attackType) => SFX.sounds.cone_attack_sounds.PlayRandomAt(transform.position);
	private void Anim_OnStep() => SFX.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);
}