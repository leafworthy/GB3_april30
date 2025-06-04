using UnityEngine;

namespace GangstaBean.Audio
{
	public class Tmato_SFX : MonoBehaviour
	{

		private Animations anim;
		private AnimationEvents animEvents;
		private Life life;
		private JumpAbility jump;
		private GunAttack_Shotgun shotgunAttack;
		private ChainsawAttack chainsawAttack;
		private ThrowMineAttack mineAttack;
		public AudioSource idleSound;
		public AudioSource chainsawAttackIdleSound;

		private void OnEnable()
		{
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			jump = GetComponent<JumpAbility>();
			shotgunAttack = GetComponent<GunAttack_Shotgun>();
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
			SFX.I.sounds.tmato_reload_sounds.PlayRandomAt(transform.position);
		}

		private void ChainsawStart(Vector2 obj)
		{
			idleSound.Play();
			SFX.I.sounds.tmato_chainsaw_start_sounds.PlayRandomAt(obj);
		}

		private void ChainsawStop(Vector2 obj)
		{
			idleSound.Stop();
		}

		private void ChainsawAttackStart(Vector2 obj)
		{
			chainsawAttackIdleSound.Play();
			SFX.I.sounds.tmato_chainsaw_attack_start_sounds.PlayRandomAt(obj);
		}

		private void ChainsawAttackStop(Vector2 obj)
		{
			chainsawAttackIdleSound.Stop();
			SFX.I.sounds.tmato_chainsaw_attack_stop_sounds.PlayRandomAt(obj);
		}


		private void ShotgunAttackOnEmpty()
		{
			SFX.I.sounds.ak47_empty_shoot_sounds.PlayRandomAt(transform.position);
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
			SFX.I.sounds.bean_gun_miss_sounds.PlayRandomAt(hitPositionh);
			SFX.I.sounds.tmato_shoot_hit_sounds.PlayRandomAt(transform.position);
		}

		private void ShotgunAttackOnOnShotHitTarget(Attack attack, Vector2 hitPosition)
		{
			SFX.I.sounds.GetBulletHitSounds(attack.DestinationLife.DebrisType).PlayRandomAt(hitPosition);
			SFX.I.sounds.tmato_shoot_hit_sounds.PlayRandomAt(attack.OriginFloorPoint);
		}

		private void Life_OnDying(Player arg1, Life arg2) => SFX.I.sounds.player_die_sounds.PlayRandomAt(transform.position);
		private void MineAttackOnThrow(Vector2 vector2, Player player) => SFX.I.sounds.tmato_mine_throw_sounds.PlayRandomAt(transform.position);
		private void Life_AttackHit(Attack attack, Life hitLife) => SFX.I.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnMiss() => SFX.I.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void Jump_OnLand(Vector2 obj) => SFX.I.sounds.land_sound.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnHit(Vector2 vector2) => SFX.I.sounds.bean_knifehit_sounds.PlayRandomAt(transform.position);

		private void Jump_OnJump(Vector2 obj) => SFX.I.sounds.jump_sound.PlayRandomAt(transform.position);
		private void Life_OnWounded(Attack obj) {
			SFX.I.sounds.bloodSounds.PlayRandomAt(transform.position);
			SFX.I.sounds.jump_sound.PlayRandomAt(transform.position);
		}
		private void Anim_Dash() => SFX.I.sounds.tmato_shield_dash_sounds.PlayRandomAt(transform.position);

		private void Anim_OnHit() => SFX.I.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => SFX.I.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);

	}
}
