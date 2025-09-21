using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class BatAttack : Ability
	{
		public override string AbilityName => "Bat-Attack";
		public event Action OnAttack;
		public event Action<Vector2> OnHitTarget;

		public AnimationClip Attack1AnimationClip;
		public AnimationClip Attack2AnimationClip;
		public AnimationClip Attack3AnimationClip;
		public AnimationClip JumpAttackAnimationClip;

		private bool isPressingAttack;
		private bool isAttacking;
		private float extraPush = 1.75f;
		private CharacterJumpAbility jumps => _jumps ??= GetComponent<CharacterJumpAbility>();
		private CharacterJumpAbility _jumps;
		private AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
		private AimAbility _aimAbility;
		private int currentAttackIndex;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.Attack3Circle.OnPress += Player_AttackPress;
			player.Controller.Attack3Circle.OnRelease += Player_AttackRelease;
		}

		private void OnDestroy()
		{
			if (player == null) return;
			player.Controller.Attack3Circle.OnPress -= Player_AttackPress;
			player.Controller.Attack3Circle.OnRelease -= Player_AttackRelease;
		}

		private void Player_AttackPress(NewControlButton newControlButton)
		{
			isPressingAttack = true;
			Do();
		}

		protected override void DoAbility()
		{
			StartAttack();
		}

		private void StartAttack()
		{
			if (isAttacking) return;
			isAttacking = true;
			if (!jumps.IsJumping)
			{
				StartJumpAttack();
				return;
			}

			StartRandomAttack();
		}

		private void Player_AttackRelease(NewControlButton obj)
		{
			isPressingAttack = false;
		}

		protected override void AnimationComplete()
		{
			isAttacking = false;
			if (isPressingAttack) Player_AttackPress(null);
			else base.AnimationComplete();
		}

		private void RegularAttackHit(int attackType)
		{
			var hits = AttackUtilities.CircleCastForXClosestTargets(life, life.TertiaryAttackRange,2);
			foreach (var hit in hits)
			{
				var hitPoint = AttackUtilities.RaycastToObject(hit, life.EnemyLayer);
				if (hitPoint.fraction <=0) continue;
				AttackUtilities.HitTarget(life, hit, hitPoint.point, GetAttackDamage(attackType));
				OnHitTarget?.Invoke(hit.gameObject.transform.position);
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
			var extraDamageFactor = player.spawnedPlayerDefence.ExtraDamageFactor;
			return attackType switch
			       {
				       1 => 50 + 50 * extraDamageFactor,
				       2 => 60 + 60 * extraDamageFactor,
				       3 => 100 + 100 * extraDamageFactor,
				       _ => 0
			       };
		}

		private void StartRandomAttack()
		{
			OnAttack?.Invoke();
			body.BottomFaceDirection(aimAbility.AimDir.x >= 0);
			currentAttackIndex = Random.Range(1, 3);
			var attackAnimation = currentAttackIndex switch
			                      {
				                      1 => Attack1AnimationClip,
				                      2 => Attack2AnimationClip,
				                      _ => Attack3AnimationClip
			                      };
			PlayAnimationClip(attackAnimation);
			Invoke(nameof(CurrentAttackHit), attackAnimation.length / 2);
		}

		private void StartJumpAttack()
		{
			OnAttack?.Invoke();
			PlayAnimationClip(JumpAttackAnimationClip);
			Invoke(nameof(CurrentAttackHit), JumpAttackAnimationClip.length / 2);
		}

		private void CurrentAttackHit()
		{
			RegularAttackHit(currentAttackIndex);
		}
	}
}
