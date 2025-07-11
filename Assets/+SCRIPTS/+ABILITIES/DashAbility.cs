using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : ServiceUser, INeedPlayer, IActivity
	{
		private AnimationEvents animEvents;
		private MoveController moveController;
		private Life life;
		private MoveAbility move;
		private Player owner;
		private JumpAbility jumps;
		private Body body;
		private Animations anim;
		public string VerbName => "Dash";
		public bool teleport;
		private string verbName;

		public bool TryCompleteGracefully(GangstaBean.Core.CompletionReason reason, GangstaBean.Core.IActivity newActivity = null)
		{
			switch (reason)
			{
				case GangstaBean.Core.CompletionReason.AnimationInterrupt:
					CancelInvoke(nameof(SafetyArmRelease));
					RefreshAimingSystem();
					return true;
				case GangstaBean.Core.CompletionReason.NewActivity:
					if (newActivity?.VerbName == "Shooting")
					{
						CancelInvoke(nameof(SafetyArmRelease));
						return true;
					}
					break;
			}
			return false;
		}

		public void SetPlayer(Player _player)
		{
			move = GetComponent<MoveAbility>();
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			jumps = GetComponent<JumpAbility>();
			moveController = GetComponent<MoveController>();

			life = GetComponent<Life>();
			owner = _player;

			animEvents = anim.animEvents;
			owner.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
			animEvents.OnDashStop += Anim_DashStop;
			animEvents.OnTeleport += Anim_Teleport;
		}

		private void OnDisable()
		{
			if (owner == null) return;
			if (owner.Controller == null) return;
			if (animEvents != null)
			{
				animEvents.OnDashStop -= Anim_DashStop;
				animEvents.OnTeleport -= Anim_Teleport;
			}

			owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;

			// Clean up any pending invokes and ensure arms are freed
			CancelInvoke(nameof(SafetyArmRelease));
			if (body != null && body.arms.currentActivity?.VerbName == VerbName)
			{
				body.arms.Stop(this);
			}
		}

		private void Anim_Teleport()
		{
			// Get the current movement direction for teleport
			Vector2 teleportDirection = moveController.MoveDir;

			// If not moving, use the last movement direction or default to facing direction
			if (teleportDirection.magnitude < 0.1f)
			{
				teleportDirection = move.GetLastMoveAimDirOffset().normalized;
				if (teleportDirection.magnitude < 0.1f)
				{
					// Fallback to facing direction
					teleportDirection = body.BottomIsFacingRight ? Vector2.right : Vector2.left;
				}
			}

			// Calculate teleport distance (adjust this value as needed)
			float teleportDistance = life.DashSpeed; // You can adjust this value
			Vector2 teleportOffset = teleportDirection.normalized * teleportDistance;
			Vector2 newPoint = (Vector2)transform.position + teleportOffset;



			var landable = body.GetLandableAtPosition(newPoint);
			body.ChangeLayer(landable != null ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			if (landable != null) body.SetDistanceToGround(landable.height);
			transform.position = newPoint;
		}

		private void Anim_DashStop()
		{


			// Cancel safety release since animation event fired properly
			CancelInvoke(nameof(SafetyArmRelease));

			body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			body.legs.Stop(this);
			body.arms.Stop(this);

			// Force refresh of aiming system after dash completes
			RefreshAimingSystem();


		}

		private void RefreshAimingSystem()
		{
			// Find and refresh the aiming ability to prevent stuck aim direction
			var aimAbility = GetComponent<AimAbility>();
			if (aimAbility != null && owner != null)
			{


				// Ensure IsShielding is cleared in case ShieldAbility left it stuck
				if (anim != null)
				{
					anim.SetBool(Animations.IsShielding, false);

				}

				// Force the aim direction to update
				if (!owner.isUsingMouse && owner.Controller != null)
				{
					var currentAim = owner.Controller.AimAxis.GetCurrentAngle();


					// Force refresh by setting the aim direction directly if controller has input
					if (currentAim.magnitude > 0.2f)
					{
						aimAbility.AimDir = currentAim.normalized;

					}
				}
			}
		}

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			if (!moveController.CanMove) return;
			if (pauseManager.IsPaused) return;
			if (!jumps.isResting) return;



			if (!body.arms.Do(this)) return;
			if (!teleport)
			{
				if (!body.legs.Do(this))
				{
					// If legs fail but arms succeeded, clean up arms
					body.arms.Stop(this);
					return;
				}
			}

			anim.SetTrigger(Animations.DashTrigger);
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

			// Safety mechanism: ensure arms are freed after animation duration
			Invoke(nameof(SafetyArmRelease), 2f);
		}

		private void SafetyArmRelease()
		{
			if (body.arms.currentActivity?.VerbName == VerbName)
			{

				body.arms.Stop(this);
				RefreshAimingSystem();
			}
		}


	}
}
