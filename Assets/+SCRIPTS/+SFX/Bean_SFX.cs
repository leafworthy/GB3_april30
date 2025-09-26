using UnityEngine;

namespace __SCRIPTS
{
	public class Bean_SFX : MonoBehaviour
	{
		private UnitAnimations anim;
		private AnimationEvents animEvents;
		private Life life;
		private JumpAbility jumpAbility;
		private Glock glock => _glock ??= GetComponent<Glock>();
		private Glock _glock;
		private AK47 ak47 => _ak47 ??= GetComponent<AK47>();
		private AK47 _ak47;
		private ReloadAbility reloadAbility;
		private KnifeAttack knifeAttack;
		private NadeAttack nadeAttack;

		private void OnEnable()
		{
			anim = GetComponent<UnitAnimations>();
			life = GetComponent<Life>();

			jumpAbility = GetComponent<JumpAbility>();

			if (jumpAbility != null)
			{
				jumpAbility.OnJump += JumpAbilityOnJumpAbility;
				jumpAbility.OnLand += JumpAbilityOnLand;
			}

			reloadAbility = GetComponent<ReloadAbility>();
			knifeAttack = GetComponent<KnifeAttack>();
			nadeAttack = GetComponent<NadeAttack>();

			animEvents = anim.animEvents;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDash += Anim_Dash;
			animEvents.OnTeleport += Anim_Teleport;

			life.OnAttackHit += Life_AttackHit;
			life.OnDying += Life_OnDying;

			glock.OnShotHitTarget += GunAttackAkGlockOnOnShotHitTarget;
			glock.OnShotMissed += GunAttackAkGlockOnShotMissed;
			glock.OnEmpty += GunAttackAkGlockOnEmpty;

			ak47.OnShotHitTarget += GunAttackAkGlockOnOnShotHitTarget;
			ak47.OnShotMissed += GunAttackAkGlockOnShotMissed;
			ak47.OnEmpty += GunAttackAkGlockOnEmpty;

			reloadAbility.OnReload += GunAttackAkGlockOnReload;
			knifeAttack.OnMiss += KnifeAttackOnMiss;
			knifeAttack.OnHit += KnifeAttackOnHit;
			nadeAttack.OnThrow += NadeAttackOnThrow;
		}


		private void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnDash -= Anim_Dash;
			animEvents.OnTeleport -= Anim_Teleport;

			life.OnAttackHit -= Life_AttackHit;
			if (jumpAbility != null)
			{
				jumpAbility.OnJump -= JumpAbilityOnJumpAbility;
				jumpAbility.OnLand -= JumpAbilityOnLand;
			}


			life.OnDying -= Life_OnDying;

			glock.OnShotHitTarget -= GunAttackAkGlockOnOnShotHitTarget;
			glock.OnShotMissed -= GunAttackAkGlockOnShotMissed;
			glock.OnEmpty -= GunAttackAkGlockOnEmpty;

			ak47.OnShotHitTarget -= GunAttackAkGlockOnOnShotHitTarget;
			ak47.OnShotMissed -= GunAttackAkGlockOnShotMissed;
			ak47.OnEmpty -= GunAttackAkGlockOnEmpty;

			reloadAbility.OnReload -= GunAttackAkGlockOnReload;
			knifeAttack.OnMiss -= KnifeAttackOnMiss;
			knifeAttack.OnHit -= KnifeAttackOnHit;
			nadeAttack.OnThrow -= NadeAttackOnThrow;
		}

		private void GunAttackAkGlockOnShotMissed(Attack attack)
		{
			Services.sfx.sounds.bean_gun_miss_sounds.PlayRandomAt(attack.DestinationFloorPoint);
			Services.sfx.sounds.ak47_shoot_sounds.PlayRandomAt(transform.position);
		}

		private void GunAttackAkGlockOnOnShotHitTarget(Attack attack)
		{
			Services.sfx.sounds.GetBulletHitSounds(attack.DestinationLife.DebrisType).PlayRandomAt(attack.DestinationFloorPoint);
			Services.sfx.sounds.ak47_shoot_sounds.PlayRandomAt(attack.OriginFloorPoint);
		}

		private void Life_OnDying(Attack attack) => Services.sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);

		private void Life_AttackHit(Attack attack) => Services.sfx.sounds.bloodSounds.PlayRandomAt(transform.position);

		private void Life_OnWounded(Attack obj)
		{
			Services.sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
			Services.sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		}

		private void NadeAttackOnThrow(Vector2 v, Vector2 v2, float f, Life p) => Services.sfx.sounds.bean_nade_throw_sounds.PlayRandomAt(transform.position);

		private void GunAttackAkGlockOnEmpty() => Services.sfx.sounds.ak47_empty_shoot_sounds.PlayRandomAt(transform.position);
		private void KnifeAttackOnMiss() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void GunAttackAkGlockOnReload() => Services.sfx.sounds.bean_reload_sounds.PlayRandomAt(transform.position);
		private void JumpAbilityOnLand(Vector2 obj) => Services.sfx.sounds.land_sound.PlayRandomAt(transform.position);
		private void KnifeAttackOnHit(Vector2 vector2) => Services.sfx.sounds.bean_knifehit_sounds.PlayRandomAt(transform.position);

		private void JumpAbilityOnJumpAbility(Vector2 obj) => Services.sfx.sounds.jump_sound.PlayRandomAt(transform.position);

		private void Anim_Dash() => Services.sfx.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
		private void Anim_Teleport() => Services.sfx.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit() => Services.sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => Services.sfx.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);
	}
}
