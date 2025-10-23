using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class JumpAbility : Ability
	{
		private enum state
		{
			resting,
			jumpingUp,
			falling,
			landing,
			flying,
			onGround,
			gettingUp,
			dead
		}
		private void SetState(state newState)
		{
			currentState = newState;
		}

		private state currentState;
		private float verticalVelocity;

		public AnimationClip jumpingAnimationClip;
		public AnimationClip fallingAnimationClip;
		public AnimationClip landingAnimationClip;
		public AnimationClip flyingAnimationClip;
		public AnimationClip deathAnimationClip;
		public AnimationClip onGroundAnimationClip;
		public AnimationClip getUpAnimationClip;
		public AnimationClip standingAnimationClip;
		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnJump;
		public event Action OnFalling;


		private const float FallInDistance = 80;
		private const float maxAirTime = 2.5f;
		private float airTimer;
		private bool IsFlying  => currentState == state.flying;

		public bool IsFalling => currentState == state.falling;
		public bool IsResting => currentState == state.resting;
		public bool IsInAir => currentState is state.jumpingUp or state.falling or state.flying;

		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => true;

		public override string AbilityName => "Jump";

		public override bool canDo()
		{
			if(!IsResting && !IsFlying) Debug.Log("cant cuz NOT RESTING");
			return base.canDo() && (IsResting || IsFlying);
		}

		public override bool canStop(IDoableAbility abilityToStopFor) => false;

		protected override void DoAbility()
		{
			Jump();
		}

		public override void Stop()
		{
			verticalVelocity = 0;
			SetState(state.resting);
			moveAbility.SetCanMove(true);
			body.SetHeight(0);
			base.Stop();
		}

		private void Jump()
		{
			if (currentState is state.flying)
			{
				StartFlyingAnimation();
			}
			else
			{
				SetState(state.jumpingUp);
				StartJumpAnimation();
			}

			airTimer = 0;
			life.SetTemporarilyInvincible(true);
			OnJump?.Invoke(transform.position);
			verticalVelocity = life.JumpSpeed;
			body.SetHeight(0);
			body.ChangeLayer(Body.BodyLayer.jumping);
		}

		private void StartJumpAnimation()
		{
			Debug.Log("tryin reg jump");
			PlayAnimationClip(jumpingAnimationClip);
		}

		private void StartFlyingAnimation()
		{
			Debug.Log("should play flying anim");
			PlayAnimationClip(flyingAnimationClip);
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			life.OnDying += Life_OnDying;
			life.OnFlying += Life_OnFlying;
			if(_player.Controller != null) _player.Controller.Jump.OnPress += Controller_Jump;
			SetState(state.resting);
			if(life.category == UnitCategory.Character)FallFromHeight(FallInDistance);
		}

		private void Life_OnFlying(Attack obj)
		{
			if(Services.pauseManager.IsPaused) return;
			Debug.Log("should start flying");
			StartFlying();
		}

		private void Controller_Jump(NewControlButton newControlButton)
		{
			Do();
		}

		private void Life_OnDying(Attack attack)
		{
			StartFlying();
		}

		private void StartFlying()
		{
			forceIt = true;
			SetState(state.flying);
			DoAbility();
		}

		private void OnDisable()
		{
			if (life == null) return;
			life.OnDying -= Life_OnDying;
			if (life.Player == null) return;
			if (life.Player.Controller == null) return;
			if (life.Player.Controller.Jump == null) return;
			life.Player.Controller.Jump.OnPress -= Controller_Jump;
		}

		private void FallFromHeight(float fallHeight)
		{
			SetState(state.falling);
			body.SetHeight(fallHeight);
			PlayAnimationClip(fallingAnimationClip.name, 0, 0);
			airTimer = 0;
		}

		protected void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (!IsInAir) return;

			Fly();
		}

		private void Fly()
		{
			airTimer += Time.fixedDeltaTime;
			if (airTimer > maxAirTime)
			{
				HitGround();
				return;
			}

			verticalVelocity -= Services.assetManager.Vars.Gravity.y * Time.fixedDeltaTime;
			if (!IsFalling && verticalVelocity < 0) StartFalling();

			if (body.GetHeight() + verticalVelocity <= 0 && verticalVelocity < 0)
				HitGround();
			else
				body.SetHeight(body.GetHeight() + verticalVelocity);
		}

		private void StartFalling()
		{
			if (!IsInAir) return;
			if (currentState is not state.jumpingUp) return;
			OnFalling?.Invoke();
			SetState(state.falling);
			PlayAnimationClip(fallingAnimationClip);
		}

		protected override void AnimationComplete()
		{
			switch (currentState)
			{
				case state.resting:
				case state.jumpingUp:
				case state.falling:
					break;
				case state.landing:
					Stop();
					break;
				case state.flying:
					break;
				case state.onGround:
					StartGettingUp();
					break;
				case state.gettingUp:
					anim.Play(standingAnimationClip.name,0,0);
					Stop();
					break;
				case state.dead:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void StartGettingUp()
		{
			PlayAnimationClip(getUpAnimationClip);
			SetState(state.gettingUp);
		}

		private void HitGround()
		{
			if (life.IsDead())
			{
				PlayAnimationClip(deathAnimationClip);
				SetState(state.dead);
			}
			else if (currentState == state.flying)
			{
				PlayAnimationClip(onGroundAnimationClip);
				SetState(state.onGround);
			}
			else
			{
				PlayAnimationClip(landingAnimationClip);
				SetState(state.landing);
			}

			Land();
		}

		private void Land()
		{
			life.SetTemporarilyInvincible(false);
			moveAbility.SetCanMove(false);
			body.SetGrounded();
			verticalVelocity = 0;
			OnLand?.Invoke(transform.position);
		}

		public void DoubleJump(float doubleJumpForce)
		{
			if (!IsInAir) return;
			if (verticalVelocity < 0) verticalVelocity = 0;
			verticalVelocity += doubleJumpForce;
		}
	}
}
