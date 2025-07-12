using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableDashAbility : ServiceAbility, INeedPlayer
	{
		public AnimationClip dashAnimationClip;
		private AnimationEvents animEvents;
		private MoveController moveController;
		private Life life;
		private MoveAbility move;
		private Player owner;
		private DoableJumpAbility jumps;

		public bool teleport;
		public override string VerbName => "Dash";

		public override bool requiresArms => false;
		public override bool requiresLegs => true;

		public override bool canDo()
		{
			if (!base.canDo())
			{
				Debug.Log("Base canDo failed for " + VerbName + " ability");
				return false;
			}
			if (pauseManager.IsPaused)
			{
				Debug.Log("Paused, cannot do " + VerbName + " ability");
				return false;
			}
			if (!jumps.canDo())
			{
				Debug.Log("Cannot do jumps, cannot do " + VerbName + " ability");
				return false;
			}
			if (body.doableArms.IsActive)
			{
				Debug.Log("Arms are active, cannot do " + VerbName + " ability");
				return false;
			}
			return true;
		}

		public override bool canStop() => false;

		public override void Do()
		{
			Debug.Log("do");
			Dash();
		}

		public void SetPlayer(Player _player)
		{
			move = GetComponent<MoveAbility>();

			jumps = GetComponent<DoableJumpAbility>();
			moveController = GetComponent<MoveController>();

			life = GetComponent<Life>();
			owner = _player;

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
			Stop(this);
		}

		private void Anim_Teleport()
		{
			var teleportDirection = moveController.MoveDir;
			if (teleportDirection.magnitude < 0.1f)
			{
				teleportDirection = move.GetLastMoveAimDirOffset().normalized;
				if (teleportDirection.magnitude < 0.1f) teleportDirection = body.BottomIsFacingRight ? Vector2.right : Vector2.left;
			}

			var teleportDistance = life.DashSpeed;
			var teleportOffset = teleportDirection.normalized * teleportDistance;
			var newPoint = (Vector2) transform.position + teleportOffset;

			var landable = body.GetLandableAtPosition(newPoint);
			body.ChangeLayer(landable != null ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			if (landable != null) body.SetDistanceToGround(landable.height);
			transform.position = newPoint;
		}

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			if (!canDo())
			{
				Debug.Log("cant do" + VerbName + " ability" );
				return;
			}
			Dash();
		}

		protected override void AnimationComplete()
		{
			body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			Stop(this);
		}

		private void Dash()
		{
			PlayAnimationClip(dashAnimationClip);
			Debug.Log("Dashing with " + VerbName + " ability");
			if (!teleport)
			{
				moveController.Push(moveController.MoveDir, life.DashSpeed);
				body.ChangeLayer(Body.BodyLayer.grounded);
			}
			else
			{
				body.ChangeLayer(Body.BodyLayer.jumping);
				body.canLand = true;
			}
		}
	}
}
