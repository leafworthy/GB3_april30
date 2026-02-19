using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class ChargeAttack : Ability
	{
		enum state
		{
			not,
			startingCharge,
			charged,
			attacking
		}

		state currentState;

		void SetState(state state)
		{
			currentState = state;
		}

		public override string AbilityName => "ChargeAttack";

		public AnimationClip chargeStartAnimationClip;
		public AnimationClip chargeAttackAnimationClip;
		public AnimationClip chargedAnimationClip;
		public event Action<Attack> OnAttackHit;
		public event Action<Attack> OnSpecialAttackHit;
		public event Action OnChargePress;
		public event Action OnChargeStop;

		Vector2 targetPosition;

		BatAttack batAttack => _batAttack ??= GetComponent<BatAttack>();
		BatAttack _batAttack;

		MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		MoveAbility _moveAbility;

		IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		IAimAbility _aimAbility;

		bool isFullyCharged;

		AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		AmmoInventory _ammoInventory;

		float SpecialAttackDistance = 4;
		float SpecialAttackWidth = 4;
		float SpecialAttackExtraPush = 200;
		GameObject currentArrowHead;

		JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		JumpAbility _jumpAbility;
		int rate = 1;
		int counter;

		public override bool canDo() => base.canDo() && currentState == state.not && jumpAbility.IsResting;

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		protected override void DoAbility()
		{
			StartCharging();
		}

		void StartCharging()
		{
			SetState(state.startingCharge);
			moveAbility.SetCanMove(false);
			isFullyCharged = false;
			OnChargePress?.Invoke();
			PlayAnimationClip(chargeStartAnimationClip);
			ammoInventory.secondaryAmmo.reserveAmmo = 0;
		}

		public override void SetPlayer(Player newPlayer)
		{
			if (newPlayer == null) return;
			base.SetPlayer(newPlayer);

			if (defence != null)
			{
				player.Controller.InteractRightShoulder.OnPress -= Player_ChargePress;
				player.Controller.InteractRightShoulder.OnRelease -= Player_ChargeRelease;

				player.Controller.InteractRightShoulder.OnPress += Player_ChargePress;
				player.Controller.InteractRightShoulder.OnRelease += Player_ChargeRelease;
			}

			ammoInventory.tertiaryAmmo.reserveAmmo = 0;
			SetState(state.not);
			InstantiateArrowHead();
			UseAllAmmo();
		}

		void OnDisable()
		{
			if (player == null) return;
			player.Controller.InteractRightShoulder.OnPress -= Player_ChargePress;
			player.Controller.InteractRightShoulder.OnRelease -= Player_ChargeRelease;
		}

		void FixedUpdate()
		{
			if (currentState is state.startingCharge or state.charged)
			{
				ShowAiming();
				AddCharge();
			}
			else
				HideAiming();
		}

		void AddCharge()
		{
			if (isFullyCharged) return;
			counter++;
			if (counter < rate) return;
			counter = 0;
			if (currentState != state.charged && currentState != state.startingCharge) return;
			if (ammoInventory.secondaryAmmo.reserveAmmo < 100)
				ammoInventory.secondaryAmmo.AddAmmoToReserve(5);
			else if (!isFullyCharged)
				FullyCharged();
		}

		void InstantiateArrowHead()
		{
			currentArrowHead = Services.objectMaker.Make(Services.assetManager.FX.nadeTargetPrefab);
			currentArrowHead.SetActive(false);
		}

		void ShowAiming()
		{
			currentArrowHead.SetActive(true);
			if (!player.Controller.AimAxis.isActive && !player.Controller.MoveAxis.isActive)
			{
				if (targetPosition == Vector2.zero)
				{
					var _targetPosition = (Vector2) transform.position + (body.TopIsFacingRight ? Vector2.right * 30 : Vector2.left * 30);
					SetTargetPosition(_targetPosition);
				}
				else
					SetTargetPosition(targetPosition);
			}
			else
			{
				var _targetPosition = !player.Controller.AimAxis.isActive ? moveAbility.GetMoveAimPoint() : aimAbility.GetAimPoint();
				SetTargetPosition(_targetPosition);
			}
		}

		void SetTargetPosition(Vector2 _targetPosition)
		{
			targetPosition = _targetPosition;
			currentArrowHead.transform.position = targetPosition;
			body.BottomFaceDirection(targetPosition.x > 0);
		}

		void HideAiming() => currentArrowHead.SetActive(false);
		void UseAllAmmo() => ammoInventory.secondaryAmmo.UseAmmo(1000);

		void Player_ChargePress(NewControlButton newControlButton)
		{
			TryToActivate();
		}

		void Player_ChargeRelease(NewControlButton newControlButton)
		{
			if (Services.pauseManager.IsPaused) return;
			if (currentState == state.not) return;
			if (!jumpAbility.IsResting) return;
			if (defence.IsDead()) return;
			SetState(state.attacking);
			if (isFullyCharged)
				StartSpecialAttack();
			else
				DoWeakAttack();
		}

		void DoWeakAttack()
		{
			StopCharging();
			batAttack.TryToActivate();
		}

		void StopCharging()
		{
			anim.SetBool(UnitAnimations.IsCharging, false);
			isFullyCharged = false;
			UseAllAmmo();
			OnChargeStop?.Invoke();
			StopAbility();
		}

		void StartSpecialAttack()
		{
			UseAllAmmo();
			OnChargeStop?.Invoke();

			anim.SetTrigger(UnitAnimations.ChargeAttackTrigger);
			anim.SetBool(UnitAnimations.IsCharging, false);
			PlayAnimationClip(chargeAttackAnimationClip);
			Invoke(nameof(Anim_AttackHit), chargeAttackAnimationClip.length / 2);
			if (player.Controller.MoveAxis.isActive) moveAbility.Push(moveAbility.GetMoveAimDir(), SpecialAttackExtraPush);
			else if (player.Controller.AimAxis.isActive) moveAbility.Push(aimAbility.AimDir, SpecialAttackExtraPush);
			else
			{
				var faceRight = body.TopIsFacingRight;
				currentArrowHead.transform.position =
					faceRight ? (Vector2) transform.position + Vector2.right * 30 : (Vector2) transform.position + Vector2.left * 30;
				moveAbility.Push(faceRight ? Vector2.right : Vector2.left, SpecialAttackExtraPush);
			}
		}

		protected override void AnimationComplete()
		{
			switch (currentState)
			{
				case state.not:
					break;
				case state.startingCharge:
					StartActualCharging();
					break;
				case state.charged:
					break;
				case state.attacking:
					StopAbility();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void StopAbility()
		{
			moveAbility.SetCanMove(true);
			SetState(state.not);
			base.StopAbility();
		}

		void StartActualCharging()
		{
			SetState(state.charged);
			PlayAnimationClip(chargedAnimationClip);
		}

		void Anim_AttackHit()
		{
			SpecialAttackHit();
		}

		void FullyCharged()
		{
			isFullyCharged = true;
			ammoInventory.secondaryAmmo.SetAmmoReserve(100);
		}

		void SpecialAttackHit()
		{
			var attackPosition = body.AimCenter.transform.position;
			var bestTargetPoint = GetBestTargetPoint(attackPosition);
			var hasSpecialHit = false;

			SpecialAttackDistance = Vector2.Distance(attackPosition, bestTargetPoint);

			var raycastHits = Physics2D.CircleCastAll(attackPosition, SpecialAttackWidth, moveAbility.GetMoveAimDir(), SpecialAttackDistance,
				offence.EnemyLayer);

			foreach (var raycastHit2D in raycastHits)
			{
				var otherLife = raycastHit2D.collider.GetComponent<Life>();
				if(otherLife == null) continue;
				var attack = MyAttackUtilities.HitTarget(offence, otherLife, offence.stats.Stats.Damage(2), SpecialAttackExtraPush, true);

				if (attack == null) continue;
				if (!hasSpecialHit)
				{
					hasSpecialHit = true;
					SpecialHit(attack);
				}
				else OnAttackHit?.Invoke(attack);
			}
		}

		void SpecialHit(Attack attack)
		{
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Special);
			TempCinemachine.CreateFollowCameraTemporary(transform, 1, 35, true);
			OnSpecialAttackHit?.Invoke(attack);

		}

		Vector2 GetBestTargetPoint(Vector3 attackPosition) =>
			moveAbility.IsIdle() ? aimAbility.GetAimPoint() : (Vector2) attackPosition + moveAbility.GetLastMoveAimDirOffset();

		float GetHitRange() => offence.stats.Stats.Range(1);
	}
}
