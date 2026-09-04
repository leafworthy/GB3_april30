using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : Ability
	{
		public AnimationClip dashAnimationClip_Bottom;
		public AnimationClip dashAnimationClip_Top;
		MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		MoveAbility _moveAbility;

		JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		JumpAbility _jumps;

		public event Action OnDash;

		public override string AbilityName => "Dash";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		public override bool canDo() => base.canDo() && jumps.IsResting;

		public override bool canStop(IDoableAbility abilityToStopFor) => abilityToStopFor is JumpAbility;

		public override void StopAbility()
		{
			StopBody();
			StopDashing();
			defence.SetTemporarilyInvincible(false);
			if (lastArmAbility is IGunAttack)
			{
				StopBody();
				lastArmAbility?.Resume();
			}
			else
			{
				StopBody();
				lastArmAbility?.TryToActivate();
			}
		}

		protected void StopDashing()
		{
			body.ChangeLayer(Body.BodyLayer.grounded);
			moveAbility.StopPush();
			anim.RevertBottomToDefault();
		}

		protected override void DoAbility()
		{
			Dash();
		}

		void UnsubscribeFromEvents()
		{
			if (player?.Controller != null)
				player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);

			UnsubscribeFromEvents();

			player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
		}

		void OnDisable()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.DashRightShoulder == null) return;
			player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		}

		void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			TryToActivate();
		}

		protected void Dash()
		{
			if (dashAnimationClip_Bottom != null) PlayAnimationClip(dashAnimationClip_Bottom);
			if (dashAnimationClip_Top != null) PlayAnimationClip(dashAnimationClip_Top, 1);
			OnDash?.Invoke();
			defence.SetTemporarilyInvincible(true);

			moveAbility.Push(moveAbility.GetMoveDir(), offence.stats.Stats.DashSpeed);
		}
	}
}
