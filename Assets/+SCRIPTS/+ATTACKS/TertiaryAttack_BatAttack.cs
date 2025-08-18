using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class TertiaryAttack_BatAttack : Attacks
	{
		private AnimationEvents animEvents;
		private JumpAbility jumps;
		public override string VerbName => "Bat-Attack";
		private bool isCharging;

		private bool isFullyCharged;
		private bool isPressingAttack;
		private bool isAttacking;
		private Player owner;
		private float extraPush = 1.75f;
		private Vector2 moveDir;
		public event Action OnSwing;
		public event Action<Vector2> OnHit;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			owner = life.player;
			jumps = GetComponent<JumpAbility>();

			if (owner == null) return;
			owner.Controller.OnAttack3_Pressed += Player_AttackPress;
			owner.Controller.OnAttack3_Released += Player_AttackRelease;
			owner.Controller.OnMoveAxis_Change += Player_MoveInDirection;
			animEvents = anim.animEvents;
			animEvents.OnAttackHit += Anim_AttackHit;
			animEvents.OnAttackStop += Anim_AttackStop;
		}

		private void OnDisable()
		{
			if (owner == null) return;
			owner.Controller.OnAttack3_Pressed -= Player_AttackPress;
			owner.Controller.OnAttack3_Released -= Player_AttackRelease;
			owner.Controller.OnMoveAxis_Change -= Player_MoveInDirection;
			if (animEvents == null) return;
			animEvents.OnAttackHit -= Anim_AttackHit;
			animEvents.OnAttackStop -= Anim_AttackStop;
		}

		private void Player_MoveInDirection(Vector2 dir)
		{
			moveDir = dir;
		}

		private void Player_AttackRelease(NewControlButton obj)
		{
			isPressingAttack = false;
		}

		private void Anim_AttackStop(int attackType)
		{
			isAttacking = false;
			body.doableArms.Stop(this);
			body.doableLegs.Stop(this);

			if (attackType == 0)
			{
				anim.ResetTrigger(UnitAnimations.ChargeAttackTrigger);
				anim.ResetTrigger(UnitAnimations.ChargeStartTrigger);
			}

			if (isPressingAttack) Player_AttackPress(null);
		}

		private void Anim_AttackHit(int attackType)
		{
			if (attackType != 5)
				RegularAttackHit(attackType);
		}

		private void RegularAttackHit(int attackType)
		{
			var circleCast = Physics2D.OverlapCircleAll(transform.position, GetHitRange(attackType), assetManager.LevelAssets.EnemyLayer);
			var closest2 = circleCast.OrderBy(item => Vector2.Distance(item.gameObject.transform.position, transform.position)).Take(2);
			foreach (var col in closest2)
			{
				if (col == null) return;
				var _life = col.gameObject.GetComponent<Life>();
				if (!_life.IsEnemyOf(attacker) || _life.cantDie) return;
				if (attacker.IsPlayer && _life.IsObstacle) return;
				HitTarget(GetAttackDamage(attackType), _life, extraPush);
				OnHit?.Invoke(col.gameObject.transform.position);
			}
		}

		private float GetHitRange(int attackType)
		{
			if (attackType == 5)
				return life.TertiaryAttackRange * 3;
			return life.TertiaryAttackRange;
		}

		private float GetAttackDamage(int attackType)
		{
			var extraDamageFactor = owner.spawnedPlayerLife.ExtraMaxDamageFactor;
			return attackType switch
			       {
				       1 => 50 + 50 * extraDamageFactor,
				       2 => 50 + 50 * extraDamageFactor,
				       3 => 100 + 100 * extraDamageFactor,
				       4 => 100 + 100 * extraDamageFactor,
				       _ => 0
			       };
		}

		private void Player_AttackPress(NewControlButton newControlButton)
		{
			isPressingAttack = true;
			if (isAttacking) return;
			if (jumps.IsJumping)
			{
				StartJumpAttack();
				return;
			}

			body.BottomFaceDirection(moveDir.x >= 0);
			StartRandomAttack();
		}

		private void StartRandomAttack()
		{

			if (isAttacking) return;
			isAttacking = true;

			var randomAttack = Random.Range(1, 3);
			switch (randomAttack)
			{
				case 1:
					anim.SetTrigger(UnitAnimations.Attack1Trigger);
					break;
				case 2:
					anim.SetTrigger(UnitAnimations.Attack2Trigger);
					break;
				default:
					anim.SetTrigger(UnitAnimations.Attack3Trigger);
					break;
			}

			OnSwing?.Invoke();
		}

		private void StartJumpAttack()
		{
			if (isAttacking) return;
			OnSwing?.Invoke();
			isAttacking = true;
			anim.SetTrigger(UnitAnimations.JumpAttackTrigger);
		}
	}
}
