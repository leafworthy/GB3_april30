using System;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class ShieldAbility : WeaponAbility
{
	public override string AbilityName => "Shield";
	public GameObject shieldObject;
	private AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
	private AimAbility _aimAbility;
	protected override bool requiresArms() => true;
	protected override bool requiresLegs() => false;

	public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle;

	protected override void PullOut()
	{
		base.PullOut();
		SetShielding(true);
	}

	public override void PutAway()
	{
		base.PutAway();
		SetShielding(false);
	}
	private void Update()
	{
		if (isActive) body.TopFaceDirection(aimAbility.AimDir.x >= 0);
	}


	private void SetShielding(bool isOn)
	{
		shieldObject.SetActive(isOn);
		anim.SetBool(UnitAnimations.IsShielding, isOn);
		life.SetShielding(isOn);
		isActive = isOn;
	}

	public override void Stop()
	{
		life.SetShielding(false);
		isActive = false;
		base.Stop();
	}


}
