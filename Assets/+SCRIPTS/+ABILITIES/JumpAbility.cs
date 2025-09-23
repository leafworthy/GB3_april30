using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{

	public class JumpAbility : Ability
	{
		private float verticalVelocity;

		public AnimationClip jumpingAnimationClip;
		public AnimationClip fallingAnimationClip;
		public AnimationClip landingAnimationClip;
		public AnimationClip flyingAnimationClip;
		public AnimationClip deathAnimationClip;

		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;
		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnJump;
		public event Action<Vector2> OnResting;
		public event Action OnFalling;

		public bool IsResting => isResting;
		private bool isResting;
		private bool isOverLandable;
		public bool IsJumping => isJumping;
		private bool isJumping;

		private float FallInDistance = 80;
		private float minBounceVelocity = 1000;
		private float bounceVelocityDragFactor = .2f;
		private float airTimer;
		private float maxFlyTime = 2.5f;

		private bool isFlying;
		public bool IsFalling { get; private set; }

		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => true;

		public override string AbilityName => "Jump";

		public override bool canDo() => base.canDo() && IsResting;

		public override bool canStop(IDoableAbility abilityToStopFor) => false;

		protected override void DoAbility()
		{
			Jump(0, life.JumpSpeed, 99);
		}

		public override void Stop()
		{
			base.Stop();
			body.SetDistanceToGround(0);
		}

		private void Jump(float startingHeight = 0, float verticalSpeed = 2, float minBounce = 1)
		{
			StartJumpAnimation();
			airTimer = 0;
			minBounceVelocity = minBounce;
			isFlying = false;
			isJumping = true;
			isResting = false;
			IsFalling = false;
			OnJump?.Invoke(transform.position + new Vector3(0, startingHeight, 0));

			verticalVelocity = verticalSpeed;

			body.SetDistanceToGround(startingHeight);
			body.ChangeLayer(Body.BodyLayer.jumping);
		}

		protected virtual void StartJumpAnimation()
		{
			anim.Play(isFlying ? flyingAnimationClip.name : jumpingAnimationClip.name, 0, 0);
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			life.OnDying += Life_OnDying;
			_player.Controller.Jump.OnPress += Controller_Jump;
			FallFromHeight(FallInDistance);
		}

		private void Controller_Jump(NewControlButton newControlButton)
		{
			if (!canDo()) return;

			Do();
		}

		private void Life_OnDying(Attack attack)
		{
			isFlying = true;
			Jump();
		}

		private void OnDisable()
		{
			if (life == null) return;
			if (life.Player == null) return;
			life.Player.Controller.Jump.OnPress -= Controller_Jump;
			life.OnDying -= Life_OnDying;
		}

		private void FallFromHeight(float fallHeight)
		{
			body.SetDistanceToGround(fallHeight);
			isJumping = true;
			isResting = false;
			anim.Play(fallingAnimationClip.name, 0, 0);
			airTimer = 0;
		}

		protected void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (isResting) return;
			if (!isJumping) return;

			Fly();
		}

		private void Fly()
		{
			airTimer += Time.fixedDeltaTime;
			if (airTimer > maxFlyTime)
			{
				Land();
				return;
			}

			verticalVelocity -= Services.assetManager.Vars.Gravity.y * Time.fixedDeltaTime;
			if (!IsFalling && verticalVelocity < 0)
			{
				IsFalling = true;
				StartFalling();
				OnFalling?.Invoke();
			}

			IsFalling = verticalVelocity < 0 && !isResting;
			if (body.GetDistanceToGround() + verticalVelocity <= 0 && verticalVelocity < 0)
			{
				if (Mathf.Abs(verticalVelocity) > minBounceVelocity)
				{
					Bounce();
				}
				else
				{
					Land();
				}
			}
			else
				body.SetDistanceToGround(body.GetDistanceToGround() + verticalVelocity);
		}

		protected virtual void StartFalling()
		{

			anim.Play(fallingAnimationClip.name, 0, 0);
		}

		private void Land()
		{

			isJumping = false;
			moveAbility.SetCanMove(false);
			body.SetGrounded();
			verticalVelocity = 0;
			StartLandingAnimation();
			OnLand?.Invoke(transform.position  );
		}

		protected virtual void StartLandingAnimation()
		{
			PlayAnimationClip(!life.IsDead() ? landingAnimationClip : deathAnimationClip);
		}

		private void Bounce()
		{
			verticalVelocity *= -1;
			var velocity = bounceVelocityDragFactor;
			verticalVelocity *= velocity;
		}

		protected override void AnimationComplete()
		{
			verticalVelocity = 0;
			isJumping = false;
			moveAbility.SetCanMove(true);
			isResting = true;
			OnResting?.Invoke(transform.position);
			Stop();
		}

		public void BounceUpwards(int bounceForce)
		{
			if (!isJumping) return;
			if (verticalVelocity < 0) verticalVelocity = 0;
			verticalVelocity += bounceForce;
		}
	}
}
