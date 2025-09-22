using UnityEngine;

namespace __SCRIPTS
{
	public class Brock_SFX : MonoBehaviour
	{
		private AnimationEvents animEvents;
		private Life life => _life ??= GetComponent<Life>();
		private Life _life;
		private JumpAbility jumps  => _jumps ??=GetComponent<JumpAbility>();
		private JumpAbility _jumps;
		private BatAttack meleeAttack => _meleeAttack ??= GetComponent<BatAttack>();
		private BatAttack _meleeAttack;
		private BatJumpAttack meleeJumpAttack => _meleeJumpAttack ??= GetComponent<BatJumpAttack>();
		private BatJumpAttack _meleeJumpAttack;
		private ChargeAttack chargeAttack  => _chargeAttack ??= GetComponent<ChargeAttack>();
		private ChargeAttack _chargeAttack;

		private KunaiAttack kunaiAttack  => _kunaiAttack ??= GetComponent<KunaiAttack>();
		private KunaiAttack _kunaiAttack;
		private UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		private UnitAnimations _anim;

		private void OnEnable()
		{

			animEvents = anim.animEvents;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnRoar += Anim_OnRoar;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDieStart += Anim_OnDie;
			animEvents.OnDash += Anim_Dash;
			animEvents.OnTeleport += Anim_Teleport;

			life.OnDying += Life_OnDying;

			jumps.OnJump += JumpsOnJumps;
			jumps.OnLand += JumpsOnLand;

			meleeAttack.OnSwing += MeleeSwingOnSwing;
			meleeAttack.OnHitTarget += MeleeAttackOnHitTarget;
			meleeJumpAttack .OnSwing += MeleeSwingOnSwing;
			meleeJumpAttack.OnHitTarget += MeleeAttackOnHitTarget;

			chargeAttack.OnChargePress += ChargeAttackOnChargeAttackChargePress;
			chargeAttack.OnSpecialAttackHit += ChargeAttackOnSpecialAttackHit;
			chargeAttack.OnChargeStop += ChargeAttackChargeStop;

			kunaiAttack.OnThrow += KunaiAttackOnThrow;
		}



		private void ChargeAttackChargeStop()
		{
			Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
			Services.sfx.sounds.brock_special_attack_sounds.PlayRandomAt(transform.position);
			Services.sfx.StopOngoingSound();
		}

		private void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnRoar -= Anim_OnRoar;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnDieStart -= Anim_OnDie;
			animEvents.OnDash -= Anim_Dash;
			animEvents.OnTeleport -= Anim_Teleport;
			jumps.OnJump -= JumpsOnJumps;
			jumps.OnLand -= JumpsOnLand;
			chargeAttack.OnChargePress -= ChargeAttackOnChargeAttackChargePress;
			chargeAttack.OnSpecialAttackHit -= ChargeAttackOnSpecialAttackHit;
			chargeAttack.OnAttackHit -= ChargeAttack_OnAttackHit;
			kunaiAttack.OnThrow -= KunaiAttackOnThrow;
			life.OnDying -= Life_OnDying;
			meleeAttack.OnSwing -= MeleeSwingOnSwing;
			meleeAttack.OnHitTarget -= MeleeAttackOnHitTarget;

			meleeJumpAttack.OnSwing -= MeleeSwingOnSwing;
			meleeJumpAttack.OnHitTarget -= MeleeAttackOnHitTarget;
			chargeAttack.OnChargeStop -= ChargeAttackChargeStop;


		}

		private void MeleeSwingOnSwing() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		private void Life_OnDying(Attack attack) => Services.sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);

		private void MeleeAttackOnHitTarget(Vector2 vector2) => Services.sfx.sounds.brock_bathit_sounds.PlayRandomAt(vector2);
		private void ChargeAttack_OnAttackHit() => Services.sfx.sounds.brock_bathit_sounds.PlayRandomAt(transform.position);

		private void KunaiAttackOnThrow() =>
			Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		private void ChargeAttackOnSpecialAttackHit() => Services.sfx.sounds.brock_homerunhit_sounds.PlayRandomAt(transform.position);
		private void ChargeAttackOnChargeAttackChargePress() => Services.sfx.StartOngoingSound();
		private void JumpsOnLand(Vector2 obj) => Services.sfx.sounds.land_sound.PlayRandomAt(transform.position);
		private void JumpsOnJumps(Vector2 obj) => Services.sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		private void Anim_Dash() => Services.sfx.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
		private void Anim_Teleport() => Services.sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnDie() => Services.sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void Anim_OnRoar() => Services.sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => Services.sfx.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);
	}
}
