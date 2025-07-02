using __SCRIPTS;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

public class ShieldAbility : MonoBehaviour, INeedPlayer, IActivity
{
	public class ShieldDashActivity : IActivity
	{
		public string VerbName => "Shield-Dash";

		public bool TryCompleteGracefully(GangstaBean.Core.CompletionReason reason, GangstaBean.Core.IActivity newActivity = null)
		{
			switch (reason)
			{
				case GangstaBean.Core.CompletionReason.AnimationInterrupt:
				case GangstaBean.Core.CompletionReason.NewActivity:
					return true;
			}
			return false;
		}
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
	private float counter;
	private float counterMax = .25f;

	public bool TryCompleteGracefully(GangstaBean.Core.CompletionReason reason, GangstaBean.Core.IActivity newActivity = null)
	{
		switch (reason)
		{
			case GangstaBean.Core.CompletionReason.AnimationInterrupt:
				SetShielding(false);
				SetDashing(false);
				return true;
			case GangstaBean.Core.CompletionReason.NewActivity:
				if (newActivity?.VerbName == "Shooting" || newActivity?.VerbName == "Dash")
				{
					SetShielding(false);
					SetDashing(false);
					return true;
				}
				break;
		}
		return false;
	}

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



	private void ShieldPush()
	{
		var hits = Physics2D.OverlapCircleAll(transform.position, 30, ASSETS.LevelAssets.EnemyLayer);
		foreach (var hit in hits)
		{
			var _life = hit.GetComponent<Life>();
			if (_life != null && _life.isEnemyOf(life))
			{

				var movement = hit.GetComponent<MoveAbility>();
				if (movement == null) return;
				movement.Push((hit.transform.position - transform.position).normalized, life.DashSpeed*1.5f);
			}
		}
	}

	private void OnDisable()
	{
		if (owner == null) return;
		if (owner.Controller == null) return;
		if (animEvents != null) animEvents.OnDashStop -= Anim_DashStop;
		owner.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
		owner.Controller.Attack1RightTrigger.OnPress -= CancelShielding;
		owner.Controller.Jump.OnPress -= StartJump;

		// Clean up any stuck shield/dash states
		if (isShielding || isDashing)
		{

			SetShielding(false);
			SetDashing(false);
		}
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

		// Check if character has teleport dash ability - if so, let that take priority
		var dashAbility = GetComponent<DashAbility>();
		if (dashAbility != null && dashAbility.teleport)
		{

			return;
		}

		if (!isShielding)
		{
			if (!body.arms.Do(this))
			{
				return;
			}

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
			ShieldPush();
		}
	}

	private void SetDashing(bool isOn)
	{

		isDashing = isOn;
		if (isOn) anim.SetTrigger(Animations.DashTrigger);
		else body.legs.StopSafely(shieldDashActivity);
	}

	private void SetShielding(bool isOn)
	{

		if (!isOn)
		{
			body.arms.StopSafely(this);
		}
		isShielding = isOn;
		anim.SetBool(Animations.IsShielding, isOn);
		life.SetShielding(isOn);

	}


	private void Anim_DashStop()
	{
		SetDashing(false);
		// Also clear shielding state when dash completes
		SetShielding(false);
	}


}
