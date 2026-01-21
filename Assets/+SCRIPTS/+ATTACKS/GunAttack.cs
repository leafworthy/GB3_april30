using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack : WeaponAbility
	{
		public Gun CurrentGun { get; private set; }
		AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		AmmoInventory _ammoInventory;
		public Gun primaryGun => _primaryGun ??= GetComponent<PrimaryGun>();
		Gun _primaryGun;
		public Gun unlimitedGun => _unlimitedGun ??= GetComponent<UnlimitedGun>();
		Gun _unlimitedGun;
		IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		IAimAbility _aimAbility;
		public Vector2 AimDir => aimAbility.AimDir;
		public override string AbilityName => "Gun Attack " + currentState;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle || currentState == weaponState.resuming;
		public bool IsUsingPrimaryGun => CurrentGun is PrimaryGun;
		bool isPressingShoot;
		JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		JumpAbility _jumpAbility;

		public event Action<bool> OnSwitchGun;
		public event Action OnNeedsReload;

		public override void StopAbility()
		{
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			base.StopAbility();
		}

#pragma warning disable UDR0001
		static string[] PrimaryAnimationClips =
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

		void StartAttacking()
		{
			if (isIdle || !jumpAbility.IsResting) return;

			if (!CurrentGun.Shoot()) return;
			SetState(weaponState.attacking);
			PlayShootAnimation();
		}

		void PlayShootAnimation()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			PlayAnimationClip(CurrentGun.GetShootClipName(), CurrentGun.AttackRate, 1);
		}

		void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			StopAbility();
			if (CurrentGun is not PrimaryGun) return;
			if (!CurrentGun.MustReload()) return;
			if (CurrentGun.CanReload()) OnNeedsReload?.Invoke();
			else SwitchGuns(false);
		}

		protected override void DoAbility()
		{
			switch (currentState)
			{
				case weaponState.resuming:
					StartIdle();
					return;
				default:
					PullOutWeapon();
					break;
			}
		}

		protected override void PullOutWeapon()
		{
			SetState(weaponState.pullOut);
			PlayAnimationClip(CurrentGun.pullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			CurrentGun = primaryGun;

			StopListeningToEvents();
			ListenToEvents();
			TryToActivate();
		}

		void SwitchToPrimaryGunIfUnlimited(Ammo obj)
		{
			if (CurrentGun is PrimaryGun) return;
			SwitchGuns(true);
		}

		void SwitchToUnlimitedGun()
		{
			Debug.Log("try to switch to unlimited gun", this);
			if (CurrentGun is not PrimaryGun) return;
			SwitchGuns(false);
		}

		void Gun_OnNeedsReload()
		{
			StopAbility();
			OnNeedsReload?.Invoke();
		}

		void ListenToEvents()
		{
			if (CurrentGun != null) CurrentGun.OnEmpty += SwitchToUnlimitedGun;
			if (ammoInventory != null) ammoInventory.OnPrimaryAmmoAdded += SwitchToPrimaryGunIfUnlimited;
			if (primaryGun != null) primaryGun.OnNeedsReload += Gun_OnNeedsReload;
			if (unlimitedGun != null) unlimitedGun.OnNeedsReload += Gun_OnNeedsReload;

			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		void OnDestroy()
		{
			StopListeningToEvents();
		}

		void OnDisable()
		{
			StopListeningToEvents();
		}

		void StopListeningToEvents()
		{
			if (primaryGun != null) primaryGun.OnNeedsReload -= Gun_OnNeedsReload;
			if (unlimitedGun != null) unlimitedGun.OnNeedsReload -= Gun_OnNeedsReload;
			if (CurrentGun != null) CurrentGun.OnEmpty -= SwitchToUnlimitedGun;
			if (ammoInventory != null) ammoInventory.OnPrimaryAmmoAdded -= SwitchToPrimaryGunIfUnlimited;

			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.Attack1RightTrigger == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		void FixedUpdate()
		{
			if (!isActive) return;
			if (isPressingShoot && currentState != weaponState.attacking) StartAttacking();
			else if (currentState == weaponState.idle) Aim();
		}

		void Aim()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			PlayAnimationClipWithoutEvent(CurrentGun.GetClipNameFromDegrees(), 1);
		}

		void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			isPressingShoot = true;
			StartAttacking();
		}

		void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
		}

		void SwitchGuns(bool toPrimary)
		{
			//if (toPrimary && primaryGun.MustReload() && _currentGun is not PrimaryGun) return;
			Debug.Log("switch guns to primary: " + toPrimary, this);
			CurrentGun = toPrimary ? primaryGun : unlimitedGun;
			OnSwitchGun?.Invoke(toPrimary);
			StopAbility();
			TryToActivate();
		}

		public void SwapGuns()
		{
			SwitchGuns(!(CurrentGun is PrimaryGun));
		}
	}
}
