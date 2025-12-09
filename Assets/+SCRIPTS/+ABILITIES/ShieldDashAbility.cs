using System;
using __SCRIPTS;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldDashAbility : DashAbility
{
	public event Action OnShieldDash;
	float extraDashPushFactor = 3f;
	public override string AbilityName => "Shield-Dash";
	ShieldAbility shieldAbility => _shieldAbility ??= GetComponent<ShieldAbility>();
	ShieldAbility _shieldAbility;

	protected override void AnimationComplete()
	{
		defence.SetTemporarilyInvincible(false);
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
			shieldAbility.SetShielding(lastArmAbility is ShieldAbility);

			StopBody();
			lastArmAbility?.Resume();
		}
		else
		{
			shieldAbility.SetShielding(false);
			StopBody();
			lastArmAbility?.Try();
		}
	}

	void ShieldDash()
	{
		var hits = Physics2D.OverlapCircleAll(transform.position, 30, offence.EnemyLayer);
		shieldAbility.SetShielding(true);
		foreach (var hit in hits)
		{
			var _life = hit.GetComponentInParent<Life>();
			if (_life == null || !_life.IsEnemyOf(defence)) continue;
			var movement = _life.GetComponent<MoveAbility>();
			if (movement == null) continue;

			OnShieldDash?.Invoke();
			defence.SetTemporarilyInvincible(true);
			movement.Push((hit.transform.position - transform.position).normalized, offence.stats.DashSpeed * extraDashPushFactor);
		}
	}

	bool hasInitialized;

	public override void SetPlayer(Player newPlayer)
	{
		player = newPlayer;
		player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
	}

	void OnDestroy()
	{
		if (player == null) return;
		player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
	}

	void OnDisable()
	{
		if (player == null) return;
		player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
	}

	void ControllerDashRightShoulderPress(NewControlButton newControlButton)
	{
		Try();
	}
}
