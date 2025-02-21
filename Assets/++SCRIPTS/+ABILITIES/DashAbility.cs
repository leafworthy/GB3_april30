using UnityEngine;

public class DashAbility : MonoBehaviour
{
	private AnimationEvents animEvents;
	private MoveController moveController;
	private Life life;
	private Player owner;
	private JumpAbility jumps;
	private Body body;
	private Animations anim;
	private string DashVerbName = "dashing";
	private AimAbility aim;
	public bool teleport;

	private void Start()
	{
		aim = GetComponent<AimAbility>();
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
		if (owner == null) return;
		if(owner != null)owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		animEvents.OnDashStop -= Anim_DashStop;
		animEvents.OnTeleport -= Anim_Teleport;
		owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
	
	}

	private void Anim_Teleport()
	{
		
		var landable = body.GetLandableAtPosition(aim.GetAimPoint());
		body.ChangeLayer(landable != null ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
		if (landable != null)body.SetDistanceToGround(landable.height);
		transform.position = aim.GetAimPoint();
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
		if (GlobalManager.IsPaused) return;
		if (!jumps.isResting) return;
		body.arms.Stop();
		if (!body.arms.Do(DashVerbName)) return;
		//if (!body.legs.Do(DashVerbName)) return;
		body.legs.Stop();
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