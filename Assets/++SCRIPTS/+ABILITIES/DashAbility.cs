using UnityEngine;

namespace __SCRIPTS
{
	public class DashAbility : MonoBehaviour
	{
		private AnimationEvents animEvents;
		private MoveController moveController;
		private Life life;
		private MoveAbility move;
		private Player owner;
		private JumpAbility jumps;
		private Body body;
		private Animations anim;
		private string DashVerbName = "dashing";
		public bool teleport;

		private void Start()
		{
			move = GetComponent<MoveAbility>();
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			jumps = GetComponent<JumpAbility>();
			moveController = GetComponent<MoveController>();

			life = GetComponent<Life>();
			owner = life.player;
			owner.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;

			animEvents = anim.animEvents;
			animEvents.OnDashStop += Anim_DashStop;
			animEvents.OnTeleport += Anim_Teleport;
		}

		private void OnDisable()
		{
			if (animEvents != null)
			{
				animEvents.OnDashStop -= Anim_DashStop;
				animEvents.OnTeleport -= Anim_Teleport;
			}
			if (owner == null) return;
			if (owner.Controller == null) return;
			owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
			owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		
	
		}

		private void Anim_Teleport()
		{
			var newPoint = (Vector2)transform.position + move.GetLastMoveAimDirOffset();
			var landable = body.GetLandableAtPosition(newPoint);
			body.ChangeLayer(landable != null ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			if (landable != null)body.SetDistanceToGround(landable.height);
			transform.position = newPoint;
		}

		private void Anim_DashStop()
		{
			body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
			body.legs.Stop(DashVerbName);
			body.arms.Stop(DashVerbName);
		}

	

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			if (!moveController.CanMove) return;
			if (PauseManager.IsPaused) return;
			if (!jumps.isResting) return;
			//body.arms.Stop();
			//if (!teleport) body.legs.Stop();
			if (!body.arms.Do(DashVerbName)) return;
			if(!teleport) if (!body.legs.Do(DashVerbName)) return;
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