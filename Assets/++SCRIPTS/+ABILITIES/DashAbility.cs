using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : MonoBehaviour, INeedPlayer, IActivity
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
		}

		private void Anim_Teleport()
		{
			var newPoint = (Vector2) transform.position + move.GetLastMoveAimDirOffset();
			var landable = body.GetLandableAtPosition(newPoint);
			body.ChangeLayer(landable != null ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			if (landable != null) body.SetDistanceToGround(landable.height);
			transform.position = newPoint;
		}

		private void Anim_DashStop()
		{
			body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			body.legs.StopSafely(this);
			body.arms.StopSafely(this);
		}

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			if (!moveController.CanMove) return;
			if (PauseManager.I.IsPaused) return;
			if (!jumps.isResting) return;
			if (!body.arms.Do(this)) return;
			if (!teleport)
			{
				if (!body.legs.Do(this))
					return;
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
		}


	}
}
