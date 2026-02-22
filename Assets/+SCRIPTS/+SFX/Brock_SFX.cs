using UnityEngine;

namespace __SCRIPTS
{
	public class Brock_SFX : MonoBehaviour
	{
		AnimationEvents animEvents;
		Life life => _life ??= GetComponent<Life>();
		Life _life;
		JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		JumpAbility _jumps;
		BatAttack meleeAttack => _meleeAttack ??= GetComponent<BatAttack>();
		BatAttack _meleeAttack;
		BatJumpAttack meleeJumpAttack => _meleeJumpAttack ??= GetComponent<BatJumpAttack>();
		BatJumpAttack _meleeJumpAttack;
		ChargeAttack chargeAttack => _chargeAttack ??= GetComponent<ChargeAttack>();
		ChargeAttack _chargeAttack;

		KunaiAttack kunaiAttack => _kunaiAttack ??= GetComponent<KunaiAttack>();
		KunaiAttack _kunaiAttack;
		UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		UnitAnimations _anim;

		void OnEnable()
		{
			animEvents = anim.animEvents;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnRoar += Anim_OnRoar;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDieStart += Anim_OnDie;
			animEvents.OnDash += Anim_Dash;
			animEvents.OnTeleport += Anim_Teleport;

			life.OnDead += LifeOnDead;

			jumps.OnJump += JumpsOnJumps;
			jumps.OnLand += JumpsOnLand;

			meleeAttack.OnSwing += MeleeSwingOnSwing;
			meleeAttack.OnHitTarget += MeleeAttackOnHitTarget;
			meleeJumpAttack.OnSwing += MeleeSwingOnSwing;
			meleeJumpAttack.OnHitTarget += MeleeAttackOnHitTarget;

			chargeAttack.OnChargePress += ChargeAttackOnChargeAttackChargePress;
			chargeAttack.OnSpecialAttackHit += ChargeAttackOnSpecialAttackHit;
			chargeAttack.OnAttackHit += ChargeAttackOnAttackHit;
			chargeAttack.OnChargeStop += ChargeAttackChargeStop;

			kunaiAttack.OnThrow += KunaiAttackOnThrow;
		}

		void ChargeAttackOnAttackHit(Attack attack)
		{
			if(attack == null)return;
			if(Services.sfx.sounds.brock_bathit_sounds == null) Debug.Log("Attack destination floor point is null");
			Services.sfx.sounds.brock_bathit_sounds.PlayRandomAt(attack.DestinationFloorPoint);
		}

		void ChargeAttackChargeStop()
		{
			Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
			Services.sfx.sounds.brock_special_attack_sounds.PlayRandomAt(transform.position);
			Services.sfx.StopOngoingSound();
		}

		void OnDisable()
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
			kunaiAttack.OnThrow -= KunaiAttackOnThrow;
			life.OnDead -= LifeOnDead;
			meleeAttack.OnSwing -= MeleeSwingOnSwing;
			meleeAttack.OnHitTarget -= MeleeAttackOnHitTarget;

			meleeJumpAttack.OnSwing -= MeleeSwingOnSwing;
			meleeJumpAttack.OnHitTarget -= MeleeAttackOnHitTarget;
			chargeAttack.OnChargeStop -= ChargeAttackChargeStop;
		}

		void MeleeSwingOnSwing() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		void LifeOnDead(Attack attack) => Services.sfx.sounds.bloodSounds.PlayRandomAt(transform.position);

		void MeleeAttackOnHitTarget(Attack attack) => Services.sfx.sounds.brock_bathit_sounds.PlayRandomAt(attack.DestinationFloorPoint);


		void KunaiAttackOnThrow() =>
			Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		void ChargeAttackOnSpecialAttackHit(Attack attack) => Services.sfx.sounds.brock_homerunhit_sounds.PlayRandomAt(attack.OriginFloorPoint);
		void ChargeAttackOnChargeAttackChargePress() => Services.sfx.StartOngoingSound();
		void JumpsOnLand(Vector2 obj) => Services.sfx.sounds.land_sound.PlayRandomAt(transform.position);
		void JumpsOnJumps(Vector2 obj) => Services.sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		void Anim_Dash() => Services.sfx.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
		void Anim_Teleport() => Services.sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		void Anim_OnDie() => Services.sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);
		void Anim_OnHit() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		void Anim_OnRoar() => Services.sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		void Anim_OnStep() => Services.sfx.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);
	}
}
