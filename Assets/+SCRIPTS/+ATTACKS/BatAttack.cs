using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class BatAttack : Ability
	{
		public override string AbilityName => "Bat-Attack";
		public event Action OnSwing;
		public event Action<Vector2> OnHitTarget;

		public AnimationClip Attack1AnimationClip;
		public AnimationClip Attack2AnimationClip;
		public AnimationClip Attack3AnimationClip;

		private bool isPressingAttack;
		private float extraPush = 1.75f;
		private JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		private JumpAbility _jumps;
		private AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
		private AimAbility _aimAbility;

		private int currentAttackIndex;
		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private const float Attack1Damage = 50;
		private const float Attack2Damage = 60;
		private const float Attack3Damage = 100;
		private const float Attack4Damage = 200;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && !jumps.IsInAir;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.Attack1RightTrigger.OnPress += Player_AttackPress;
			player.Controller.Attack3Circle.OnPress += Player_AttackPress;
			player.Controller.Attack1RightTrigger.OnRelease += Player_AttackRelease;
			player.Controller.Attack3Circle.OnRelease += Player_AttackRelease;
		}

		private void OnDestroy()
		{
			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= Player_AttackPress;
			player.Controller.Attack1RightTrigger.OnRelease -= Player_AttackRelease;
		}

		private void Player_AttackPress(NewControlButton newControlButton)
		{
			isPressingAttack = true;
			Do();
		}

		protected override void DoAbility()
		{
			StartRandomAttack();
		}

		private void Player_AttackRelease(NewControlButton obj)
		{
			isPressingAttack = false;
		}

		private void RegularAttackHit(int attackType)
		{
			var hits = AttackUtilities.CircleCastForXClosestTargets(life, life.TertiaryAttackRange);
			if (hits == null) return;
			foreach (var hit in hits)
			{
				AttackUtilities.HitTarget(life, hit, GetAttackDamage(attackType));
				OnHitTarget?.Invoke(hit.gameObject.transform.position);
			}
		}

		private float GetAttackDamage(int attackType)
		{
			var extraDamageFactor = player.spawnedPlayerDefence.ExtraDamageFactor;
			return attackType switch
			       {
				       1 => Attack1Damage + Attack1Damage * extraDamageFactor,
				       2 => Attack2Damage + Attack2Damage * extraDamageFactor,
				       3 => Attack3Damage + Attack3Damage * extraDamageFactor,
				       _ => Attack4Damage
			       };
		}

		private void StartRandomAttack()
		{
			OnSwing?.Invoke();
			moveAbility.SetCanMove(false);
			body.BottomFaceDirection(aimAbility.AimDir.x >= 0);
			currentAttackIndex = Random.Range(1, 4);
			var attackAnimation = currentAttackIndex switch
			                      {
				                      1 => Attack1AnimationClip,
				                      2 => Attack2AnimationClip,
				                      _ => Attack3AnimationClip
			                      };
			PlayAnimationClip(attackAnimation);
			Invoke(nameof(CurrentAttackHit), attackAnimation.length / 4);
		}

		public override void Stop()
		{
			moveAbility.SetCanMove(true);
			base.Stop();
		}

		protected override void AnimationComplete()
		{
			Stop();
			if (isPressingAttack) Player_AttackPress(null);
		}

		private void CurrentAttackHit()
		{
			RegularAttackHit(currentAttackIndex);
		}
	}
}
