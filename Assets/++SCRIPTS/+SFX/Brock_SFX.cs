using __SCRIPTS._ABILITIES;
using __SCRIPTS._ATTACKS;
using __SCRIPTS._COMMON;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS._SFX
{
	public class Brock_SFX : MonoBehaviour
	{
		private Animations anim;
		private AnimationEvents animEvents;
		private Life life;
		private JumpAbility jump;
		private BatAttacks meleeAttack;
		private ChargeAttack chargeAttack;

		private KunaiAttacks kunaiAttacks;

		private void OnEnable()
		{
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			jump = GetComponent<JumpAbility>();
			meleeAttack = GetComponent<BatAttacks>();
			chargeAttack = GetComponent<ChargeAttack>();
			kunaiAttacks = GetComponent<KunaiAttacks>();
			
			animEvents = anim.animEvents;
			animEvents.OnAttackHit += Anim_OnAttackHit;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnRoar += Anim_OnRoar;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDieStart += Anim_OnDie;
			animEvents.OnDash += Anim_Dash;
			animEvents.OnTeleport += Anim_Teleport;
			life.OnDamaged += Life_OnDamaged;
			jump.OnJump += Jump_OnJump;
			jump.OnLand += Jump_OnLand;
			meleeAttack.OnHit += MeleeAttackOnHit;
			meleeAttack.OnAttackStop += MeleeAttackOnStop;
			chargeAttack.OnChargePress += ChargeAttack_OnChargePress;
			chargeAttack.OnSpecialAttackHit += ChargeAttack_OnSpecialAttackHit;
			kunaiAttacks.OnThrow += KunaiAttacks_OnThrow;
		}

		private void MeleeAttackOnStop()
		{
			SFX.StopSpecialSound();
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
			jump.OnLand -= Jump_OnLand;
			chargeAttack.OnChargePress -= ChargeAttack_OnChargePress;
			chargeAttack.OnSpecialAttackHit -= ChargeAttack_OnSpecialAttackHit;
			chargeAttack.OnAttackHit -= ChargeAttack_OnAttackHit;
			kunaiAttacks.OnThrow -= KunaiAttacks_OnThrow;
			 
		}

		private void MeleeAttackOnHit() => SFX.sounds.brock_bathit_sounds.PlayRandom();
		private void ChargeAttack_OnAttackHit() => SFX.sounds.brock_bathit_sounds.PlayRandom();

		private void KunaiAttacks_OnThrow(Vector3 vector3, Vector3 vector4, float arg3, Life arg4, bool arg5) =>
			SFX.sounds.brock_bat_swing_sounds.PlayRandom();

		private void ChargeAttack_OnSpecialAttackHit() => SFX.sounds.brock_homerunhit_sounds.PlayRandom();
		private void ChargeAttack_OnChargePress() => SFX.sounds.brock_charge_sounds.PlayRandom();
		private void Jump_OnLand(Vector2 obj) => SFX.sounds.land_sound.PlayRandom();
		private void Jump_OnJump(Vector2 obj) => SFX.sounds.jump_sound.PlayRandom();
		private void Life_OnDamaged(Attack obj) => SFX.sounds.jump_sound.PlayRandom();
		private void Anim_Dash() => SFX.sounds.bean_roll_sounds.PlayRandom();
		private void Anim_Teleport() => SFX.sounds.brock_teleport_sounds.PlayRandom();
		private void Anim_OnDie() => SFX.sounds.cone_die_sounds.PlayRandom();
		private void Anim_OnHit() => SFX.sounds.cone_gethit_sounds.PlayRandom();
		private void Anim_OnRoar() => SFX.sounds.cone_roar_sounds.PlayRandom();
		private void Anim_OnAttackHit(int attackType) => SFX.sounds.cone_attack_sounds.PlayRandom();
		private void Anim_OnStep() => SFX.sounds.cone_walk_sounds.PlayRandom();
	}
}