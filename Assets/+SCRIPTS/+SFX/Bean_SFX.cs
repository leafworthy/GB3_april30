using JetBrains.Annotations;
using UnityEngine;

namespace __SCRIPTS
{
	public class Bean_SFX : ServiceUser
	{

		private Animations anim;
		private AnimationEvents animEvents;
		private Life life;
		private JumpAbility jump;
		private GunAttack_AK_Glock gunAttackAkGlock;
		private TertiaryAttack_Knife tertiaryAttackKnife;
		private NadeAttack nadeAttack;

		private void OnEnable()
		{
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();

			jump = GetComponent<JumpAbility>();

			if (jump != null)
			{
				jump.OnJump += Jump_OnJump;
				jump.OnLand += Jump_OnLand;
			}
			gunAttackAkGlock = GetComponent<GunAttack_AK_Glock>();
			tertiaryAttackKnife = GetComponent<TertiaryAttack_Knife>();
			nadeAttack = GetComponent<NadeAttack>();

			animEvents = anim.animEvents;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDash += Anim_Dash;
			animEvents.OnTeleport += Anim_Teleport;

			life.OnWounded += Life_OnWounded;
			life.OnAttackHit += Life_AttackHit;
			life.OnDying += Life_OnDying;

			gunAttackAkGlock.OnShotHitTarget += GunAttackAkGlockOnOnShotHitTarget;
			gunAttackAkGlock.OnShotMissed += GunAttackAkGlockOnShotMissed;
			gunAttackAkGlock.OnReload += GunAttackAkGlockOnReload;
			gunAttackAkGlock.OnEmpty += GunAttackAkGlockOnEmpty;
			tertiaryAttackKnife.OnMiss += TertiaryAttackKnifeOnMiss;
			tertiaryAttackKnife.OnHit += TertiaryAttackKnifeOnHit;
			nadeAttack.OnThrow += NadeAttackOnThrow;
		}

		private void GunAttackAkGlockOnEmpty()
		{
			sfx.sounds.ak47_empty_shoot_sounds.PlayRandomAt(transform.position);
		}


		private void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnDash -= Anim_Dash;
			animEvents.OnTeleport -= Anim_Teleport;
			life.OnWounded -= Life_OnWounded;

			life.OnAttackHit -= Life_AttackHit;
			if (jump != null)
			{
				jump.OnJump -= Jump_OnJump;
				jump.OnLand -= Jump_OnLand;
			}

			gunAttackAkGlock.OnShotHitTarget -= GunAttackAkGlockOnOnShotHitTarget;
			gunAttackAkGlock.OnShotMissed -= GunAttackAkGlockOnShotMissed;
			gunAttackAkGlock.OnReload -= GunAttackAkGlockOnReload;
			tertiaryAttackKnife.OnMiss -= TertiaryAttackKnifeOnMiss;
			tertiaryAttackKnife.OnHit -= TertiaryAttackKnifeOnHit;
			nadeAttack.OnThrow -= NadeAttackOnThrow;
		}

		private void GunAttackAkGlockOnShotMissed(Attack attack, Vector2 hitPositionh)
		{
			sfx.sounds.bean_gun_miss_sounds.PlayRandomAt(hitPositionh);
			sfx.sounds.ak47_shoot_sounds.PlayRandomAt(transform.position);
		}

		private void GunAttackAkGlockOnOnShotHitTarget(Attack attack, Vector2 hitPosition)
		{
			sfx.sounds.GetBulletHitSounds(attack.DestinationLife.DebrisType).PlayRandomAt(hitPosition);
			sfx.sounds.ak47_shoot_sounds.PlayRandomAt(attack.OriginFloorPoint);
		}

		private void Life_OnDying(Player arg1, Life arg2) => sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);
		private void NadeAttackOnThrow(Vector2 vector2, Vector2 vector3, float arg3, Player arg4) => sfx.sounds.bean_nade_throw_sounds.PlayRandomAt(transform.position);
		private void Life_AttackHit(Attack attack, Life hitLife) => sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnMiss() => sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void GunAttackAkGlockOnReload() => sfx.sounds.bean_reload_sounds.PlayRandomAt(transform.position);
		private void Jump_OnLand(Vector2 obj) => sfx.sounds.land_sound.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnHit(Vector2 vector2) => sfx.sounds.bean_knifehit_sounds.PlayRandomAt(transform.position);

		private void Jump_OnJump(Vector2 obj) => sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		private void Life_OnWounded(Attack obj) {
			sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
			sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		}
		private void Anim_Dash() => sfx.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
		private void Anim_Teleport() => sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit() => sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => sfx.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);

	}
}
