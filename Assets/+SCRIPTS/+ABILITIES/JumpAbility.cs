using System;
using System.Collections;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class JumpAbility : Ability
	{
		enum state
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

		void SetState(state newState)
		{
			currentState = newState;
		}

		state currentState;
		float verticalVelocity;

		public AnimationClip jumpingAnimationClip;
		public AnimationClip fallingAnimationClip;
		public AnimationClip landingAnimationClip;
		public AnimationClip flyingAnimationClip;
		public AnimationClip deathAnimationClip;
		public AnimationClip onGroundAnimationClip;
		public AnimationClip getUpAnimationClip;
		public AnimationClip standingAnimationClip;
		ICanMove moveAbility => _moveAbility ??= GetComponent<ICanMove>();
		ICanMove _moveAbility;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnJump;
		public event Action OnFalling;

		const float FallInDistance = 80;
		const float maxAirTime = 2.5f;
		float airTimer;
		float getUpTime = 2;
		bool IsFlying => currentState == state.flying;

		public bool IsFalling => currentState == state.falling;
		public bool IsResting => currentState == state.resting;
		public bool IsInAir => currentState is state.jumpingUp or state.falling or state.flying;

		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => true;

		public override string AbilityName => "Jump";

		public override bool canDo() => base.canDo() && (IsResting || IsFlying);

		public override bool canStop(IDoableAbility abilityToStopFor) => false;

		protected override void DoAbility()
		{
			Jump();
		}

		public override void StopAbility()
		{
			verticalVelocity = 0;
			SetState(state.resting);
			moveAbility.SetCanMove(true);
			body.SetHeight(0);
			base.StopAbility();
		}

		void Jump()
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
			verticalVelocity = offence.stats.Stats.JumpSpeed;
			body.SetHeight(0);
			body.ChangeLayer(Body.BodyLayer.jumping);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			defence.OnDead += DefenceOnDead;
			defence.OnFlying += DefenceOnFlying;
			if (newPlayer.Controller != null) newPlayer.Controller.Jump.OnPress += Controller_Jump;
			SetState(state.resting);
			if (defence.category == UnitCategory.Character) FallFromHeight(FallInDistance);
		}

		void DefenceOnFlying(Attack obj)
		{
			if (Services.pauseManager.IsPaused) return;
			StartFlying();
		}

		void Controller_Jump(NewControlButton newControlButton)
		{
			TryToActivate();
		}

		void DefenceOnDead(Attack attack)
		{
			if(attack.DestinationLife.player.IsHuman()) StartFlying();
		}

		void StartFlying()
		{
			SetState(state.flying);
			//TryToActivate();
			DoAbility();
		}

		void OnDisable()
		{
			if (defence == null) return;
			defence.OnDead -= DefenceOnDead;
			if (defence.player == null) return;
			if (defence.player.Controller == null) return;
			if (defence.player.Controller.Jump == null) return;
			defence.player.Controller.Jump.OnPress -= Controller_Jump;
		}

		void FallFromHeight(float fallHeight)
		{
			SetState(state.falling);
			body.SetHeight(fallHeight);
			PlayAnimationClip(fallingAnimationClip,  0);
			airTimer = 0;
		}

		protected void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (!IsInAir) return;

			Fly();
		}

		void Fly()
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

		void StartFalling()
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
					StopAbility();
					break;
				case state.flying:
					break;
				case state.onGround:
					StartGettingUp();
					break;
				case state.gettingUp:
					anim.Play(standingAnimationClip.name, 0, 0);
					StopAbility();
					break;
				case state.dead:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void StartGettingUp()
		{
			StartCoroutine(GettingUpCoroutine());
		}

		IEnumerator GettingUpCoroutine()
		{
			yield return new WaitForSeconds(getUpTime);
			PlayAnimationClip(getUpAnimationClip);
			SetState(state.gettingUp);
		}

		void HitGround()
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

		void Land()
		{
			defence.SetTemporarilyInvincible(false);
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
