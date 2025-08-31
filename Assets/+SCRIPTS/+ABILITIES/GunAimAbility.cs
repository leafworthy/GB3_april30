using System;
using System.Collections.Generic;
using __SCRIPTS;
using __SCRIPTS.Cursor;
using UnityEngine;

public interface IAimableGun
{
	event Action<Attack, Vector2> OnShotHitTarget;
	event Action<Attack, Vector2> OnShotMissed;
	bool isGlocking { get; }
	bool isReloading { get; }
}

public class GunAimAbility : DoableAimAbility
{
	public Vector2 CorrectForGlockOffset = new(-0.37f, 0);
	public Vector2 OriginalTopSpritePosition;
	public GameObject TopSprite;

	[Header("AK Clips")] public AnimationClip E;
	public AnimationClip EEN;
	public AnimationClip EN;
	public AnimationClip NE;
	public AnimationClip NW;
	public AnimationClip WN;
	public AnimationClip WWN;
	public AnimationClip W;
	public AnimationClip WWS;
	public AnimationClip WS;
	public AnimationClip SW;
	public AnimationClip SE;
	public AnimationClip ES;
	public AnimationClip EES;

	[Header("Glock Clips")] public AnimationClip Glock_E;
	public AnimationClip Glock_EEN;
	public AnimationClip Glock_EN;
	public AnimationClip Glock_NE;
	public AnimationClip Glock_NW;
	public AnimationClip Glock_WN;
	public AnimationClip Glock_WWN;
	public AnimationClip Glock_W;
	public AnimationClip Glock_WWS;
	public AnimationClip Glock_WS;
	public AnimationClip Glock_SW;
	public AnimationClip Glock_SE;
	public AnimationClip Glock_ES;
	public AnimationClip Glock_EES;

	private List<AnimationClip> akClips = new();
	private List<AnimationClip> glockClips = new();
	private IAimableGun aimableGun;
	private bool Reloading;

	public override void SetPlayer(Player _player)
	{
		base.SetPlayer(_player);
		aimableGun = GetComponent<IAimableGun>();
		akClips.AddMany(E, EEN, EN, NE, NW, WN, WWN, W, WWS, WS, SW, SE, ES, EES);
		glockClips.AddMany(Glock_E, Glock_EEN, Glock_EN, Glock_NE, Glock_NW, Glock_WN, Glock_WWN, Glock_W, Glock_WWS, Glock_WS, Glock_SW, Glock_SE, Glock_ES,
			Glock_EES);
		OriginalTopSpritePosition = TopSprite.transform.localPosition;
		aimableGun.OnShotHitTarget += AimableGunOnShoot;
		aimableGun.OnShotMissed += AimableGunOnShoot;
	}

	private void OnDisable()
	{
		if (aimableGun == null) return;
		aimableGun.OnShotHitTarget -= AimableGunOnShoot;
		aimableGun.OnShotMissed -= AimableGunOnShoot;
	}

	protected override Vector3 GetRealAimDir()
	{
		if (player.isUsingMouse) return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
		return player.Controller.AimAxis.GetCurrentAngle();
	}

	private void AimableGunOnShoot(Attack attack, Vector2 vector2)
	{
		if (attack.Owner != player) return;
		anim.animator.speed = 1;
		anim.SetFloat(UnitAnimations.ShootSpeed, 1);
		Debug.Log(" aim gun shoot");
		anim.Play(DegreesToClipName(GetDegrees()), 1, .25f);
		CorrectTopSpritePositionForGlock();
	}

	protected override void DoAbility()
	{
		base.DoAbility();
		Debug.Log(" doing ability aim gun");
		AimInDirection(GetDegrees());
	}

	private float GetDegrees()
	{
		var degrees = Vector2.Angle(AimDir, Vector2.right);
		if (AimDir.y < 0) degrees -= 360;

		return degrees;
	}

	protected override void Update()
	{
		if (pauseManager.IsPaused) return;
		TopFaceCorrectDirection();
		CorrectTopSpritePositionForGlock();
		base.Update();
	}

	private void TopFaceCorrectDirection()
	{
		body.TopFaceDirection(GetClampedAimDir().x >= 0);
	}

	private void CorrectTopSpritePositionForGlock()
	{
		if (!aimableGun.isGlocking) return;
		if (!body.TopIsFacingRight)
			TopSprite.transform.localPosition = OriginalTopSpritePosition + CorrectForGlockOffset;
		else TopSprite.transform.localPosition = OriginalTopSpritePosition;
	}

	private string DegreesToClipName(float degrees)
	{
		degrees = Mathf.Abs(degrees);
		degrees += 12.85f; // (-12.85 - 347.15)

		var angleDifference = 25.7f;
		var whichOne = Mathf.FloorToInt(Mathf.Abs(degrees / angleDifference));
		var whichClips = aimableGun.isGlocking ? glockClips : akClips;
		if (whichOne >= whichClips.Count) whichOne = 0;
		return whichClips[whichOne].name;
	}

	private void AimInDirection(float degrees)
	{
		var whichClip = DegreesToClipName(degrees);
		Debug.Log("aim in direction: " + whichClip);
		anim.Play(whichClip, 1, 0);
		anim.SetFloat(UnitAnimations.ShootSpeed, 0);
	}

	private Vector2 GetClampedAimDir()
	{
		var clampedDir = AimDir.normalized;
		if (clampedDir.x is <= .25f and >= 0)
		{
			clampedDir.x = .25f;
			if (AimDir.y > 0)
				clampedDir.y = 1f;
			else
				clampedDir.y = -1;
		}

		if (clampedDir.x < 0 && clampedDir.x > -.25) clampedDir.x = -.25f;
		return clampedDir;
	}
}
