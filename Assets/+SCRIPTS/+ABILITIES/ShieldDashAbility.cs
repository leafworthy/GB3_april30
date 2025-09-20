using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldDashAbility : DashAbility
{
	private float extraDashPushFactor = 1.5f;
	public override string AbilityName => "Shield-Dash";
	private IDoableAbility lastAbility;

	private bool isDashing;
	protected override void DoAbility()
	{
		base.DoAbility();
		ShieldDash();
		isDashing = true;
	}

	public override void Stop()
	{
		isDashing = false;
		base.Stop();
		lastAbility?.Do();
	}

	private void ShieldDash()
	{
		var hits = Physics2D.OverlapCircleAll(transform.position, 30, life.EnemyLayer);
		foreach (var hit in hits)
		{
			var _life = hit.GetComponentInParent<Life>();
			if (_life == null || !_life.IsEnemyOf(life))
			{
				continue;
			}
			var movement = _life.GetComponent<MoveAbility>();
			if (movement == null)
			{
				continue;
			}
			movement.Push((hit.transform.position - transform.position).normalized, life.DashSpeed * extraDashPushFactor);

		}
	}

	private bool hasInitialized = false;
	public override void SetPlayer(Player _player)
	{
		player = _player;
		player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
	}

	private void OnDestroy()
	{
		if (player == null) return;
		Debug.Log("ShieldDashAbility OnDestroy");
		player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
	}

	private void OnDisable()
	{
		if (player == null) return;
		Debug.Log("ShieldDashAbility OnDisable");
		player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
	}

	private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
	{

		if (isDashing)
		{
			Debug.LogWarning( "Already Dashing");
			return;
		}

		lastAbility = body.doableArms.CurrentAbility;
		Do();
	}


}
