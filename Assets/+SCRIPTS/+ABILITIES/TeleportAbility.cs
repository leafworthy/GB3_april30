using __SCRIPTS;
using UnityEngine;

public class TeleportAbility : Ability
{
	public override string AbilityName => "Teleport";
	public AnimationClip teleportAnimationClip;

	MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
	MoveAbility _moveAbility;

	JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
	JumpAbility _jumps;
	const float teleportTime = .1f;
	public override bool requiresArms() => true;

	public override bool requiresLegs() => true;
	public override bool canDo() => base.canDo() && jumps.IsResting;

	protected override void DoAbility()
	{
		Teleport();
	}

	void Teleport()
	{
		if (teleportAnimationClip != null) PlayAnimationClip(teleportAnimationClip);
		life.SetTemporarilyInvincible(true);
		Invoke(nameof(Anim_Teleport), teleportTime);
	}

	public override void SetPlayer(Player newPlayer)
	{
		base.SetPlayer(newPlayer);
		player.Controller.DashRightShoulder.OnPress += Controller_DashPress;
	}

	void Controller_DashPress(NewControlButton obj)
	{
		TryToDoAbility();
	}

	void OnDestroy()
	{
		if (player == null) return;
		if (player.Controller == null) return;
		player.Controller.DashRightShoulder.OnPress -= Controller_DashPress;
	}

	void Anim_Teleport()
	{
		var teleportDirection = moveAbility.GetMoveDir();
		if (teleportDirection.magnitude < 0.1f)
		{
			teleportDirection = moveAbility.GetLastMoveAimDirOffset().normalized;
			if (teleportDirection.magnitude < 0.1f) teleportDirection = body.BottomIsFacingRight ? Vector2.right : Vector2.left;
		}

		var teleportDistance = attacker.stats.Stats.DashSpeed;
		var teleportOffset = teleportDirection.normalized * teleportDistance;
		var newPoint = (Vector2) transform.position + teleportOffset;

		transform.position = newPoint;
	}

	public override void StopAbilityBody()
	{
		life.SetTemporarilyInvincible(false);
		base.StopAbilityBody();
		lastLegAbility?.TryToDoAbility();
	}
}
