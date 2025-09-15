using __SCRIPTS;
using UnityEngine;

public class ShieldAbility : Ability
{
	public override string AbilityName => "Shield";
	public AnimationClip shieldOutClip;
	private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
	private MoveAbility _moveAbility;
	private CharacterJumpAbility jumpAbility => _jumpAbility ??= GetComponent<CharacterJumpAbility>();
	private CharacterJumpAbility _jumpAbility;

	private bool isShielding;
	protected override bool requiresArms() => true;
	public override bool canStop() => true;
	protected override bool requiresLegs() => false;
	protected override void DoAbility()
	{
		if (isShielding) return;
		isShielding = true;
		PlayAnimationClip(shieldOutClip.name, 1 );
	}



	public override void SetPlayer(Player _player)
	{
		base.SetPlayer(_player);
		_player.Controller.InteractRightShoulder.OnPress += Controller_RightShouldPress;
		_player.Controller.InteractRightShoulder.OnRelease += Controller_RightShouldRelease;
	}

	private void Controller_RightShouldRelease(NewControlButton obj)
	{
		Stop();
	}

	private void OnDestroy()
	{
		if (player == null) return;
		if (player.Controller == null) return;
		player.Controller.InteractRightShoulder.OnPress -= Controller_RightShouldPress;
	}

	public override void Stop()
	{
		base.Stop();
		isShielding = false;
	}


	public override bool canDo() => base.canDo() && jumpAbility.IsResting && !moveAbility.GetCanMove();

	private void Controller_RightShouldPress(NewControlButton newControlButton)
	{
		Do();
	}

}
