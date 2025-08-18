using UnityEngine;

namespace __SCRIPTS
{
	[DisallowMultipleComponent]
	public class DoableDashAbility : DoableActivity
	{
		public AnimationClip dashAnimationClip;
		private JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		private JumpAbility _jumpAbility;

		public bool teleport;
		public override string VerbName => "Dash";

		protected override bool requiresArms() => false;
		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && jumpAbility.canDo();

		public override bool canStop() => false;

		public override void StartActivity()
		{
			Dash();
		}

		public void Teleport()
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

		protected override void AnimationComplete()
		{
			body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			StopActivity();
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
