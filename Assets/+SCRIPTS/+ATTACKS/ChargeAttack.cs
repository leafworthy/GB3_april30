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
			charging,
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
		public AnimationClip chargingAnimationClip;
		public event Action OnAttackHit;
		public event Action OnSpecialAttackHit;
		public event Action OnChargePress;
		public event Action OnChargeStop;

		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _aimAbility;

		private bool isCharging;
		private bool isFullyCharged;
		private bool isPressingCharge;

		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;

		private float SpecialAttackDistance = 3;
		private float SpecialAttackWidth = 3;
		private float SpecialAttackExtraPush = 2;
		private GameObject currentArrowHead;

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		protected override void DoAbility()
		{
			StartCharging();
		}

		private void StartCharging()
		{
			isCharging = true;
			isFullyCharged = false;
			OnChargePress?.Invoke();

			anim.SetTrigger(UnitAnimations.ChargeStartTrigger);
			anim.SetBool(UnitAnimations.IsCharging, true);
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);

			if (life != null)
			{
				life.Player.Controller.Attack2LeftTrigger.OnPress += Player_ChargePress;
				life.Player.Controller.Attack2LeftTrigger.OnRelease += Player_ChargeRelease;
			}

			SetState(state.not);
			InstantiateArrowHead();
		}

		private void OnDisable()
		{
			if (life != null) return;
			life.Player.Controller.Attack2LeftTrigger.OnPress -= Player_ChargePress;
			life.Player.Controller.Attack2LeftTrigger.OnRelease -= Player_ChargeRelease;
		}

		private void Update()
		{
			if (currentState == state.startingCharge)
				ShowAiming();
			else
				HideAiming();
		}

		private void InstantiateArrowHead()
		{
			currentArrowHead = Services.objectMaker.Make(Services.assetManager.FX.nadeTargetPrefab);
			currentArrowHead.SetActive(false);
		}

		private void ShowAiming()
		{
			currentArrowHead.SetActive(true);
			currentArrowHead.transform.position = moveAbility.GetMoveAimPoint();
			body.BottomFaceDirection(moveAbility.GetMoveAimDir().x > 0);
		}

		private void HideAiming() => currentArrowHead.SetActive(false);
		private void UseAllAmmo() => ammoInventory.secondaryAmmo.UseAmmo(1000);

		private void Player_ChargePress(NewControlButton newControlButton)
		{
			isPressingCharge = true;
			if (isCharging) return;
			Do();
		}

		private void Player_ChargeRelease(NewControlButton newControlButton)
		{
			isPressingCharge = false;
			if (Services.pauseManager.IsPaused) return;
			if (!isCharging) return;
			StopCharging();
			if (isFullyCharged)
				StartAttack();
			else
				Stop();
		}

		private void StopCharging()
		{
			isCharging = false;
			anim.SetBool(UnitAnimations.IsCharging, false);
			UseAllAmmo();
			OnChargeStop?.Invoke();
		}

		private void StartAttack()
		{
			UseAllAmmo();
			anim.SetTrigger(UnitAnimations.ChargeAttackTrigger);
			anim.SetBool(UnitAnimations.IsCharging, false);
			PlayAnimationClip(chargeAttackAnimationClip);
			Invoke(nameof(FullyCharged), chargeStartAnimationClip.length + chargingAnimationClip.length);
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
				case state.charging:
					break;
				case state.attacking:
					Stop();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			base.AnimationComplete();
		}

		private void StartActualCharging()
		{
			SetState(state.charging);
			PlayAnimationClip(chargingAnimationClip);
		}

		private void Anim_AttackHit(int attackType)
		{
			if (attackType != 5) return;
			SpecialAttackHit(attackType);
		}

		private void FullyCharged()
		{
			isFullyCharged = true;
		}

		private void SpecialAttackHit(int attackType)
		{
			var attackPosition = body.AimCenter.transform.position;
			var bestTargetPoint = GetBestTargetPoint(attackPosition);
			var connect = false;

			SpecialAttackDistance = Vector2.Distance(attackPosition, bestTargetPoint);

			MoveIfDidCollideWithBuilding(attackPosition, bestTargetPoint, out var raycastHits);
			OnSpecialAttackHit?.Invoke();

			foreach (var raycastHit2D in raycastHits)
			{
				var otherLife = raycastHit2D.collider.GetComponent<Life>();
				AttackUtilities.HitTarget(life, otherLife, raycastHit2D.point,life.SecondaryAttackDamageWithExtra);
			}

			var circleCast = Physics2D.OverlapCircleAll((Vector2) transform.position + moveAbility.GetMoveAimDir() * SpecialAttackDistance,
				GetHitRange(attackType));

			foreach (var hit2D in circleCast)
			{
				var otherLife = hit2D.gameObject.GetComponent<Life>();
				if (otherLife == null) continue;
				if (!otherLife.IsEnemyOf(life) || otherLife.IsNotInvincible || otherLife.IsObstacle) return;
				AttackUtilities.HitTarget(life, otherLife, hit2D.ClosestPoint(body.AttackStartPoint.transform.position), otherLife.SecondaryAttackDamageWithExtra, SpecialAttackExtraPush);
				connect = true;
				Services.objectMaker.Make(Services.assetManager.FX.hits.GetRandom(), hit2D.transform.position);
			}

			if (!connect) return;
			OnAttackHit?.Invoke();
		}

		private Vector2 GetBestTargetPoint(Vector3 attackPosition) => moveAbility.IsIdle() ? aimAbility.GetAimPoint() : (Vector2) attackPosition + moveAbility.GetLastMoveAimDirOffset();

		private void MoveIfDidCollideWithBuilding(Vector3 position, Vector3 targetPoint, out RaycastHit2D[] raycast)
		{
			var hitObstacle = Physics2D.Linecast(position, targetPoint, Services.assetManager.LevelAssets.BuildingLayer);
			if (hitObstacle) targetPoint = hitObstacle.point;

			raycast = Physics2D.CircleCastAll(position, SpecialAttackWidth, moveAbility.GetMoveAimDir(), SpecialAttackDistance,
				Services.assetManager.LevelAssets.EnemyLayer);
			transform.position = targetPoint;
		}

		private float GetHitRange(int attackType)
		{
			if (attackType == 5)
				return life.PrimaryAttackRange * 2;
			return life.PrimaryAttackRange;
		}
	}
}
