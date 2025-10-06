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

	public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle || abilityToStopFor is ShieldDashAbility;

	protected override void PullOut()
	{
		base.PullOut();
		SetShielding(true);
	}

	private void Update()
	{
		if (isActive) body.TopFaceDirection(aimAbility.AimDir.x >= 0);
	}

	public void SetShielding(bool isOn)
	{
		shieldObject.SetActive(isOn);
		//anim.SetBool(UnitAnimations.IsShielding, isOn);
		life.SetShielding(isOn);
		isActive = isOn;
	}

	public override void Stop()
	{
		SetShielding(false);
		base.Stop();
	}
}
