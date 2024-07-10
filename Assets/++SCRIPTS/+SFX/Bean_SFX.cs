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
		animEvents = anim.animEvents;
		animEvents.OnAttackHit += Anim_OnAttackHit;
		animEvents.OnStep += Anim_OnStep;
		animEvents.OnRoar += Anim_OnRoar;
		animEvents.OnHitStart += Anim_OnHit;
		animEvents.OnDieStart += Anim_OnDie;
		animEvents.OnDash += Anim_Dash;
		animEvents.OnTeleport += Anim_Teleport;
		life = GetComponent<Life>();
		life.OnDamaged += Life_OnDamaged;
		life.OnAttackHit += Life_AttackHit;
		jump = GetComponent<JumpAbility>();
		jump.OnJump += Jump_OnJump;
		SFX.sounds.ak47_shoot_sounds.PlayRandom();
		SFX.sounds.bean_gun_miss_sounds.PlayRandom();
		gunAttacks = GetComponent<GunAttacks>();
		gunAttacks.OnShoot += GunAttacks_OnShoot;
		gunAttacks.OnReload += GunAttacks_OnReload;
		knifeAttacks = GetComponent<KnifeAttacks>();
		knifeAttacks.OnMiss += KnifeAttacks_OnMiss;
		knifeAttacks.OnHit += KnifeAttacks_OnHit;
		nadeAbility = GetComponent<NadeAbility>();
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
		jump.OnJump -= Jump_OnJump;
		gunAttacks.OnShoot -= GunAttacks_OnShoot;
		gunAttacks.OnReload -= GunAttacks_OnReload;
		knifeAttacks.OnMiss -= KnifeAttacks_OnMiss;
		knifeAttacks.OnHit -= KnifeAttacks_OnHit;
		nadeAbility.OnThrow -= NadeAbilityOnThrow;
	}

	private void NadeAbilityOnThrow(Vector2 vector2, Vector2 vector3, float arg3, Player arg4) => SFX.sounds.bean_nade_throw_sounds.PlayRandom();
	private void Life_AttackHit(Attack attack, Life hitLife) => SFX.sounds.GetHitSounds((int)hitLife.DebreeType).PlayRandom();
	private void KnifeAttacks_OnMiss() => SFX.sounds.brock_bat_swing_sounds.PlayRandom();
	private void GunAttacks_OnReload() => SFX.sounds.bean_reload_sounds.PlayRandom();

	private void GunAttacks_OnShoot(Attack arg1, Vector2 arg2)
	{
		SFX.sounds.ak47_shoot_sounds.PlayRandom();
		SFX.sounds.bean_gun_miss_sounds.PlayRandom();
	}

	private void KnifeAttacks_OnHit(Vector2 vector2) => SFX.sounds.bean_knifehit_sounds.PlayRandom();

	private void Jump_OnJump(Vector2 obj) => SFX.sounds.jump_sound.PlayRandom();
	private void Life_OnDamaged(Attack obj) => SFX.sounds.jump_sound.PlayRandom();
	private void Anim_Dash() => SFX.sounds.bean_roll_sounds.PlayRandom();
	private void Anim_Teleport() => SFX.sounds.brock_teleport_sounds.PlayRandom();
	private void Anim_OnDie() => SFX.sounds.cone_die_sounds.PlayRandom();
	private void Anim_OnHit() => SFX.sounds.cone_gethit_sounds.PlayRandom();
	private void Anim_OnRoar() => SFX.sounds.cone_roar_sounds.PlayRandom();
	private void Anim_OnAttackHit(int attackType) => SFX.sounds.cone_attack_sounds.PlayRandom();
	private void Anim_OnStep() => SFX.sounds.player_walk_sounds_concrete.PlayRandom();
}