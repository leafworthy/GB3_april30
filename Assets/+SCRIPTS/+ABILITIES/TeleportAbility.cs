using __SCRIPTS;
using UnityEngine;

public class TeleportAbility : Ability
{
	public override string AbilityName => "Teleport";
	public AnimationClip teleportAnimationClip;

	private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
	private MoveAbility _moveAbility;

	private CharacterJumpAbility jumps => _jumps ??= GetComponent<CharacterJumpAbility>();
	private CharacterJumpAbility _jumps;
	private const float teleportTime = .2f;
	protected override bool requiresArms() => true;

	protected override bool requiresLegs() => true;
	public override bool canDo() => base.canDo() && jumps.IsResting;

	protected override void DoAbility()
	{
		Teleport();
	}

	private void Teleport()
	{
		if (teleportAnimationClip != null) PlayAnimationClip(teleportAnimationClip);
		Invoke(nameof(Anim_Teleport), teleportTime);
	}

	public override void SetPlayer(Player _player)
	{
		base.SetPlayer(_player);
		player.Controller.DashRightShoulder.OnPress += Controller_DashPress;
	}

	private void Controller_DashPress(NewControlButton obj)
	{
		Debug.LogWarning("dash pressed inside dash");
		Do();
	}

	private void OnDestroy()
	{
		if (player == null) return;
		if (player.Controller == null) return;
		player.Controller.DashRightShoulder.OnPress -= Controller_DashPress;
	}

	private void Anim_Teleport()
	{
		var teleportDirection = moveAbility.GetMoveDir();
		if (teleportDirection.magnitude < 0.1f)
		{
			teleportDirection = moveAbility.GetLastMoveAimDirOffset().normalized;
			if (teleportDirection.magnitude < 0.1f) teleportDirection = body.BottomIsFacingRight ? Vector2.right : Vector2.left;
		}

		var teleportDistance = life.DashSpeed;
		var teleportOffset = teleportDirection.normalized * teleportDistance;
		var newPoint = (Vector2) transform.position + teleportOffset;

		transform.position = newPoint;
	}

	public override void Stop()
	{
		base.Stop();
		lastLegAbility?.Do();
	}
}