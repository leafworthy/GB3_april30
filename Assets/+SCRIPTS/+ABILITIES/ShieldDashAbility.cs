using System;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldDashAbility : DashAbility
{
	public event Action OnShieldDash;
	float extraDashPushFactor = 3f;
	public override string AbilityName => "Shield-Dash";
	ShieldAbility shieldAbility => _shieldAbility ??= GetComponent<ShieldAbility>();
	ShieldAbility _shieldAbility;

	ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>();
	ICanAttack _attacker;

	ChainsawAttack chainsawAttack => _chainsawAttack ??= GetComponent<ChainsawAttack>();
	ChainsawAttack _chainsawAttack;

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

	public override void StopAbility()
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
			lastArmAbility?.TryToActivate();
		}
	}

	void ShieldDash()
	{
		var hits = Physics2D.OverlapCircleAll(chainsawAttack.ChainsawAttackStartPoint.transform.position, 30, offence.EnemyLayer);
		shieldAbility.SetShielding(true);
		OnShieldDash?.Invoke();
		var connected = false;
		foreach (var hit in hits)
		{
			var _life = hit.GetComponentInParent<Life>();
			if (_life == null) continue;
			if (!_life.IsEnemyOf(defence)) continue;
			var enemyMovement = _life.GetComponent<MoveAbility>();
			if (enemyMovement == null) continue;
			connected = true;

			var launchingAttack = Attack.Create(attacker, _life).WithDamage(1).WithFlying().WithExtraPush(50);
			_life.TakeDamage(launchingAttack);
			defence.SetTemporarilyInvincible(true);
		}

		if (connected)
		{
			Services.sfx.sounds.tmato_shield_hit_sounds.PlayRandomAt(transform.position);
			Services.sfx.sounds.tmato_shield_hit_sounds.PlayRandomAt(transform.position);
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
		TryToActivate();
	}
}
