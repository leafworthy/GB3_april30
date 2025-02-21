using System.Collections.Generic;
using UnityEngine;

public class GunAimAbility : AimAbility
{

	private Animations anim;
	private bool isAttacking;
	public Vector2 CorrectForGlockOffset = new Vector2(-0.37f,0);
	public Vector2 OriginalTopSpritePosition;
	public GameObject TopSprite;

	[Header("AK Clips")]
	public AnimationClip E;
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

	[Header("Glock Clips")]
	public AnimationClip Glock_E;
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
	private GunAttack gunAttack;
	private bool Reloading;

	public override void Start()
	{
		base.Start();
		gunAttack = GetComponent<GunAttack>();
		gunAttack.OnShotHitTarget += GunOnShoot;
		gunAttack.OnShotMissed += GunOnShoot;
		anim = GetComponent<Animations>();
		anim.animEvents.OnAnimationComplete += Anim_OnComplete;
		akClips.AddMany(E, EEN, EN, NE, NW, WN, WWN, W, WWS, WS, SW, SE, ES, EES);
		glockClips.AddMany(Glock_E, Glock_EEN, Glock_EN, Glock_NE, Glock_NW, Glock_WN, Glock_WWN, Glock_W, Glock_WWS, Glock_WS, Glock_SW, Glock_SE, Glock_ES, Glock_EES);
		OriginalTopSpritePosition = TopSprite.transform.localPosition;
	}

	private void OnDisable()
	{
		if (gunAttack == null) return;
		gunAttack.OnShotHitTarget -= GunOnShoot;
		gunAttack.OnShotMissed -= GunOnShoot;
	}

	protected override Vector3 GetRealAimDir()
	{
		if (owner.isUsingMouse)
		{
			return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
		}
		else
		{
			return owner.Controller.AimAxis.GetCurrentAngle();
		}
	}

	private void Anim_OnComplete()
	{
		isAttacking = false;
	}

	private void GunOnShoot(Attack attack, Vector2 vector2)
	{
		if (attack.Owner != owner) return;
		isAttacking = true;
		anim.animator.speed = 1;
		anim.SetFloat(Animations.ShootSpeed, 1);
		anim.Play(GetAnimationClipNameFromDegrees(GetDegrees()), 1, .25f);
		if (gunAttack.isGlocking)
		{
			if (!body.TopIsFacingRight) TopSprite.transform.localPosition = OriginalTopSpritePosition + CorrectForGlockOffset;
			else TopSprite.transform.localPosition = OriginalTopSpritePosition;
		}
	}

	private float GetDegrees()
	{
		var degrees = Vector2.Angle(AimDir, Vector2.right);
		if (AimDir.y < 0) degrees -= 360;

		return degrees;
	}



	protected override void Update()
	{
		base.Update();
		body.TopFaceDirection(GetClampedAimDir().x >= 0);
		if (gunAttack.isGlocking)
		{
			if (!body.TopIsFacingRight) TopSprite.transform.localPosition = OriginalTopSpritePosition + CorrectForGlockOffset;
			else TopSprite.transform.localPosition = OriginalTopSpritePosition;
		}
		if (!isAttacking && !gunAttack.isReloading && !body.arms.isActive) AimInDirection(GetDegrees());
	}

	private string GetAnimationClipNameFromDegrees(float degrees)
	{
	
		degrees = Mathf.Abs(degrees);
		degrees += 12.85f; // (-12.85 - 347.15)

		var angleDifference = 25.7f;
		var whichOne = Mathf.FloorToInt(Mathf.Abs(degrees / angleDifference));

		if (gunAttack.isGlocking)
		{
			if (whichOne >= glockClips.Count) whichOne = 0;
			return glockClips[whichOne].name;
		}
		else
		{
			if (whichOne >= akClips.Count) whichOne = 0;
			return akClips[whichOne].name;
		}

		
	}

	private void AimInDirection(float degrees)
	{
		degrees = Mathf.Abs(degrees);
		if (degrees > 90)
			degrees += 12.85f; // (-12.85 - 347.15)

		var angleDifference = 25.7f;
		var whichOne = Mathf.FloorToInt(Mathf.Abs(degrees / angleDifference));

		if (whichOne >= akClips.Count) whichOne = 0;

		if(gunAttack.isGlocking)
		{
			if (glockClips[whichOne] != null) anim.Play(glockClips[whichOne].name, 1, 0);
			if(!body.TopIsFacingRight) TopSprite.transform.localPosition = OriginalTopSpritePosition + CorrectForGlockOffset;
			else TopSprite.transform.localPosition = OriginalTopSpritePosition;
		}
		else
		{
			TopSprite.transform.localPosition = OriginalTopSpritePosition;
			if (akClips[whichOne] != null) anim.Play(akClips[whichOne].name, 1, 0);
		}
		
		
		anim.SetFloat(Animations.ShootSpeed, 0);
	}

	private Vector2 GetClampedAimDir()
	{
		var clampedDir = AimDir.normalized;
		if (clampedDir.x <= .25f && clampedDir.x >= 0)
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