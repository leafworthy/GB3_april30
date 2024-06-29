using UnityEngine;

public class Dasher : MonoBehaviour
{
	private AnimationEvents animEvents;
	private MoveController moveController;
	private UnitStats stats;
	private Player owner;
	private JumpAbility jumps;
	private Body body;
	private Animations anim;
	private string DashVerbName = "dashing";
	private GunAimer aim;
	public bool teleport;

	private void Start()
	{
		aim = GetComponent<GunAimer>();
		anim = GetComponent<Animations>();
		body = GetComponent<Body>();
		jumps = GetComponent<JumpAbility>();
		moveController = GetComponent<MoveController>();

		stats = GetComponent<UnitStats>();
		owner = stats.player;
		owner.Controller.Dash.OnPress += ControllerDashPress;

		animEvents = anim.animEvents;
		animEvents.OnDash += Anim_Dash;
		animEvents.OnDashStop += Anim_DashStop;
		animEvents.OnTeleport += Anim_Teleport;
	}

	private void Anim_Teleport()
	{
		ASSETS.sounds.brock_teleport_sounds.PlayRandom();
		var landable = body.GetLandableAtPosition(aim.GetAimPoint(3));
		body.ChangeLayer(landable != null ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
		if (landable != null)body.SetDistanceToGround(landable.height);
		transform.position = aim.GetAimPoint(3);
	}

	private void Anim_DashStop()
	{
		body.ChangeLayer(body.isOverLandable ? Body.BodyLayer.landed : Body.BodyLayer.grounded);
		body.legs.Stop(DashVerbName);
		body.arms.Stop(DashVerbName);
	}

	private void Anim_Dash()
	{
		ASSETS.sounds.bean_roll_sounds.PlayRandom();
	}

	private void ControllerDashPress(NewControlButton newControlButton)
	{
		if (!moveController.CanMove) return;
		if (Game_GlobalVariables.IsPaused) return;
		if (!jumps.isResting) return;
		if (!body.legs.Do(DashVerbName)) return;
		if (!body.arms.Do(DashVerbName)) return;
		anim.SetTrigger(Animations.DashTrigger);
		if (!teleport)
		{
			moveController.Push(moveController.MoveDir, stats.DashSpeed);
			body.ChangeLayer(Body.BodyLayer.grounded);
		}
		else
		{
			body.ChangeLayer(Body.BodyLayer.jumping);
			body.canLand = true;
		}

		
	}
}