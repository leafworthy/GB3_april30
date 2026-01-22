using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : Ability
	{
		public AnimationClip dashAnimationClip_Bottom;
		public AnimationClip dashAnimationClip_Top;
		MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		MoveAbility _moveAbility;

		public override string AbilityName => "Dash";
		public override bool requiresArms() => true;

		public override bool requiresLegs() => true;

		public override bool canDo() => base.canDo() && body.isGrounded;

		public override bool canStop(Ability abilityToStopFor) => abilityToStopFor is JumpAbility;

		public override void StopAbilityBody()
		{
			StopBody();
			StopDashing();
			life.SetTemporarilyInvincible(false);
			StopBody();
			lastArmAbility?.TryToDoAbility();
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

		void OnDestroy()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.DashRightShoulder == null) return;
			player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		}

		void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			TryToDoAbility();
		}

		protected void Dash()
		{
			if (dashAnimationClip_Bottom != null) PlayAnimationClip(dashAnimationClip_Bottom);
			if (dashAnimationClip_Top != null) PlayAnimationClip(dashAnimationClip_Top, 1);
			life.SetTemporarilyInvincible(true);

			moveAbility.Push(moveAbility.GetMoveDir(), attacker.stats.Stats.DashSpeed);
		}
	}
}
