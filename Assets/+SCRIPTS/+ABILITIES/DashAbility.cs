using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : Ability
	{
		public AnimationClip dashAnimationClip_Bottom;
		public AnimationClip dashAnimationClip_Top;
		private AnimationEvents animEvents;
		protected MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private CharacterJumpAbility jumps  => _jumps ??= GetComponent<CharacterJumpAbility>();
		private CharacterJumpAbility _jumps;

		public bool teleport;

		public override string AbilityName => "Dash";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		public override bool canDo() => base.canDo() && jumps.IsResting;

		public override bool canStop(IDoableAbility abilityToStopFor)
		{
			if (abilityToStopFor is CharacterJumpAbility) return true;

			return false;
		}

		public override void Stop()
		{
			base.Stop();
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
			// These won't throw exceptions even if not subscribed
			if (player?.Controller != null)
				player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;

			if (animEvents != null)
				animEvents.OnTeleport -= Anim_Teleport;
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);

			Debug.LogWarning("Set player in dash");
			UnsubscribeFromEvents();

			animEvents = anim.animEvents;
			player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
			animEvents.OnTeleport += Anim_Teleport;
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (animEvents != null) animEvents.OnTeleport -= Anim_Teleport;
			player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		}

		private void Anim_Teleport()
		{
			var teleportDirection = moveAbility.GetMoveDir();
			if (teleportDirection.magnitude < 0.1f)
			{
				teleportDirection = moveAbility.GetLastMoveAimDirOffset().normalized;
				if (teleportDirection.magnitude < 0.1f) teleportDirection = body.BottomIsFacingRight ? Vector2.right : Vector2.left;
			}

			var teleportDistance = life.DashSpeed;
			var teleportOffset = teleportDirection.normalized * teleportDistance;
			var newPoint = (Vector2) transform.position + teleportOffset;

			body.ChangeLayer(Body.BodyLayer.grounded);
			transform.position = newPoint;
		}

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			Debug.LogWarning("dash pressed inside dash");
			Do();
		}

		protected void Dash()
		{
			if (dashAnimationClip_Bottom != null) PlayAnimationClip(dashAnimationClip_Bottom);
			if (dashAnimationClip_Top != null) PlayAnimationClip(dashAnimationClip_Top, 1);

			moveAbility.Push(moveAbility.GetMoveDir(), life.DashSpeed);
		}
	}
}
