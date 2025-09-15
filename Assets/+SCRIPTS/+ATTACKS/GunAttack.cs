using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack : Ability
	{
		public bool IsUsingPrimaryGun => currentGun is PrimaryGun;
		private bool isPressingShoot;
		public Gun CurrentGun => currentGun;
		private Gun currentGun;
		private float currentCooldownTime;
		private IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _aimAbility;
		public override string AbilityName => "Shooting";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;
		public override bool canStop() => true;

		public Vector2 AimDir => aimAbility.AimDir;

		private List<Gun> allGuns => _allGuns ??= new List<Gun>(GetComponents<Gun>());
		private List<Gun> _allGuns;
		private GunAttack aimableGun;
		public bool simpleShoot;
		private CharacterJumpAbility jumps => _jumps ??= GetComponent<CharacterJumpAbility>();
		private CharacterJumpAbility _jumps;

		public event Action OnNeedsReload;

		public override void Stop()
		{
			base.Stop();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
		}


		private static string[] PrimaryAnimationClips =
		{
			"E",
			"EES",
			"ES",
			"SE",
			"SSE",
			"SSE",
			"SE",
			"ES",
			"EES",
			"E",
			"EEN",
			"EN",
			"NE",
			"NNE",
			"NNE",
			"NE",
			"EN",
			"EEN"
		};

		protected override void DoAbility()
		{
			Debug.Log("Shoot");
			CurrentGun.Shoot(aimAbility.AimDir);
			body.TopFaceDirection(GetClampedAimDir().x >= 0);
			PlayAnimationClip(GetClipNameFromDegrees(GetDegreesFromAimDir()), CurrentGun.AttackRate, 1);
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
		}

		private float GetDegreesFromAimDir()
		{
			var radians = Mathf.Atan2(-AimDir.y, AimDir.x); // NEGATE y-axis
			var degrees = radians * Mathf.Rad2Deg;
			if (degrees < 0)
				degrees += 360f;
			return degrees;
		}

		private string GetClipNameFromDegrees(float degrees)
		{
			var whichPortion = GetDirectionPortion(degrees);
			if (whichPortion > PrimaryAnimationClips.Length) whichPortion = 0;
			return IsUsingPrimaryGun ? PrimaryAnimationClips[whichPortion] : PrimaryAnimationClips[whichPortion] + "_Glock";
		}

		private static int GetDirectionPortion(float degrees)
		{
			var normalizedDegrees = degrees % 360;
			if (normalizedDegrees < 0) normalizedDegrees += 360;
			var offsetDegrees = (normalizedDegrees + 10) % 360;
			var portionWidth = 360f / PrimaryAnimationClips.Length;
			var portionNumber = Mathf.FloorToInt(offsetDegrees / portionWidth);
			portionNumber = Mathf.Clamp(portionNumber, 0, PrimaryAnimationClips.Length - 1);
			return portionNumber;
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

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			currentGun = allGuns[0];
			Debug.Log(  "set player gun attack, current gun is primary: " + (currentGun is PrimaryGun) + " total guns: " + allGuns.Count);
			foreach (var gun in allGuns)
			{
				gun.OnShotHitTarget -= Gun_OnShotHitTarget;
				gun.OnShotMissed -= Gun_OnShotMissed;
				gun.OnNeedsReload -= Gun_OnNeedsReload;

				gun.OnShotHitTarget += Gun_OnShotHitTarget;
				gun.OnShotMissed += Gun_OnShotMissed;
				gun.OnNeedsReload += Gun_OnNeedsReload;
			}

			StopListeningToEvents();
			ListenToEvents();
		}

		private void Gun_OnNeedsReload()
		{
			Stop();
			OnNeedsReload?.Invoke();
			Debug.Log("gun needs reload");
		}

		private void Gun_OnShotMissed(Attack attack)
		{
			Shoot();
		}

		private void Shoot()
		{
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			if (currentGun.simpleShoot)
				anim.Play("Shoot", 1, 0);
			else
				anim.Play(GetClipNameFromDegrees(GetDegreesFromAimDir()), 1, .25f);
		}

		private void Gun_OnShotHitTarget(Attack attack)
		{
			Shoot();
		}

		public override bool canDo() => base.canDo() && CurrentGun.CanShoot() && jumps.IsResting;

		private void ListenToEvents()
		{
			foreach (var gun in allGuns)
			{
				gun.OnShotHitTarget -= Gun_OnShotHitTarget;
				gun.OnShotMissed -= Gun_OnShotMissed;
			}

			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		private void OnDisable()
		{
			StopListeningToEvents();
		}

		private void StopListeningToEvents()
		{
			foreach (var gun in allGuns)
			{
				gun.OnShotHitTarget -= Gun_OnShotHitTarget;
				gun.OnShotMissed -= Gun_OnShotMissed;
			}

			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		private void FixedUpdate()
		{
			if (isPressingShoot)
				Do();
			else
				Aim();
		}

		private void Aim()
		{
			if (body.doableArms.IsActive) return;
			body.TopFaceDirection(GetClampedAimDir().x >= 0);
			PlayAnimationClip(GetClipNameFromDegrees(GetDegreesFromAimDir()), 0, 1);
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			isPressingShoot = true;
			Do();
		}

		private void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
		}

		public void SwitchGuns(int i)
		{
			if (i < 0 || i >= allGuns.Count) return;
			currentGun = allGuns[i];
			Debug.Log("switched to gun " + i + " isPrimaryGun: " + (currentGun is PrimaryGun));
		}
	}
}
