using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class ChainsawAttack : WeaponAbility
	{
		public override string AbilityName => "Chainsaw-Attack";

		private bool isPressingChainsawButton;
		private AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
		private AimAbility _aimAbility;

		public event Action<Vector2> OnStartChainsawing;
		public event Action<Vector2> OnStartAttacking;
		public event Action<Vector2> OnStopAttacking;
		public event Action<Vector2> OnStopChainsawing;
		private float cooldownCounter;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle || currentState == weaponState.not;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.Attack2LeftTrigger.OnPress += PlayerChainsawPress;
			player.Controller.Attack2LeftTrigger.OnRelease += PlayerChainsawRelease;
		}

		protected override void DoAbility()
		{
			PullOut();
		}

		protected override void PullOut()
		{
			base.PullOut();
			isActive = true;
			anim.SetBool(UnitAnimations.IsChainsawing, true);
			OnStartChainsawing?.Invoke(transform.position);
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			StartAttacking();
		}

		private void PlayerChainsawPress(NewControlButton newControlButton)
		{
			isPressingChainsawButton = true;
			if (!isActive) return;
			StartAttacking();
		}

		private void StartAttacking()
		{
			if (!isPressingChainsawButton || currentState != weaponState.idle) return;
			SetState(weaponState.attacking);
			anim.SetBool(UnitAnimations.IsAttacking, true);
			OnStartAttacking?.Invoke(transform.position);
		}

		private void PlayerChainsawRelease(NewControlButton newControlButton)
		{
			isPressingChainsawButton = false;
			if (!isActive)
			{
				SetState(weaponState.not);
				return;
			}
			StopAttacking();
		}

		private void StopAttacking()
		{
			if (currentState != weaponState.attacking) return;
			anim.SetBool(UnitAnimations.IsAttacking, false);
			OnStopAttacking(transform.position);
			StartIdle();
		}

		private void OnDestroy()
		{
			SetState(weaponState.not);
			isPressingChainsawButton = false;
			if (player == null) return;
			player.Controller.Attack2LeftTrigger.OnPress -= PlayerChainsawPress;
			player.Controller.Attack2LeftTrigger.OnRelease -= PlayerChainsawRelease;
		}

		private void FixedUpdate()
		{
			if (!isActive) return;
			body.TopFaceDirection(aimAbility.AimDir.x > 0);
			switch (currentState)
			{
				case weaponState.idle when isPressingChainsawButton:
					StartAttacking();
					break;
				case weaponState.attacking:
					AttackContinuously();
					break;
			}
		}

		private void AttackContinuously()
		{
			cooldownCounter += Time.fixedDeltaTime;
			if (!(cooldownCounter >= life.TertiaryAttackRate)) return;
			cooldownCounter = 0;
			AttackUtilities.HitTargetsWithinRange(life, body.AttackStartPoint.transform.position, life.TertiaryAttackRange, life.TertiaryAttackDamageWithExtra);
		}

		public override void PutAway()
		{
			base.PutAway();
			anim.SetBool(UnitAnimations.IsChainsawing, false);
			OnStopChainsawing?.Invoke(transform.position);
		}

		public override void Stop()
		{
			SetState(weaponState.not);
			base.Stop();
		}
	}
}
