using UnityEngine;

namespace __SCRIPTS
{
	public class Bean_SFX : MonoBehaviour
	{
	
		private Animations anim;
		private AnimationEvents animEvents;
		private Life life;
		private JumpAbility jump;
		private GunAttack gunAttack;
		private TertiaryAttack_Knife tertiaryAttackKnife;
		private NadeAttack nadeAttack;

		private void OnEnable()
		{
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			jump = GetComponent<JumpAbility>();
			gunAttack = GetComponent<GunAttack>();
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
			jump.OnJump += Jump_OnJump;
			jump.OnLand += Jump_OnLand;
			gunAttack.OnShotHitTarget += GunAttackOnOnShotHitTarget;
			gunAttack.OnShotMissed += GunAttackOnShotMissed;
			gunAttack.OnReload += GunAttackOnReload;
			gunAttack.OnEmpty += GunAttackOnEmpty;
			tertiaryAttackKnife.OnMiss += TertiaryAttackKnifeOnMiss;
			tertiaryAttackKnife.OnHit += TertiaryAttackKnifeOnHit;
			nadeAttack.OnThrow += NadeAttackOnThrow;
		}

		private void GunAttackOnEmpty()
		{
			SFX.I.sounds.ak47_empty_shoot_sounds.PlayRandomAt(transform.position);
		}


		private void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
			animEvents.OnHitStart -= Anim_OnHit;
			animEvents.OnDash -= Anim_Dash;
			animEvents.OnTeleport -= Anim_Teleport;
			life.OnWounded -= Life_OnWounded;
		
			life.OnAttackHit -= Life_AttackHit;
			jump.OnJump -= Jump_OnJump;
			jump.OnLand -= Jump_OnLand;
			gunAttack.OnShotHitTarget -= GunAttackOnOnShotHitTarget;
			gunAttack.OnShotMissed -= GunAttackOnShotMissed;
			gunAttack.OnReload -= GunAttackOnReload;
			tertiaryAttackKnife.OnMiss -= TertiaryAttackKnifeOnMiss;
			tertiaryAttackKnife.OnHit -= TertiaryAttackKnifeOnHit;
			nadeAttack.OnThrow -= NadeAttackOnThrow;
		}

		private void GunAttackOnShotMissed(Attack attack, Vector2 hitPositionh)
		{
			SFX.I.sounds.bean_gun_miss_sounds.PlayRandomAt(hitPositionh);
			SFX.I.sounds.ak47_shoot_sounds.PlayRandomAt(transform.position);
		}

		private void GunAttackOnOnShotHitTarget(Attack attack, Vector2 hitPosition)
		{
			SFX.I.sounds.GetBulletHitSounds(attack.DestinationLife.DebrisType).PlayRandomAt(hitPosition);
			SFX.I.sounds.ak47_shoot_sounds.PlayRandomAt(attack.OriginFloorPoint);
		}

		private void Life_OnDying(Player arg1, Life arg2) => SFX.I.sounds.player_die_sounds.PlayRandomAt(transform.position);
		private void NadeAttackOnThrow(Vector2 vector2, Vector2 vector3, float arg3, Player arg4) => SFX.I.sounds.bean_nade_throw_sounds.PlayRandomAt(transform.position);
		private void Life_AttackHit(Attack attack, Life hitLife) => SFX.I.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnMiss() => SFX.I.sounds.brock_bat_swing_sounds.PlayRandomAt(transform.position);
		private void GunAttackOnReload() => SFX.I.sounds.bean_reload_sounds.PlayRandomAt(transform.position);
		private void Jump_OnLand(Vector2 obj) => SFX.I.sounds.land_sound.PlayRandomAt(transform.position);
		private void TertiaryAttackKnifeOnHit(Vector2 vector2) => SFX.I.sounds.bean_knifehit_sounds.PlayRandomAt(transform.position);

		private void Jump_OnJump(Vector2 obj) => SFX.I.sounds.jump_sound.PlayRandomAt(transform.position);
		private void Life_OnWounded(Attack obj) {
			SFX.I.sounds.bloodSounds.PlayRandomAt(transform.position);
			SFX.I.sounds.jump_sound.PlayRandomAt(transform.position);
		}
		private void Anim_Dash() => SFX.I.sounds.bean_roll_sounds.PlayRandomAt(transform.position);
		private void Anim_Teleport() => SFX.I.sounds.brock_teleport_sounds.PlayRandomAt(transform.position);
		private void Anim_OnHit() => SFX.I.sounds.bloodSounds.PlayRandomAt(transform.position);
		private void Anim_OnStep() => SFX.I.sounds.player_walk_sounds_concrete.PlayRandomAt(transform.position);

	}
}