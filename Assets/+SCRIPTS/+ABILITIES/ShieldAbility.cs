using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class ShieldAbility : Ability
{
	public override string AbilityName => "Shield";
	public AnimationClip shieldOutClip;
	public AnimationClip shieldAwayClip;
	public AnimationClip shieldingClip;
	public GameObject shieldObject;

	private bool isShielding;
	private bool isPuttingAway;
	private bool isPressingShield;
	private bool isResuming;

	private AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
	private AimAbility _aimAbility;
	protected override bool requiresArms() => true;
	protected override bool requiresLegs() => false;
	public override bool canStop(IDoableAbility newAbility) => true;

	protected override void DoAbility()
	{
		if (isShielding) return;
		shieldObject.SetActive(true);
		isShielding = true;
		life.SetShielding(true);
		PlayAnimationClip(isResuming ? shieldingClip : shieldOutClip, 1);
	}

	private void Update()
	{
		if (canDo() && isPressingShield && !isShielding && !isPuttingAway)
		{
			isResuming = true;
			Do();
		}

		if (isShielding)
		{

				 body.TopFaceDirection(aimAbility.AimDir.x >= 0);

		}
	}

	protected override void AnimationComplete()
	{
		if (isPuttingAway)
		{
			Stop();
			return;
		}

		if (isShielding) return;
		PutShieldAway();
	}

	public override void SetPlayer(Player _player)
	{
		base.SetPlayer(_player);
		player.Controller.InteractRightShoulder.OnPress += Controller_RightShouldPress;
		player.Controller.InteractRightShoulder.OnRelease += Controller_RightShouldRelease;
	}

	private void Controller_RightShouldRelease(NewControlButton obj)
	{
		isPressingShield = false;
		if (!isShielding) return;
		PutShieldAway();
	}

	private void PutShieldAway()
	{
		if (isPuttingAway) return;
		shieldObject.SetActive(false);
		isPuttingAway = true;
		isShielding = false;
		life.SetShielding(false);
		PlayAnimationClip(shieldAwayClip, 1);
	}

	private void OnDestroy()
	{
		if (player == null) return;
		if (player.Controller == null) return;
		player.Controller.InteractRightShoulder.OnPress -= Controller_RightShouldPress;
		player.Controller.InteractRightShoulder.OnRelease -= Controller_RightShouldRelease;
	}

	public override void Stop()
	{
		base.Stop();
		isPuttingAway = false;
		life.SetShielding(false);
		isShielding = false;
	}

	private void Controller_RightShouldPress(NewControlButton newControlButton)
	{
		isPressingShield = true;
		Do();
	}
}
