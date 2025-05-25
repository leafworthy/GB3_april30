using __SCRIPTS;
using __SCRIPTS.HUD_Displays;
using UnityEngine;

public class ShieldAbility : MonoBehaviour, INeedPlayer, IActivity
{
	public class ShieldDashActivity : IActivity
	{
		public string VerbName => "Shield-Dash";
	}
	private AnimationEvents animEvents;
	private MoveController moveController;
	private Life life;
	private Player owner;
	private JumpAbility jumps;
	private JumpController jumpController;
	private Body body;
	private Animations anim;
	private bool isDashing;
	private bool isShielding;
	public AnimationClip shieldOutClip;
	public string VerbName => "Shield";
	private ShieldDashActivity shieldDashActivity = new ShieldDashActivity();

	public void SetPlayer(Player _player)
	{
		anim = GetComponent<Animations>();
		body = GetComponent<Body>();
		jumps = GetComponent<JumpAbility>();
		jumpController = GetComponent<JumpController>();
		moveController = GetComponent<MoveController>();

		life = GetComponent<Life>();
		owner = _player;

		animEvents = anim.animEvents;
		owner.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
		owner.Controller.Attack1RightTrigger.OnPress += CancelShielding;
		owner.Controller.Jump.OnPress += StartJump;
		animEvents.OnDashStop += Anim_DashStop;
	}

	private void OnDisable()
	{
		if (owner == null) return;
		if (owner.Controller == null) return;
		if (animEvents != null) animEvents.OnDashStop -= Anim_DashStop;
		owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		owner.Controller.Attack1RightTrigger.OnPress -= CancelShielding;
		owner.Controller.Jump.OnPress -= StartJump;
	}
	private void StartJump(NewControlButton obj)
	{
		SetShielding(false);
		jumpController.Jump();
	}

	private void CancelShielding(NewControlButton obj)
	{
		SetShielding(false);
	}

	private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
	{
		if (!moveController.CanMove) return;
		if (PauseManager.I.IsPaused) return;
		if (!jumps.isResting) return;

		if (!isShielding)
		{
			if (!body.arms.Do(this))
			{
				return;
			}
			Debug.Log("shielding start", this);
			SetShielding(true);

			if (!moveController.IsIdle())
			{
				ShieldDash();
			}
			else
			{
				anim.Play(shieldOutClip.name, 1, 0);
			}
		}
		else
		{
			Debug.Log("shielding dash", this);
			ShieldDash();
		}
	}

	private void ShieldDash()
	{
		if (!isShielding) return;
		if (isDashing) return;
		if (moveController.IsIdle())
		{
			CancelShielding(null);
		}
		else
		{
			if (!body.legs.Do(shieldDashActivity)) return;

			SetDashing(true);
			moveController.Push(moveController.MoveDir, life.DashSpeed);
		}
	}

	private void SetDashing(bool isOn)
	{
		Debug.Log("dashing " + isOn, this);
		isDashing = isOn;
		if (isOn) anim.SetTrigger(Animations.DashTrigger);
		else body.legs.StopSafely(shieldDashActivity);
	}

	private void SetShielding(bool isOn)
	{
		Debug.Log(  "shielding " + isOn, this);
		if (!isOn)
		{
			body.arms.StopSafely(this);
		}
		isShielding = isOn;
		anim.SetBool(Animations.IsShielding, isOn);

	}


	private void Anim_DashStop()
	{
		SetDashing(false);
	}


}
