using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class JumpAbility : DoableActivity
	{
		private float verticalVelocity;

		public AnimationClip jumpingAnimationClip;
		public AnimationClip fallingAnimationClip;
		public AnimationClip landingAnimationClip;
		public AnimationClip flyingAnimationClip;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnResting;
		public event Action<Vector2> OnJump;
		public event Action<Vector2> OnFall;
		public event Action OnBounce;

		public bool isResting;
		private bool isOverLandable;
		private float currentLandableHeight;
		private bool isJumping;

		private float FallInDistance = 80;
		private float minBounceVelocity = 1000;
		private float bounceVelocityDragFactor = .2f;
		private float airTimer;
		private float maxFlyTime = 2.5f;
		private bool canJump;

		public override string VerbName => "Jump";
		public bool IsJumping=> isJumping;
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && isResting && canJump;

		public override bool canStop() => false;

		public override void StartActivity()
		{
			Jump(body.GetCurrentLandableHeight(), life.JumpSpeed, 99);
		}

		public override void StopActivity()
		{
			base.StopActivity();
			body.canLand = false;
			body.SetDistanceToGround(body.GetCurrentLandableHeight());
		}

		private void Jump(float startingHeight = 0, float verticalSpeed = 2, float minBounce = 1, bool isFlying = false)
		{
			//anim.ResetTrigger(UnitAnimations.LandTrigger);
			//anim.SetTrigger(UnitAnimations.JumpTrigger);
			anim.Play(isFlying ? flyingAnimationClip.name : jumpingAnimationClip.name, 0, 0);
			airTimer = 0;
			minBounceVelocity = minBounce;
			isJumping = true;
			isResting = false;
			OnJump?.Invoke(transform.position + new Vector3(0, startingHeight, 0));

			verticalVelocity = verticalSpeed;

			body.SetDistanceToGround(startingHeight);
			body.ChangeLayer(Body.BodyLayer.jumping);
		}

		private void Controller_Jump(NewControlButton newControlButton)
		{
			if (!canDo()) return;
			DoSafely();
		}

		private void Life_OnDying(Player arg1, Life arg2)
		{
			Jump();
		}

		public void FallFromLandable()
		{
			anim.SetBool(UnitAnimations.IsFalling, true);
			body.SetDistanceToGround(FallInDistance);
			isJumping = true;
			isResting = false;
			anim.Play(fallingAnimationClip.name, 0, 0);
			airTimer = 0;
			OnFall?.Invoke(transform.position);
		}

		protected void FixedUpdate()
		{
			if (pauseManager.IsPaused) return;
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

			currentLandableHeight = body.GetCurrentLandableHeight();
			verticalVelocity -= assetManager.Vars.Gravity.y * Time.fixedDeltaTime;
			if (body.GetDistanceToGround() + verticalVelocity <= currentLandableHeight && verticalVelocity < 0)
				Land();
			else
			{
				body.canLand = verticalVelocity < 0;
				body.SetDistanceToGround(body.GetDistanceToGround() + verticalVelocity);
			}
		}

		private void Land()
		{
			if (Mathf.Abs(verticalVelocity) > minBounceVelocity)
			{
				Bounce();
				return;
			}

			isJumping = false;
			OnLand?.Invoke(transform.position + new Vector3(0, currentLandableHeight, 0));
			anim.Play(landingAnimationClip.name, 0, 0);

			body.canLand = false;
			moveController.SetCanMove(false);
			body.SetDistanceToGround(currentLandableHeight);
			if (body != null) body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			verticalVelocity = 0;
			PlayAnimationClip(landingAnimationClip);
		}

		private void Bounce()
		{
			verticalVelocity *= -1;
			var velocity = bounceVelocityDragFactor;
			verticalVelocity *= velocity;
			OnBounce?.Invoke();
		}

		protected override void AnimationComplete()
		{
			verticalVelocity = 0;
			isResting = true;
			OnResting?.Invoke(transform.position);
			anim.SetBool(UnitAnimations.IsFalling, false);
			isJumping = false;
			body.canLand = false;
			moveController.SetCanMove(true);
			StopActivity();
		}

		public void SetCanJump(bool on)
		{
			canJump = on;
		}

		public void SetDistanceToGround(float attackOriginHeight)
		{
			body.SetDistanceToGround(attackOriginHeight);
		}
	}
}
