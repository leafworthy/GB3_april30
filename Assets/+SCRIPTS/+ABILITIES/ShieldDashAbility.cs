using System;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldDashAbility : DashAbility
{
	public event Action OnShieldDash;
	private float extraDashPushFactor = 3f;
	public override string AbilityName => "Shield-Dash";
	private ShieldAbility shieldAbility => _shieldAbility ??= GetComponent<ShieldAbility>();
	private ShieldAbility _shieldAbility;


	protected override void AnimationComplete()
	{
		life.SetTemporarilyInvincible(false);
		base.AnimationComplete();
	}

	protected override void DoAbility()
	{
		base.DoAbility();
		ShieldDash();
	}

	public override void Stop()
	{
		StopDashing();

		if (lastArmAbility is ShieldAbility or GunAttack)
		{
			Debug.Log( "Resuming Ability");
			if (lastArmAbility is ShieldAbility)
			{
				shieldAbility.SetShielding(true);
			}
			else
			{
				shieldAbility.SetShielding(false);
			}

			StopBody();
			lastArmAbility?.Resume();
		}
		else
		{
			shieldAbility.SetShielding(false);
			StopBody();
			lastArmAbility?.Do();
		}
	}

	private void ShieldDash()
	{
		var hits = Physics2D.OverlapCircleAll(transform.position, 30, life.EnemyLayer);
		shieldAbility.SetShielding(true);
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

			OnShieldDash?.Invoke();
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


		Do();
	}


}
