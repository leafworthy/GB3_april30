using UnityEngine;

namespace __SCRIPTS
{
	public class Brock_SFX : MonoBehaviour
	{
		private Animations anim;
		private AnimationEvents animEvents;
		private Life life;
		private JumpAbility jump;
		private TertiaryAttack_BatAttack meleeAttack;
		private SecondaryAttack_ChargeAttack secondaryAttackChargeAttack;

		private PrimaryAttack_Kunai primaryAttackKunai;

		private void OnEnable()
		{
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			jump = GetComponent<JumpAbility>();
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
			life.OnWounded += Life_OnWounded;
			life.OnDying += Life_OnDying;
			jump.OnJump += Jump_OnJump;
			jump.OnLand += Jump_OnLand;
			meleeAttack.OnSwing += MeleeAttackOnSwing;
			meleeAttack.OnHit += MeleeAttackOnHit;
			secondaryAttackChargeAttack.OnChargePress += SecondaryAttackChargeAttackOnSecondaryAttackChargePress;
			secondaryAttackChargeAttack.OnSpecialAttackHit += SecondaryAttackChargeAttackOnSpecialAttackHit;
			secondaryAttackChargeAttack.OnChargeStop += SecondaryAttackChargeStop;
			primaryAttackKunai.OnThrow += PrimaryAttackKunaiOnThrow;
		}

	
	
		private void SecondaryAttackChargeStop()
		{
			SFX.I.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
			SFX.I.sounds.brock_special_attack_sounds.PlayRandomAt(transform.position);
			SFX.I.StopOngoingSound();
		}

		private void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnRoar -= Anim_OnRoar;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnDieStart -= Anim_OnDie;
			animEvents.OnDash -= Anim_Dash;
			animEvents.OnTeleport -= Anim_Teleport;
			life.OnWounded -= Life_OnWounded;
			jump.OnJump -= Jump_OnJump;
			jump.OnLand -= Jump_OnLand;
			secondaryAttackChargeAttack.OnChargePress -= SecondaryAttackChargeAttackOnSecondaryAttackChargePress;
			secondaryAttackChargeAttack.OnSpecialAttackHit -= SecondaryAttackChargeAttackOnSpecialAttackHit;
			secondaryAttackChargeAttack.OnAttackHit -= ChargeAttack_OnAttackHit;
			primaryAttackKunai.OnThrow -= PrimaryAttackKunaiOnThrow;
			 
		}

		private void MeleeAttackOnSwing() => SFX.I.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		private void Life_OnDying(Player arg1, Life arg2) => SFX.I.sounds.player_die_sounds.PlayRandomAt(transform.position);

		private void MeleeAttackOnHit(Vector2 vector2) => SFX.I.sounds.brock_bathit_sounds.PlayRandomAt(vector2);
		private void ChargeAttack_OnAttackHit() => SFX.I.sounds.brock_bathit_sounds.PlayRandomAt(transform.position);

		private void PrimaryAttackKunaiOnThrow(Vector3 vector3, Vector3 vector4, float arg3, Life arg4, bool arg5) =>
			SFX.I.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);

		private void SecondaryAttackChargeAttackOnSpecialAttackHit() => SFX.I.sounds.brock_homerunhit_sounds.PlayRandomAt(transform.position);
		private void SecondaryAttackChargeAttackOnSecondaryAttackChargePress() => SFX.I.StartOngoingSound();
		private void Jump_OnLand(Vector2 obj) => SFX.I.sounds.land_sound.PlayRandomAt(transform.position);
		private void Jump_OnJump(Vector2 obj) => SFX.I.sounds.jump_sound.PlayRandomAt(transform.position);
		private void Life_OnWounded(Attack obj)  {
			SFX.I.sounds.brock_gethit_sounds.PlayRandomAt(transform.position);
			SFX.I.sounds.jump_sound.PlayRandomAt(transform.position);
		}
		private void Anim_Dash() => SFX.I.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
		private void Anim_Teleport() => SFX.I.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnDie() => SFX.I.sounds.player_die_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit() => SFX.I.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void Anim_OnRoar() => SFX.I.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => SFX.I.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);
	}
}