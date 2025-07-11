using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableDashAbility : ServiceUser, INeedPlayer, IActivity, IDoableActivity
	{
		public AnimationClip dashAnimationClip;
		private AnimationEvents animEvents;
		private MoveController moveController;
		private Life life;
		private MoveAbility move;
		private Player owner;
		private DoableJumpAbility jumps;
		private Body body;
		private Animations anim;
		public bool teleport;
		public string VerbName => "Dash";

		public bool canDo()
		{
			if (!moveController.CanMove) return false;
			if (pauseManager.IsPaused) return false;
			if (!jumps.canDo()) return false;
			return true;
		}

		public bool canStop() => false;

		public bool Do()
		{
			Dash();
			return true;
		}

		public void ForceStop()
		{
			CancelInvoke(nameof(StartResting));
			StartResting();
		}


		public void SetPlayer(Player _player)
		{
			move = GetComponent<MoveAbility>();
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			jumps = GetComponent<DoableJumpAbility>();
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


			body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			body.legs.Stop(this);
			body.arms.Stop(this);



		}



		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			if (!body.doableArms.CanDoerDo(this) || !body.doableLegs.CanDoerDo(this)) return;
			Dash();
			SetActive(true);
			}

		private void SetActive(bool _active)
		{
			if (_active)
			{
				body.doableArms.DoSafely(this);
				body.doableLegs.DoSafely(this);
			}
			else
			{
				body.doableArms.Stop(this);
				body.doableLegs.Stop(this);
			}
		}
		public void StartResting()
		{
			SetActive(false);
		}
		private void Dash()
		{
			anim.Play(dashAnimationClip.name,0,0);
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

			Invoke(nameof(StartResting), dashAnimationClip.length);
		}




	}
}
