using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class BatJumpAttack : Ability
	{
		private enum state
		{
			not,
			jumpUpAttack,
			jumpDownAttack,
			landingAttack
		}

		private state currentState;

		private void SetState(state newState)
		{
			currentState = newState;
		}
		public event Action OnSwing;
		public event Action<Vector2> OnHitTarget;
		public AnimationClip JumpAttackUpAnimationClip;
		public AnimationClip JumpAttackFallingAnimationClip;
		public AnimationClip LandAttackAnimationClip;

		private JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		private JumpAbility _jumps;
		private AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
		private AimAbility _aimAbility;

		private float GetAttackDamage() => player.spawnedPlayerStats.PrimaryAttackDamageWithExtra;
		public override string AbilityName => "Bat-Air-Attack";

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && jumps.IsInAir;

		protected override void DoAbility()
		{
			StartJumpAttack();
		}

		private void Update()
		{
			if (currentState == state.jumpDownAttack)
			{
				body.BottomFaceDirection(aimAbility.AimDir.x >= 0);
			}
		}

		private void StartJumpAttack()
		{
			OnSwing?.Invoke();

			PlayAnimationClip(JumpAttackUpAnimationClip);
			SetState(state.jumpUpAttack);

			body.BottomFaceDirection(aimAbility.AimDir.x >= 0);

			jumps.DoubleJump(jumps.IsFalling ? 1 : .5f);

			Debug.Log("jump attack started");

			jumps.OnLand += LandWithAttack;
			jumps.OnFalling += FallWithAttack;
		}

		public void OnDisable()
		{
			if (player == null) return;
			jumps.OnLand -= LandWithAttack;
			jumps.OnFalling -= FallWithAttack;
			player.Controller.Attack1RightTrigger.OnPress -= Player_AttackPress;
		}

		protected override void AnimationComplete()
		{
			if (currentState == state.landingAttack)
			{
				Stop();
			}
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.Attack1RightTrigger.OnPress += Player_AttackPress;
			SetState(state.not);
		}

		private void LandAttackHit()
		{
			RegularAttackHit();
		}

		private void Player_AttackPress(NewControlButton obj)
		{
			if (currentState != state.not) return;
			Do();
		}

		public override void Stop()
		{
			SetState(state.not);
			jumps.OnLand -= LandWithAttack;
			jumps.OnFalling -= FallWithAttack;
			base.Stop();
		}

		private void FallWithAttack()
		{
			SetState(state.jumpDownAttack);
			PlayAnimationClip(JumpAttackFallingAnimationClip);
			jumps.OnFalling -= FallWithAttack;
		}

		private void LandWithAttack(Vector2 obj)
		{
			SetState(state.landingAttack);
			RegularAttackHit();
			Invoke(nameof(LandAttackHit), LandAttackAnimationClip.length / 4);
			PlayAnimationClip(LandAttackAnimationClip);
			jumps.OnLand -= LandWithAttack;
		}

		private void RegularAttackHit()
		{
			var hits = AttackUtilities.CircleCastForXClosestTargets(life, life.TertiaryAttackRange);
			if (hits == null) return;
			foreach (var hit in hits)
			{
				AttackUtilities.HitTarget(life, hit, GetAttackDamage());
				OnHitTarget?.Invoke(hit.transform.position);
			}
		}
	}
}
