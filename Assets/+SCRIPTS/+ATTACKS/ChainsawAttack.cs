using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class ChainsawAttack : WeaponAbility
	{
		public override string AbilityName => "Chainsaw-Attack";

		bool isPressingChainsawButton;
		AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
		AimAbility _aimAbility;

		public event Action<Vector2> OnStartChainsawing;
		public event Action<Vector2> OnStartAttacking;
		public event Action<Vector2> OnStopAttacking;
		public event Action<Vector2> OnStopChainsawing;
		float cooldownCounter;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle || currentState == weaponState.not;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			player.Controller.Attack2LeftTrigger.OnPress += PlayerChainsawPress;
			player.Controller.Attack3Circle.OnPress += PlayerChainsawPress;
			player.Controller.Attack2LeftTrigger.OnRelease += PlayerChainsawRelease;
			player.Controller.Attack3Circle.OnRelease += PlayerChainsawRelease;
		}

		protected override void DoAbility()
		{
			if (currentState != weaponState.resuming) PullOut();
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

		void PlayerChainsawPress(NewControlButton newControlButton)
		{
			isPressingChainsawButton = true;
			if (!isActive) return;
			StartAttacking();
		}

		void StartAttacking()
		{
			if (!isPressingChainsawButton || currentState != weaponState.idle) return;
			SetState(weaponState.attacking);
			anim.SetBool(UnitAnimations.IsAttacking, true);
			OnStartAttacking?.Invoke(transform.position);
		}

		void PlayerChainsawRelease(NewControlButton newControlButton)
		{
			isPressingChainsawButton = false;
			if (!isActive)
			{
				SetState(weaponState.not);
				return;
			}

			StopAttacking();
		}

		void StopAttacking()
		{
			if (currentState != weaponState.attacking) return;
			anim.SetBool(UnitAnimations.IsAttacking, false);
			OnStopAttacking(transform.position);
			StartIdle();
		}

		void OnDestroy()
		{
			SetState(weaponState.not);
			isPressingChainsawButton = false;
			if (player == null) return;
			player.Controller.Attack2LeftTrigger.OnPress -= PlayerChainsawPress;
			player.Controller.Attack2LeftTrigger.OnRelease -= PlayerChainsawRelease;
		}

		void FixedUpdate()
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

		void AttackContinuously()
		{

			cooldownCounter += Time.fixedDeltaTime;
			if (!(cooldownCounter >= offence.stats.TertiaryAttackRate)) return;
			cooldownCounter = 0;
			AttackUtilities.HitTargetsWithinRange(offence, body.AttackStartPoint.transform.position, offence.stats.TertiaryAttackRange,
				offence.stats.TertiaryAttackDamageWithExtra);
		}

		public override void Stop()
		{
			anim.SetBool(UnitAnimations.IsChainsawing, false);
			OnStopChainsawing?.Invoke(transform.position);
			SetState(weaponState.not);
			base.Stop();
		}
	}
}
