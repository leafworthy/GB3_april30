using UnityEngine;

namespace __SCRIPTS
{
	public class Tmato_SFX : MonoBehaviour
	{
		UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		UnitAnimations _anim;
		AnimationEvents animEvents => _animEvents ??= anim.animEvents;
		AnimationEvents _animEvents;
		Life life => _life ??= GetComponent<Life>();
		Life _life;
		JumpAbility jump => _jump ??= GetComponent<JumpAbility>();
		JumpAbility _jump;
		Shotgun shotgunAttack => _shotgunAttack ??= GetComponent<Shotgun>();
		Shotgun _shotgunAttack;
		ChainsawAttack chainsawAttack => _chainsawAttack ??= GetComponent<ChainsawAttack>();
		ChainsawAttack _chainsawAttack;
		ThrowMineAttack mineAttack => _mineAttack ??= GetComponent<ThrowMineAttack>();
		ThrowMineAttack _mineAttack;
		public AudioSource idleSound;
		public AudioSource chainsawAttackIdleSound;

		ShieldDashAbility shieldDash => _shieldDash ??= GetComponent<ShieldDashAbility>();
		ShieldDashAbility _shieldDash;

		void OnEnable()
		{
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnReload += Anim_OnReload;

			shieldDash.OnShieldDash += Anim_Dash;
			life.OnAttackHit += Life_AttackHit;
			life.OnDead += LifeOnDead;
			jump.OnJump += JumpOnJump;
			jump.OnLand += JumpOnLand;
			shotgunAttack.OnShotHitTarget += ShotgunAttackOnOnShotHitTarget;
			shotgunAttack.OnShotMissed += ShotgunAttackOnShotMissed;
			shotgunAttack.OnEmpty += ShotgunAttackOnEmpty;
			chainsawAttack.OnStartChainsawing += ChainsawStart;
			chainsawAttack.OnReload += ChainsawReload;
			chainsawAttack.OnStopChainsawing += ChainsawStop;
			chainsawAttack.OnStartAttacking += ChainsawAttackStart;
			chainsawAttack.OnStopAttacking += ChainsawAttackStop;
			mineAttack.OnThrow += MineAttackOnThrow;
		}

		void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnReload -= Anim_OnReload;

			shieldDash.OnShieldDash -= Anim_Dash;
			life.OnAttackHit -= Life_AttackHit;
			jump.OnJump -= JumpOnJump;
			jump.OnLand -= JumpOnLand;
			shotgunAttack.OnShotHitTarget -= ShotgunAttackOnOnShotHitTarget;
			shotgunAttack.OnShotMissed -= ShotgunAttackOnShotMissed;
			shotgunAttack.OnEmpty -= ShotgunAttackOnEmpty;
			chainsawAttack.OnStartChainsawing -= ChainsawStart;
			chainsawAttack.OnReload -= ChainsawReload;
			chainsawAttack.OnStopChainsawing -= ChainsawStop;
			chainsawAttack.OnStartAttacking -= ChainsawAttackStart;
			chainsawAttack.OnStopAttacking -= ChainsawAttackStop;
			mineAttack.OnThrow -= MineAttackOnThrow;
		}

		void Anim_OnReload()
		{
			Services.sfx.sounds.tmato_reload_sounds.PlayRandomAt(transform.position);
		}

		void ChainsawStart(Vector2 obj)
		{
			idleSound.Play();
			Services.sfx.sounds.tmato_chainsaw_start_sounds.PlayRandomAt(transform.position);
		}

		void ChainsawReload(Vector2 obj)
		{
			Services.sfx.sounds.tmato_chainsaw_start_sounds.PlayRandomAt(obj);
		}

		void ChainsawStop(Vector2 obj)
		{
			idleSound.Stop();
		}

		void ChainsawAttackStart(Vector2 obj)
		{
			chainsawAttackIdleSound.Play();
			Services.sfx.sounds.tmato_chainsaw_attack_start_sounds.PlayRandomAt(obj);
		}

		void ChainsawAttackStop(Vector2 obj)
		{
			chainsawAttackIdleSound.Stop();
			Services.sfx.sounds.tmato_chainsaw_attack_stop_sounds.PlayRandomAt(obj);
		}

		void ShotgunAttackOnEmpty()
		{
			Services.sfx.sounds.ak47_empty_shoot_sounds.PlayRandomAt(transform.position);
		}



		void ShotgunAttackOnShotMissed(Attack attack)
		{
			Services.sfx.sounds.bean_gun_miss_sounds.PlayRandomAt(attack.DestinationFloorPoint);
			Services.sfx.sounds.tmato_shoot_hit_sounds.PlayRandomAt(transform.position);
		}

		void ShotgunAttackOnOnShotHitTarget(Attack attack)
		{
			Services.sfx.sounds.GetBulletHitSounds(attack.DestinationLife.DebrisType).PlayRandomAt(attack.DestinationFloorPoint);
			Services.sfx.sounds.tmato_shoot_hit_sounds.PlayRandomAt(attack.OriginFloorPoint);
		}

		void LifeOnDead(Attack attack)  {
			idleSound.Stop();
			Services.sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);
		}
		void MineAttackOnThrow(Vector2 vector2, Player player) => Services.sfx.sounds.tmato_mine_throw_sounds.PlayRandomAt(transform.position);
		void Life_AttackHit(Attack attack) => Services.sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
		void TertiaryAttackKnifeOnMiss() => Services.sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		void JumpOnLand(Vector2 obj) => Services.sfx.sounds.land_sound.PlayRandomAt(transform.position);
		void TertiaryAttackKnifeOnHit(Vector2 vector2) => Services.sfx.sounds.bean_knifehit_sounds.PlayRandomAt(transform.position);

		void JumpOnJump(Vector2 obj) => Services.sfx.sounds.jump_sound.PlayRandomAt(transform.position);

		void Anim_Dash() => Services.sfx.sounds.tmato_shield_dash_sounds.PlayRandomAt(transform.position);

		void Anim_OnHit() => Services.sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
		void Anim_OnStep() => Services.sfx.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);
	}
}
