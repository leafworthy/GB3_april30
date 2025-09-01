using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableJumpAbility : Ability
	{
		private float verticalVelocity;

		public AnimationClip jumpingAnimationClip;
		public AnimationClip fallingAnimationClip;
		public AnimationClip landingAnimationClip;
		public AnimationClip flyingAnimationClip;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnJump;
		public event Action<Vector2> OnResting;

		public bool IsResting  => isResting;
		private bool isResting;
		private bool isOverLandable;
		private float currentLandableHeight;
		public bool IsJumping => isJumping;
		private bool isJumping;

		private float FallInDistance = 80;
		private float minBounceVelocity = 1000;
		private float bounceVelocityDragFactor = .2f;
		private float airTimer;
		private float maxFlyTime = 2.5f;

		private bool isFlying;

		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => true;

		public override string VerbName => "Jump";

		public override bool canDo()
		{
			if (!base.canDo()) return false;

			//if (body.doableArms.IsActive)
			//{
			//	Debug.Log("Cannot jump, arms are active" + (body.doableArms.CurrentAbility != null ? $"({body.doableArms.CurrentAbility.VerbName})" : ""));
			//	return false;
			//}

			return IsResting;
		}

		public override bool canStop() => false;

		protected override void DoAbility()
		{
			Jump(0, life.JumpSpeed, 99);
		}

		public override void Stop()
		{
			Debug.Log("jump ability stop");
			base.Stop();
			body.SetDistanceToGround(0);
		}

		private void Jump(float startingHeight = 0, float verticalSpeed = 2, float minBounce = 1)
		{
			anim.Play(isFlying ? flyingAnimationClip.name : jumpingAnimationClip.name, 0, 0);
			isFlying = false;
			airTimer = 0;
			minBounceVelocity = minBounce;
			isJumping = true;
			isResting = false;
			OnJump?.Invoke(transform.position + new Vector3(0, startingHeight, 0));

			verticalVelocity = verticalSpeed;

			body.SetDistanceToGround(startingHeight);
			body.ChangeLayer(Body.BodyLayer.jumping);
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
			if (!canDo())
			{
				Debug.Log("Cannot jump, not resting or dead or paused or arms active");
				return;
			}

			Do();
		}

		private void Life_OnDying(Player arg1, Life arg2)
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

			verticalVelocity -= assetManager.Vars.Gravity.y * Time.fixedDeltaTime;
			if (body.GetDistanceToGround() + verticalVelocity <= currentLandableHeight && verticalVelocity < 0)
				Land();
			else
			{
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

			body.SetCanMove(false);
			body.SetDistanceToGround(currentLandableHeight);
			if (body != null) body.ChangeLayer(Body.BodyLayer.grounded);
			verticalVelocity = 0;
			PlayAnimationClip(landingAnimationClip);
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
			body.SetCanMove(true);
			isResting = true;
			OnResting?.Invoke(transform.position);
			Stop();
		}
	}
}
