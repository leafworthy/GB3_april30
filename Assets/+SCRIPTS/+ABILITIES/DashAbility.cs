using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : Ability
	{
		public AnimationClip dashAnimationClip_Bottom;
		public AnimationClip dashAnimationClip_Top;
		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		private JumpAbility _jumps;

		public override string AbilityName => "Dash";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		public override bool canDo() => base.canDo() && jumps.IsResting;

		public override bool canStop(IDoableAbility abilityToStopFor) => abilityToStopFor is JumpAbility;

		public override void Stop()
		{
			StopBody();
			StopDashing();
			life.SetTemporarilyInvincible(false);
			if (lastArmAbility is GunAttack)
			{
				StopBody();
				lastArmAbility?.Resume();
			}
			else
			{
				StopBody();
				lastArmAbility?.Do();
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

		private void UnsubscribeFromEvents()
		{
			if (player?.Controller != null)
				player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);

			UnsubscribeFromEvents();

			player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
		}

		private void OnDestroy()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		}

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			Do();
		}



		protected void Dash()
		{
			if (dashAnimationClip_Bottom != null) PlayAnimationClip(dashAnimationClip_Bottom);
			if (dashAnimationClip_Top != null) PlayAnimationClip(dashAnimationClip_Top, 1);
			life.SetTemporarilyInvincible(true);

			moveAbility.Push(moveAbility.GetMoveDir(), life.DashSpeed);
		}
	}
}
