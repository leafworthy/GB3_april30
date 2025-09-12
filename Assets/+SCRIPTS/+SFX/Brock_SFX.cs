using UnityEngine;

namespace __SCRIPTS
{
	public class Brock_SFX : MonoBehaviour
	{
		private UnitAnimations anim;
		private AnimationEvents animEvents;
		private Life life;
		private SimpleJumpAbility simpleJump;
		private TertiaryAttack_BatAttack meleeAttack;
		private SecondaryAttack_ChargeAttack secondaryAttackChargeAttack;

		private PrimaryAttack_Kunai primaryAttackKunai;

		private void OnEnable()
		{
			anim = GetComponent<UnitAnimations>();
			life = GetComponent<Life>();
			simpleJump = GetComponent<SimpleJumpAbility>();
			meleeAttack = GetComponent<TertiaryAttack_BatAttack>();
			secondaryAttackChargeAttack = GetComponent<SecondaryAttack_ChargeAttack>();
			primaryAttackKunai = GetComponent<PrimaryAttack_Kunai>();

			animEvents = anim.animEvents;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnRoar += Anim_OnRoar;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDieStart += Anim_OnDie;
			animEvents.OnDash += Anim_Dash;
			animEvents.OnTeleport += Anim_Teleport;
			life.OnDying += Life_OnDying;
			simpleJump.OnJump += SimpleJumpOnSimpleJump;
			simpleJump.OnLand += SimpleJumpOnLand;
			meleeAttack.OnSwing += MeleeAttackOnSwing;
			meleeAttack.OnHit += MeleeAttackOnHit;
			secondaryAttackChargeAttack.OnChargePress += SecondaryAttackChargeAttackOnSecondaryAttackChargePress;
			secondaryAttackChargeAttack.OnSpecialAttackHit += SecondaryAttackChargeAttackOnSpecialAttackHit;
			secondaryAttackChargeAttack.OnChargeStop += SecondaryAttackChargeStop;
			primaryAttackKunai.OnThrow += PrimaryAttackKunaiOnThrow;
		}



		private void SecondaryAttackChargeStop()
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
			simpleJump.OnJump -= SimpleJumpOnSimpleJump;
			simpleJump.OnLand -= SimpleJumpOnLand;
			secondaryAttackChargeAttack.OnChargePress -= SecondaryAttackChargeAttackOnSecondaryAttackChargePress;
			secondaryAttackChargeAttack.OnSpecialAttackHit -= SecondaryAttackChargeAttackOnSpecialAttackHit;
			secondaryAttackChargeAttack.OnAttackHit -= ChargeAttack_OnAttackHit;
			primaryAttackKunai.OnThrow -= PrimaryAttackKunaiOnThrow;

		}

		private void MeleeAttackOnSwing() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		private void Life_OnDying(Attack attack) => Services.sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);

		private void MeleeAttackOnHit(Vector2 vector2) => Services.sfx.sounds.brock_bathit_sounds.PlayRandomAt(vector2);
		private void ChargeAttack_OnAttackHit() => Services.sfx.sounds.brock_bathit_sounds.PlayRandomAt(transform.position);

		private void PrimaryAttackKunaiOnThrow(Vector3 vector3, Vector3 vector4, float arg3, Life arg4, bool arg5) =>
			Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		private void SecondaryAttackChargeAttackOnSpecialAttackHit() => Services.sfx.sounds.brock_homerunhit_sounds.PlayRandomAt(transform.position);
		private void SecondaryAttackChargeAttackOnSecondaryAttackChargePress() => Services.sfx.StartOngoingSound();
		private void SimpleJumpOnLand(Vector2 obj) => Services.sfx.sounds.land_sound.PlayRandomAt(transform.position);
		private void SimpleJumpOnSimpleJump(Vector2 obj) => Services.sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		private void Life_OnWounded(Attack obj)  {
			Services.sfx.sounds.brock_gethit_sounds.PlayRandomAt(transform.position);
			Services.sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		}
		private void Anim_Dash() => Services.sfx.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
		private void Anim_Teleport() => Services.sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnDie() => Services.sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void Anim_OnRoar() => Services.sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => Services.sfx.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);
	}
}
