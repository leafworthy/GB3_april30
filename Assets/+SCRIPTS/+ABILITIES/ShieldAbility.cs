using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class ShieldAbility : MonoBehaviour, INeedPlayer, IActivity
{
	public class ShieldDashActivity : IActivity
	{
		public string AbilityName => "Shield-Dash";


	}

	private AnimationEvents animEvents;
	private MoveAbility moveAbility;
	private Life life;
	private Player owner;
	private SimpleJumpAbility simpleJumps;
	private Body body;
	private UnitAnimations anim;
	private bool isDashing;
	private bool isShielding;
	public AnimationClip shieldOutClip;
	public string AbilityName => "Shield";
	private ShieldDashActivity shieldDashActivity = new ShieldDashActivity();
	private float counter;
	private float counterMax = .25f;



	public void SetPlayer(Player _player)
	{
		anim = GetComponent<UnitAnimations>();
		body = GetComponent<Body>();
		simpleJumps = GetComponent<SimpleJumpAbility>();
		moveAbility = GetComponent<MoveAbility>();

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
		var hits = Physics2D.OverlapCircleAll(transform.position, 30, Services.assetManager.LevelAssets.EnemyLayer);
		foreach (var hit in hits)
		{
			var _life = hit.GetComponent<Life>();
			if (_life != null && _life.IsEnemyOf(life))
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
	}

	private void CancelShielding(NewControlButton obj)
	{
		SetShielding(false);
	}

	private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
	{
		if (!moveAbility.GetCanMove()) return;
		if (Services.pauseManager.IsPaused) return;
		if (!simpleJumps.isResting) return;


		if (!isShielding)
		{
			if (!body.arms.Do(this))
			{
				return;
			}

			SetShielding(true);

			if (!moveAbility.IsIdle())
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
		if (moveAbility.IsIdle())
		{
			CancelShielding(null);
		}
		else
		{
			if (!body.legs.Do(shieldDashActivity)) return;

			SetDashing(true);
			moveAbility.Push(moveAbility.GetMoveDir(), life.DashSpeed);
			ShieldPush();
		}
	}

	private void SetDashing(bool isOn)
	{

		isDashing = isOn;
		if (isOn) anim.SetTrigger(UnitAnimations.DashTrigger);
		else body.legs.Stop(shieldDashActivity);
	}

	private void SetShielding(bool isOn)
	{

		if (!isOn)
		{
			body.arms.Stop(this);
		}
		isShielding = isOn;
		anim.SetBool(UnitAnimations.IsShielding, isOn);
		life.SetShielding(isOn);

	}


	private void Anim_DashStop()
	{
		SetDashing(false);
		// Also clear shielding state when dash completes
		SetShielding(false);
	}


}
