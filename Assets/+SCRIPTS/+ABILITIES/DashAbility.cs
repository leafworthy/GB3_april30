using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : Ability
	{
		public AnimationClip dashAnimationClip;
		private AnimationEvents animEvents;
		private MoveAbility moveAbility  => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;
		private Player owner;
		private DoableJumpAbility jumps;

		public bool teleport;
		public override string VerbName => "Dash";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => true;

		public override bool canDo()
		{
			if (!base.canDo())
			{
				Debug.Log("Base canDo failed for " + VerbName + " ability");
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

		public override bool canStop() => true;


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

			jumps = GetComponent<DoableJumpAbility>();

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
			//Stop(this);
		}

		private void Anim_Teleport()
		{
			var teleportDirection = moveAbility.MoveDir;
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
			body.ChangeLayer(Body.BodyLayer.grounded);
			moveAbility.StopPush();
			Stop();
		}

		private void Dash()
		{
			PlayAnimationClip(dashAnimationClip);
			Debug.Log("Dashing with " + VerbName + " ability");
			if (!teleport)
			{
				moveAbility.Push(moveAbility.MoveDir, life.DashSpeed);
				body.ChangeLayer(Body.BodyLayer.grounded);
			}
			else
			{
				body.ChangeLayer(Body.BodyLayer.jumping);
			}
		}
	}
}
