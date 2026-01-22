using System;
using __SCRIPTS;
using UnityEngine;

public class ShieldAbility : WeaponAbility
{
	public override string AbilityName => "Shield";
	public GameObject shieldObject;
	AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
	AimAbility _aimAbility;

	public override bool requiresArms() => true;
	public override bool requiresLegs() => false;

	public override bool canStop(Ability abilityToStopFor) => currentState == weaponState.idle || abilityToStopFor is ShieldDashAbility;

	protected override void PullOutWeapon()
	{
		base.PullOutWeapon();
	}

	protected override void AnimationComplete()
	{
		switch (currentState)
		{
			case weaponState.not:
				break;
			case weaponState.pullOut:
				SetShielding(true);
				break;
			case weaponState.idle:
				break;
			case weaponState.attacking:
				break;
			case weaponState.resuming:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	void Update()
	{
		if (currentState is weaponState.idle or weaponState.attacking) body.TopFaceDirection(aimAbility.AimDir.x >= 0);
	}

	public void SetShielding(bool isOn)
	{
		shieldObject.SetActive(isOn);
		anim.SetBool(UnitAnimations.IsShielding, isOn);
		life.SetShielding(isOn);
		SetState(weaponState.idle);
	}

	public override void StopAbilityBody()
	{
		SetShielding(false);
		StopBody();
	}
}
