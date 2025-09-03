using UnityEngine;

namespace __SCRIPTS
{
	public class Tmato_SFX : ServiceUser
	{

		private UnitAnimations anim;
		private AnimationEvents animEvents;
		private Life life;
		private JumpAbility jump;
		private Shotgun shotgunAttack;
		private ChainsawAttack chainsawAttack;
		private ThrowMineAttack mineAttack;
		public AudioSource idleSound;
		public AudioSource chainsawAttackIdleSound;

		private void OnEnable()
		{
			anim = GetComponent<UnitAnimations>();
			life = GetComponent<Life>();
			jump = GetComponent<JumpAbility>();
			shotgunAttack = GetComponent<Shotgun>();
			chainsawAttack = GetComponent<ChainsawAttack>();
			mineAttack = GetComponent<ThrowMineAttack>();

			animEvents = anim.animEvents;
			animEvents.OnStep += Anim_OnStep;
			animEvents.OnHitStart += Anim_OnHit;
			animEvents.OnDash += Anim_Dash;
			animEvents.OnReload += Anim_OnReload;
			life.OnWounded += Life_OnWounded;
			life.OnAttackHit += Life_AttackHit;
			life.OnDying += Life_OnDying;
			jump.OnJump += Jump_OnJump;
			jump.OnLand += Jump_OnLand;
			shotgunAttack.OnShotHitTarget += ShotgunAttackOnOnShotHitTarget;
			shotgunAttack.OnShotMissed += ShotgunAttackOnShotMissed;
			shotgunAttack.OnEmpty += ShotgunAttackOnEmpty;

			chainsawAttack.OnStartChainsawing += ChainsawStart;
			chainsawAttack.OnStopChainsawing += ChainsawStop;
			chainsawAttack.OnStartAttacking += ChainsawAttackStart;
			chainsawAttack.OnStopAttacking += ChainsawAttackStop;

			mineAttack.OnThrow += MineAttackOnThrow;
		}

		private void Anim_OnReload()
		{
			sfx.sounds.tmato_reload_sounds.PlayRandomAt(transform.position);
		}

		private void ChainsawStart(Vector2 obj)
		{
			idleSound.Play();
			sfx.sounds.tmato_chainsaw_start_sounds.PlayRandomAt(obj);
		}

		private void ChainsawStop(Vector2 obj)
		{
			idleSound.Stop();
		}

		private void ChainsawAttackStart(Vector2 obj)
		{
			chainsawAttackIdleSound.Play();
			sfx.sounds.tmato_chainsaw_attack_start_sounds.PlayRandomAt(obj);
		}

		private void ChainsawAttackStop(Vector2 obj)
		{
			chainsawAttackIdleSound.Stop();
			sfx.sounds.tmato_chainsaw_attack_stop_sounds.PlayRandomAt(obj);
		}


		private void ShotgunAttackOnEmpty()
		{
			sfx.sounds.ak47_empty_shoot_sounds.PlayRandomAt(transform.position);
		}


		private void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnDash -= Anim_Dash;
			life.OnWounded -= Life_OnWounded;

			life.OnAttackHit -= Life_AttackHit;
			jump.OnJump -= Jump_OnJump;
			jump.OnLand -= Jump_OnLand;
			shotgunAttack.OnShotHitTarget -= ShotgunAttackOnOnShotHitTarget;
			shotgunAttack.OnShotMissed -= ShotgunAttackOnShotMissed;
			chainsawAttack.OnMiss -= TertiaryAttackKnifeOnMiss;
			mineAttack.OnThrow -= MineAttackOnThrow;
		}

		private void ShotgunAttackOnShotMissed(Attack attack, Vector2 hitPositionh)
		{
			sfx.sounds.bean_gun_miss_sounds.PlayRandomAt(hitPositionh);
			sfx.sounds.tmato_shoot_hit_sounds.PlayRandomAt(transform.position);
		}

		private void ShotgunAttackOnOnShotHitTarget(Attack attack, Vector2 hitPosition)
		{
			sfx.sounds.GetBulletHitSounds(attack.DestinationLife.DebrisType).PlayRandomAt(hitPosition);
			sfx.sounds.tmato_shoot_hit_sounds.PlayRandomAt(attack.OriginFloorPoint);
		}

		private void Life_OnDying(Player arg1, Life arg2) => sfx.sounds.player_die_sounds.PlayRandomAt(transform.position);
		private void MineAttackOnThrow(Vector2 vector2, Player player) => sfx.sounds.tmato_mine_throw_sounds.PlayRandomAt(transform.position);
		private void Life_AttackHit(Attack attack, Life hitLife) => sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnMiss() => sfx.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void Jump_OnLand(Vector2 obj) => sfx.sounds.land_sound.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnHit(Vector2 vector2) => sfx.sounds.bean_knifehit_sounds.PlayRandomAt(transform.position);

		private void Jump_OnJump(Vector2 obj) => sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		private void Life_OnWounded(Attack obj) {
			sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
			sfx.sounds.jump_sound.PlayRandomAt(transform.position);
		}
		private void Anim_Dash() => sfx.sounds.tmato_shield_dash_sounds.PlayRandomAt(transform.position);

		private void Anim_OnHit() => sfx.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => sfx.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);

	}
}
