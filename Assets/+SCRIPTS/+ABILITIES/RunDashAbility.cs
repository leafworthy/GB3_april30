using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class RunDashAbility : Ability
	{
		public event Action OnDash;
		public AnimationClip dashAnimationClip_Bottom;
		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private Life life => _life ??= GetComponent<Life>();
		private Life _life;
		private JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		private JumpAbility _jumps;

		public override string AbilityName => "Dash";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		public override bool canDo() => base.canDo() && jumps.IsResting;

		public override bool canStop(IDoableAbility abilityToStopFor) => abilityToStopFor is JumpAbility;

		public override void StopAbility()
		{
			StopBody();
			StopDashing();
			life.Stats.ExtraSpeedFactor = 1;
			defence.SetTemporarilyInvincible(false);
			if (lastArmAbility is GunAttack)
			{
				Debug.Log("Resuming last arm ability", this);
				lastArmAbility?.Resume();
			}
			else
			{
				Debug.Log("Trying to activate last arm ability", this);
				lastArmAbility?.TryToActivate();
			}
		}

		protected void StopDashing()
		{
			body.ChangeLayer(Body.BodyLayer.grounded);
			moveAbility.StopPush();
			anim.RevertBottomToDefault();
			anim.SetBool(UnitAnimations.IsDashing, false);
		}

		protected override void DoAbility()
		{
			Dash();
		}

		private void UnsubscribeFromEvents()
		{
			if (player?.Controller != null)
				player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
			player.Controller.DashRightShoulder.OnRelease -= ControllerDashRightShoulderRelease;
		}

		void ControllerDashRightShoulderRelease(NewControlButton obj)
		{
			StopAbility();
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);

			UnsubscribeFromEvents();

			player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
			player.Controller.DashRightShoulder.OnRelease += ControllerDashRightShoulderRelease;
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.DashRightShoulder == null) return;
			UnsubscribeFromEvents();
		}

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			TryToActivate();
		}

		protected override void AnimationComplete()
		{

		}

		protected void Dash()
		{
			if (dashAnimationClip_Bottom != null) PlayAnimationClip(dashAnimationClip_Bottom);
			anim.SetBool(UnitAnimations.IsDashing, true);
			defence.SetTemporarilyInvincible(true);
			OnDash?.Invoke();
			life.Stats.ExtraSpeedFactor = 2;
			moveAbility.Push(moveAbility.GetMoveDir(), offence.stats.Stats.DashSpeed);
		}
	}
}
