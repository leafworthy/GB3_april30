using System;
using System.Collections.Generic;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack : WeaponAbility
	{
		public Gun CurrentGun => currentGun;
		private Gun currentGun;
		private float currentCooldownTime;
		private IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _aimAbility;
		public override string AbilityName => currentState == weaponState.attacking ? "Gun-Attacking" : "Gun-Aiming";
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle;

		public Vector2 AimDir => aimAbility.AimDir;
		private List<Gun> allGuns => _allGuns ??= new List<Gun>(GetComponents<Gun>());
		private List<Gun> _allGuns;
		private CharacterJumpAbility jumps => _jumps ??= GetComponent<CharacterJumpAbility>();
		private CharacterJumpAbility _jumps;
		public bool IsUsingPrimaryGun => currentGun is PrimaryGun;
		private bool isPressingShoot;

		public event Action OnNeedsReload;

		public override void Stop()
		{
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			base.Stop();
		}

#pragma warning disable UDR0001
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
#pragma warning restore UDR0001

		private void StartAttacking()
		{
			if (!isActive || currentState != weaponState.idle) return;
			SetState(weaponState.attacking);
			CurrentGun.Shoot(AimDir);
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			PlayAnimationClip(GetClipNameFromDegrees(GetDegreesFromAimDir()), CurrentGun.AttackRate, 1);
			body.TopFaceDirection(GetClampedAimDir().x >= 0);
		}

		protected override void AnimationComplete()
		{
			base.AnimationComplete();
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			if (!currentGun.MustReload()) return;
			Stop();
			OnNeedsReload?.Invoke();
		}

		protected override void DoAbility()
		{
			Debug.Log("gun attack start");
			if (currentState == weaponState.resuming)
			{
				StartIdle();
			}

			if (currentState == weaponState.not)
			{
				PullOut();
			}
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

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			currentGun = allGuns[0];
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

		public override bool canDo() => base.canDo() && CurrentGun.CanShoot();

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

		private void OnDestroy()
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
			if (!isActive) return;
			if (isPressingShoot && currentState !=  weaponState.attacking) StartAttacking();
			else if(currentState == weaponState.idle) Aim();
		}

		private void Aim()
		{
			Debug.Log("aim");
			body.TopFaceDirection(GetClampedAimDir().x >= 0);
			PlayAnimationClip(GetClipNameFromDegrees(GetDegreesFromAimDir()), 0, 1);
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			isPressingShoot = true;
			StartAttacking();
		}

		private void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
		}

		public void SwitchGuns(int i)
		{
			if (i < 0 || i >= allGuns.Count) return;
			currentGun = allGuns[i];
		}

		public void ResumeFromReload()
		{
			SetState(weaponState.resuming);
			Do();
		}
	}
}
