using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class ChargeAttack : Ability
	{
		private enum state
		{
			not,
			startingCharge,
			charged,
			attacking
		}

		private state currentState;

		private void SetState(state state)
		{
			currentState = state;
		}

		public override string AbilityName => "ChargeAttack";

		public AnimationClip chargeStartAnimationClip;
		public AnimationClip chargeAttackAnimationClip;
		public AnimationClip chargedAnimationClip;
		public event Action OnAttackHit;
		public event Action OnSpecialAttackHit;
		public event Action OnChargePress;
		public event Action OnChargeStop;

		private Vector2 targetPosition;

		private BatAttack batAttack => _batAttack ??= GetComponent<BatAttack>();
		private BatAttack _batAttack;

		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _aimAbility;

		private bool isFullyCharged;

		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;

		private float SpecialAttackDistance = 3;
		private float SpecialAttackWidth = 3;
		private float SpecialAttackExtraPush = 200;
		private GameObject currentArrowHead;

		private JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		private JumpAbility _jumpAbility;
		private int rate = 1;
		private int counter;

		public override bool canDo() => base.canDo() && currentState == state.not && jumpAbility.IsResting;

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		protected override void DoAbility()
		{
			StartCharging();
		}

		private void StartCharging()
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

		private void OnDisable()
		{
			if (defence != null) return;
			player.Controller.InteractRightShoulder.OnPress -= Player_ChargePress;
			player.Controller.InteractRightShoulder.OnRelease -= Player_ChargeRelease;
		}

		private void FixedUpdate()
		{
			if (currentState is state.startingCharge or state.charged)
			{
				ShowAiming();
				AddCharge();
			}
			else
				HideAiming();
		}

		private void AddCharge()
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

		private void InstantiateArrowHead()
		{
			currentArrowHead = Services.objectMaker.Make(Services.assetManager.FX.nadeTargetPrefab);
			currentArrowHead.SetActive(false);
		}

		private void ShowAiming()
		{
			currentArrowHead.SetActive(true);
			if (!player.Controller.AimAxis.isActive && !player.Controller.MoveAxis.isActive)
			{
				if (targetPosition == Vector2.zero)
				{
					var _targetPosition = (Vector2)transform.position + (body.TopIsFacingRight ? Vector2.right * 30 : Vector2.left * 30);
					SetTargetPosition(_targetPosition);
				}
				else
				{
					SetTargetPosition(targetPosition);
				}
			}
			else
			{
				var _targetPosition = !player.Controller.AimAxis.isActive ? moveAbility.GetMoveAimPoint() : aimAbility.GetAimPoint();
				SetTargetPosition(_targetPosition);
			}
		}

		private void SetTargetPosition(Vector2 _targetPosition)
		{
			targetPosition = _targetPosition;
			currentArrowHead.transform.position = (Vector2) targetPosition;
			body.BottomFaceDirection(targetPosition.x > 0);
		}

		private void HideAiming() => currentArrowHead.SetActive(false);
		private void UseAllAmmo() => ammoInventory.secondaryAmmo.UseAmmo(1000);

		private void Player_ChargePress(NewControlButton newControlButton)
		{
			Try();
		}

		private void Player_ChargeRelease(NewControlButton newControlButton)
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

		private void DoWeakAttack()
		{
			StopCharging();
			batAttack.Try();
		}

		private void StopCharging()
		{
			anim.SetBool(UnitAnimations.IsCharging, false);
			isFullyCharged = false;
			UseAllAmmo();
			OnChargeStop?.Invoke();
			Stop();
		}

		private void StartSpecialAttack()
		{
			UseAllAmmo();
			OnChargeStop?.Invoke();
			CameraStunner_FX.StartStun(CameraStunner_FX.StunLength.Special);
			TempCinemachine.CreateFollowCameraTemporary(transform, .75f, zoom: 35, orthographic: true);
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
					Stop();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void Stop()
		{
			moveAbility.SetCanMove(true);
			SetState(state.not);
			base.Stop();
		}

		private void StartActualCharging()
		{
			SetState(state.charged);
			PlayAnimationClip(chargedAnimationClip);
		}

		private void Anim_AttackHit()
		{
			SpecialAttackHit();
		}

		private void FullyCharged()
		{
			isFullyCharged = true;
			ammoInventory.secondaryAmmo.SetAmmoReserve(100);
		}

		private void SpecialAttackHit()
		{
			var attackPosition = body.AimCenter.transform.position;
			var bestTargetPoint = GetBestTargetPoint(attackPosition);
			var connect = false;

			SpecialAttackDistance = Vector2.Distance(attackPosition, bestTargetPoint);

			var raycastHits = Physics2D.CircleCastAll(attackPosition, SpecialAttackWidth, moveAbility.GetMoveAimDir(), SpecialAttackDistance, offence.EnemyLayer);
			OnSpecialAttackHit?.Invoke();

			foreach (var raycastHit2D in raycastHits)
			{
				var otherLife = raycastHit2D.collider.GetComponent<Life>();
				MyAttackUtilities.HitTarget(offence, otherLife, offence.stats.Stats.Damage(2));
			}

			var circleCast = Physics2D.OverlapCircleAll((Vector2) transform.position + moveAbility.GetMoveAimDir() * SpecialAttackDistance, GetHitRange(),
				offence.EnemyLayer);

			foreach (var hit2D in circleCast)
			{
				var otherLife = hit2D.gameObject.GetComponent<Life>();
				if(otherLife == null) continue;
				MyAttackUtilities.HitTarget(offence, otherLife, otherLife.Stats.Damage(2), SpecialAttackExtraPush, true);

				connect = true;
				Services.objectMaker.Make(Services.assetManager.FX.hits.GetRandom(), hit2D.transform.position);
			}

			if (!connect) return;
			OnAttackHit?.Invoke();
		}

		private Vector2 GetBestTargetPoint(Vector3 attackPosition) =>
			moveAbility.IsIdle() ? aimAbility.GetAimPoint() : (Vector2) attackPosition + moveAbility.GetLastMoveAimDirOffset();

		private float GetHitRange() => offence.stats.Stats.Range(1);
	}
}
