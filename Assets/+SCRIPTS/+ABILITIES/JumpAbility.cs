using System;
using System.Collections;
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
		private ICanMove moveAbility => _moveAbility ??= GetComponent<ICanMove>();
		private ICanMove _moveAbility;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnJump;
		public event Action OnFalling;


		private const float FallInDistance = 80;
		private const float maxAirTime = 2.5f;
		private float airTimer;
		private float getUpTime = 2;
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
			Debug.Log("stopping jump ability");
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
				moveAbility.SetCanMove(false);
				PlayAnimationClip(flyingAnimationClip);
			}
			else
			{
				SetState(state.jumpingUp);
				PlayAnimationClip(jumpingAnimationClip);
			}

			airTimer = 0;
			defence.SetTemporarilyInvincible(true);
			OnJump?.Invoke(transform.position);
			verticalVelocity = offence.stats.JumpSpeed;
			body.SetHeight(0);
			body.ChangeLayer(Body.BodyLayer.jumping);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			defence.OnDead += DefenceOnDead;
			defence.OnFlying += DefenceOnFlying;
			if(newPlayer.Controller != null) newPlayer.Controller.Jump.OnPress += Controller_Jump;
			SetState(state.resting);
			if(defence.category == UnitCategory.Character)FallFromHeight(FallInDistance);
		}

		private void DefenceOnFlying(Attack obj)
		{
			if(Services.pauseManager.IsPaused) return;
			Debug.Log("should start flying");
			StartFlying();
		}

		private void Controller_Jump(NewControlButton newControlButton)
		{
			Try();
		}

		private void DefenceOnDead(Attack attack)
		{
			StartFlying();
		}

		private void StartFlying()
		{
			SetState(state.flying);
			Try();
		}

		private void OnDisable()
		{
			if (defence == null) return;
			defence.OnDead -= DefenceOnDead;
			if (defence.player == null) return;
			if (defence.player.Controller == null) return;
			if (defence.player.Controller.Jump == null) return;
			defence.player.Controller.Jump.OnPress -= Controller_Jump;
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
			StartCoroutine(GettingUpCoroutine());

		}

		private IEnumerator GettingUpCoroutine()
		{
			Debug.Log("coroutine started");
			yield return new WaitForSeconds(getUpTime);
			Debug.Log("coroutine finished");
			PlayAnimationClip(getUpAnimationClip);
			SetState(state.gettingUp);
		}

		private void HitGround()
		{
			if (defence.IsDead())
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
			defence.SetTemporarilyInvincible(false);
			moveAbility.SetCanMove(false);
			Debug.Log("move should be false");
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
