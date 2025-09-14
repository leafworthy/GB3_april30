using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : Ability
	{
		public AnimationClip dashAnimationClip_Bottom;
		public AnimationClip dashAnimationClip_Top;
		private AnimationEvents animEvents;
		private MoveAbility moveAbility  => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;
		private Player owner;
		private CharacterJumpAbility jumps;

		public bool teleport;

		public override string AbilityName => "Dash";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		public override bool canDo()
		{
			if (!base.canDo())
			{
				Debug.Log("Base canDo failed for " + AbilityName + " ability");
				return false;
			}

			if (!jumps.IsResting)
			{
				Debug.Log("Cannot do jumps, cannot do " + AbilityName + " ability");
				return false;
			}
			return true;
		}

		public override bool canStop() => false;

		public override void Stop()
		{
			base.Stop();
			body.ChangeLayer(Body.BodyLayer.grounded);
			moveAbility.StopPush();
			anim.RevertBottomToDefault();
			Debug.Log("interrupt");
		}

		protected override void DoAbility()
		{
			Dash();
		}

		private void UnsubscribeFromEvents()
		{
			// These won't throw exceptions even if not subscribed
			if (owner?.Controller != null)
				owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;

			if (animEvents != null)
				animEvents.OnTeleport -= Anim_Teleport;
		}
		public override void SetPlayer(Player _player)
		{

			jumps = GetComponent<CharacterJumpAbility>();

			owner = _player;
			UnsubscribeFromEvents();

			animEvents = anim.animEvents;
			owner.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
			animEvents.OnTeleport += Anim_Teleport;
		}

		private void OnDisable()
		{
			if (owner == null) return;
			if (owner.Controller == null) return;
			if (animEvents != null) animEvents.OnTeleport -= Anim_Teleport;
			owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
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
			Do();
		}

		protected override void AnimationComplete()
		{

			Stop();
		}

		private void Dash()
		{
			if(dashAnimationClip_Bottom != null)PlayAnimationClip(dashAnimationClip_Bottom);
			if (dashAnimationClip_Bottom != null) PlayAnimationClip(dashAnimationClip_Top,1);
			Debug.Log("Dashing with " + AbilityName + " ability");
			if (!teleport)
			{
				moveAbility.Push(moveAbility.GetMoveDir(), life.DashSpeed);
				body.ChangeLayer(Body.BodyLayer.grounded);
			}
			else
			{
				body.ChangeLayer(Body.BodyLayer.jumping);
			}
		}
	}
}
