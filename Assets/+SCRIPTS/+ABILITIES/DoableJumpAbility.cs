using System;
using GangstaBean.Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{

	public class DoableJumpAbility : ServiceUser, IDoableActivity, INeedPlayer
	{
		private Body body;
		private float verticalVelocity;

		public AnimationClip jumpingAnimationClip;
		public AnimationClip fallingAnimationClip;
		public AnimationClip landingAnimationClip;
		public AnimationClip flyingAnimationClip;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnJump;

		public bool isResting;
		private bool isOverLandable;
		private float currentLandableHeight;
		private bool isJumping;

		private float FallInDistance = 80;
		private float minBounceVelocity = 1000;
		private float bounceVelocityDragFactor = .2f;
		private float airTimer;
		private float maxFlyTime = 2.5f;

		private Life life;
		private Animations anim;
		private bool isFlying;

		public string VerbName => "Jump";

		public bool canDo() => isResting && !life.IsDead() && pauseManager.IsPaused;

		public bool canStop() => false;

		public bool Do()
		{
			Jump(body.GetCurrentLandableHeight(), life.JumpSpeed, 99);
			return true;
		}

		public void ForceStop()
		{
			CancelInvoke(nameof(StartResting));
			StartResting();

			body.canLand = false;
			body.SetDistanceToGround(body.GetCurrentLandableHeight());
		}

		private void Jump(float startingHeight = 0, float verticalSpeed = 2, float minBounce = 1)
		{

			anim.Play(isFlying? flyingAnimationClip.name : jumpingAnimationClip.name,0,0);
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

		public void SetPlayer(Player _player)
		{
			life = GetComponent<Life>();
			life.OnDying += Life_OnDying;

			_player.Controller.Jump.OnPress += Controller_Jump;

			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			body.OnFallFromLandable += FallFromHeight;

			FallFromHeight(FallInDistance);
		}


		private void Controller_Jump(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			if (!body.doableArms.CanDoerDo(this) || !body.doableLegs.CanDoerDo(this)) return;
			SetActive(true);
		}

		private void SetActive(bool _active)
		{
			if(_active)
			{
				body.doableArms.DoSafely(this);
				body.doableLegs.DoSafely(this);
			}
			else
			{
				body.doableArms.Stop(this);
				body.doableLegs.Stop(this);
			}
		}

		private void Life_OnDying(Player arg1, Life arg2)
		{
			isFlying = true;
			Jump();
		}


		private void OnDisable()
		{
			if (body != null) body.OnFallFromLandable -= FallFromHeight;
			if (life == null) return;
			if (life.player == null) return;
			life.player.Controller.Jump.OnPress -= Controller_Jump;
			life.OnDying -= Life_OnDying;
		}

		private void FallFromHeight(float fallHeight)
		{
			body.SetDistanceToGround(fallHeight);
			isJumping = true;
			isResting = false;
			anim.Play(fallingAnimationClip.name,0,0);
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

			currentLandableHeight = body.GetCurrentLandableHeight();
			verticalVelocity -= assets.Vars.Gravity.y * Time.fixedDeltaTime;
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
			body.SetCanMove(false);
			body.SetDistanceToGround(currentLandableHeight);
			if (body != null) body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			verticalVelocity = 0;
			Invoke(nameof(StartResting), landingAnimationClip.length);
		}

		private void Bounce()
		{
			verticalVelocity *= -1;
			var velocity = bounceVelocityDragFactor;
			verticalVelocity *= velocity;
		}

		public void StartResting()
		{
			verticalVelocity = 0;
			isResting = true;
			isJumping = false;
			body.canLand = false;
			body.SetCanMove(true);
			SetActive(false);
		}
	}
}
