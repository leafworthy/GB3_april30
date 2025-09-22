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

		private BatAttack batAttack => _batAttack ??= GetComponent<BatAttack>();
		private BatAttack _batAttack;

		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _aimAbility;

		private bool isFullyCharged;
		private bool isPressingCharge;

		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;

		private float SpecialAttackDistance = 3;
		private float SpecialAttackWidth = 3;
		private float SpecialAttackExtraPush = 2;
		private int chargeRate = 4;
		private GameObject currentArrowHead;

		private JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		private JumpAbility _jumpAbility;

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

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);

			if (life != null)
			{
				life.Player.Controller.InteractRightShoulder.OnPress += Player_ChargePress;
				life.Player.Controller.InteractRightShoulder.OnRelease += Player_ChargeRelease;
			}

			ammoInventory.tertiaryAmmo.reserveAmmo = 0;
			SetState(state.not);
			InstantiateArrowHead();
		}

		private void OnDisable()
		{
			if (life != null) return;
			life.Player.Controller.InteractRightShoulder.OnPress -= Player_ChargePress;
			life.Player.Controller.InteractRightShoulder.OnRelease -= Player_ChargeRelease;
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
			if (!isPressingCharge)
			{
				StopCharging();
				return;
			}
			if (currentState != state.charged && currentState != state.startingCharge) return;
			if (ammoInventory.secondaryAmmo.reserveAmmo < 100)
				ammoInventory.secondaryAmmo.reserveAmmo += chargeRate;
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
			currentArrowHead.transform.position = moveAbility.GetMoveAimPoint();
			body.BottomFaceDirection(moveAbility.GetMoveAimDir().x > 0);
		}

		private void HideAiming() => currentArrowHead.SetActive(false);
		private void UseAllAmmo() => ammoInventory.secondaryAmmo.UseAmmo(1000);

		private void Player_ChargePress(NewControlButton newControlButton)
		{
			isPressingCharge = true;
			Do();
		}

		private void Player_ChargeRelease(NewControlButton newControlButton)
		{
			isPressingCharge = false;
			if (Services.pauseManager.IsPaused) return;
			switch (currentState)
			{
				case state.startingCharge:
					DoWeakAttack();
					break;
				case state.charged:

					break;
			}

			StopCharging();
			if (isFullyCharged)
				StartAttack();
			else
				Stop();
		}

		private void DoWeakAttack()
		{
			Stop();
			batAttack.Do();
		}

		private void StopCharging()
		{
			SetState(state.not);
			anim.SetBool(UnitAnimations.IsCharging, false);
			UseAllAmmo();
			OnChargeStop?.Invoke();
			Stop();
		}

		private void StartAttack()
		{
			UseAllAmmo();
			anim.SetTrigger(UnitAnimations.ChargeAttackTrigger);
			anim.SetBool(UnitAnimations.IsCharging, false);
			PlayAnimationClip(chargeAttackAnimationClip);
			Invoke(nameof(Anim_AttackHit), chargeAttackAnimationClip.length / 2);
			moveAbility.Push(moveAbility.GetMoveAimDir(), SpecialAttackExtraPush);

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
			base.AnimationComplete();
		}

		public override void Stop()
		{
			moveAbility.SetCanMove(true);
			base.Stop();
		}

		private void StartActualCharging()
		{
			SetState(state.charged);
			PlayAnimationClip(chargedAnimationClip);
		}

		private void Anim_AttackHit(int attackType)
		{
			if (attackType != 5) return;
			SpecialAttackHit(attackType);
		}

		private void FullyCharged()
		{
			isFullyCharged = true;
			ammoInventory.secondaryAmmo.SetAmmoReserve(100);
		}

		private void SpecialAttackHit(int attackType)
		{
			var attackPosition = body.AimCenter.transform.position;
			var bestTargetPoint = GetBestTargetPoint(attackPosition);
			var connect = false;

			SpecialAttackDistance = Vector2.Distance(attackPosition, bestTargetPoint);


			var raycastHits = Physics2D.CircleCastAll(attackPosition, SpecialAttackWidth, moveAbility.GetMoveAimDir(), SpecialAttackDistance,
				life.EnemyLayer);
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
				AttackUtilities.HitTarget(life, otherLife, hit2D.ClosestPoint(body.AttackStartPoint.transform.position), otherLife.SecondaryAttackDamageWithExtra, SpecialAttackExtraPush);
				connect = true;
				Services.objectMaker.Make(Services.assetManager.FX.hits.GetRandom(), hit2D.transform.position);
			}

			if (!connect) return;
			OnAttackHit?.Invoke();
		}

		private Vector2 GetBestTargetPoint(Vector3 attackPosition) => moveAbility.IsIdle() ? aimAbility.GetAimPoint() : (Vector2) attackPosition + moveAbility.GetLastMoveAimDirOffset();

		private float GetHitRange(int attackType)
		{
			if (attackType == 5)
				return life.PrimaryAttackRange * 2;
			return life.PrimaryAttackRange;
		}
	}
}
