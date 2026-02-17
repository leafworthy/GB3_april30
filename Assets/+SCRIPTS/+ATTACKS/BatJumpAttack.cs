using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class BatJumpAttack : Ability
	{
		enum state
		{
			not,
			jumpUpAttack,
			jumpDownAttack,
			landingAttack
		}

		state currentState;

		void SetState(state newState)
		{
			currentState = newState;
		}

		public event Action OnSwing;
		public event Action<Attack> OnHitTarget;
		public AnimationClip JumpAttackUpAnimationClip;
		public AnimationClip JumpAttackFallingAnimationClip;
		public AnimationClip LandAttackAnimationClip;

		JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		JumpAbility _jumps;
		AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
		AimAbility _aimAbility;

		float GetAttackDamage() => player.spawnedPlayerStats.Stats.Damage(1);
		public override string AbilityName => "Bat-Air-Attack";

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && jumps.IsInAir;

		protected override void DoAbility()
		{
			StartJumpAttack();
		}

		void Update()
		{
			if (currentState == state.jumpDownAttack) body.BottomFaceDirection(aimAbility.AimDir.x >= 0);
		}

		void StartJumpAttack()
		{
			OnSwing?.Invoke();

			PlayAnimationClip(JumpAttackUpAnimationClip);
			SetState(state.jumpUpAttack);

			body.BottomFaceDirection(aimAbility.AimDir.x >= 0);

			jumps.DoubleJump(jumps.IsFalling ? 1 : .5f);

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
			if (currentState == state.landingAttack) StopAbility();
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			player.Controller.Attack1RightTrigger.OnPress += Player_AttackPress;
			SetState(state.not);
		}

		void LandAttackHit()
		{
			RegularAttackHit();
		}

		void Player_AttackPress(NewControlButton obj)
		{
			if (currentState != state.not) return;
			TryToActivate();
		}

		public override void StopAbility()
		{
			SetState(state.not);
			jumps.OnLand -= LandWithAttack;
			jumps.OnFalling -= FallWithAttack;
			base.StopAbility();
		}

		void FallWithAttack()
		{
			SetState(state.jumpDownAttack);
			PlayAnimationClip(JumpAttackFallingAnimationClip);
			jumps.OnFalling -= FallWithAttack;
		}

		void LandWithAttack(Vector2 obj)
		{
			SetState(state.landingAttack);
			RegularAttackHit();
			Invoke(nameof(LandAttackHit), LandAttackAnimationClip.length / 4);
			PlayAnimationClip(LandAttackAnimationClip);
			jumps.OnLand -= LandWithAttack;
		}

		void RegularAttackHit()
		{
			var hits = MyAttackUtilities.CircleCastForXClosestTargets(offence, offence.stats.Stats.Range(1));
			if (hits == null) return;
			foreach (var hit in hits)
			{
				var attack = MyAttackUtilities.HitTarget(offence, hit, GetAttackDamage());
				OnHitTarget?.Invoke(attack);
			}
		}
	}
}
