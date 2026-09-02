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
		public event Action OnEmpty;

		public event Action<bool> OnSwitchGun;
		public event Action OnNeedsReload;

		public override void Resume()
		{
			SetState(weaponState.idle);
		}

		void Start()
		{
			Debug.Log("WHATTHEFUCK");
			TryToActivate();
		}

		protected virtual void StartAttacking()
		{
			if (!isIdle || !jumpAbility.IsResting) return;
			if (body.doableArms.CurrentAbility != null)
			{
				if (!body.doableArms.CurrentAbility.canStop(this)) return;
				body.doableArms.CurrentAbility.StopAbility();
			}

			if (!CurrentGun.Shoot()) return;
			SetState(weaponState.attacking);
			PlayShootAnimation();
		}

		protected virtual void PlayShootAnimation()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			PlayAnimationClip(CurrentGun.GetShootClipName(), CurrentGun.AttackRate, 1);
		}

		protected virtual void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

		protected override void DoAbility()
		{
			Debug.Log("current state: " + currentState, this);
			switch (currentState)
			{

				case weaponState.resuming:
				case weaponState.pullOut:
					StartIdle();
					break;
				case weaponState.not:
					PullOutWeapon();
					break;
				case weaponState.idle:
					StartIdle();
					break;
				case weaponState.attacking:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			if (!CurrentGun.CanReload() && CurrentGun.MustReload() && CurrentGun is PrimaryGun)
			{
				SwitchGuns(false);
				return;
			}

			if(CurrentGun.CanReload() && CurrentGun.MustReload())
			{
				Debug.Log("here");
				OnNeedsReload?.Invoke();
			}
		}

		protected override void PullOutWeapon()
		{
			Debug.Log("pull out here, hudSlotState: " + currentState, this);
			SetState(weaponState.pullOut);
			anim.SetBool(UnitAnimations.IsUsingPrimary, true);
			PlayAnimationClip(CurrentGun.pullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			if(unlimitedGun == null) CurrentGun = primaryGun;
			 else CurrentGun = primaryGun.HasAnyAmmo() ? primaryGun : unlimitedGun;

			StopListeningToEvents();
			ListenToEvents();
			if(CurrentGun.HasAnyAmmo()) TryToActivate();
		}

		protected virtual void SwitchToPrimaryGunIfUnlimited(Ammo obj)
		{
			if (unlimitedGun == null) return;
			if (CurrentGun is PrimaryGun) return;
			SwitchGuns(true);
		}

		protected virtual void SwitchToUnlimitedGun()
		{
			if (unlimitedGun == null) return;
			Debug.Log("try to switch to unlimited gun", this);
			if (CurrentGun is not PrimaryGun) return;
			SwitchGuns(false);
		}

		protected void Gun_OnNeedsReload()
		{
			//StopAbility();
			OnNeedsReload?.Invoke();
		}

		protected virtual void ListenToEvents()
		{
			if (CurrentGun != null) CurrentGun.OnEmpty += SwitchToUnlimitedGun;
			if (ammoInventory != null) ammoInventory.OnPrimaryAmmoAdded += SwitchToPrimaryGunIfUnlimited;
			if (primaryGun != null) primaryGun.OnNeedsReload += Gun_OnNeedsReload;
			if (unlimitedGun != null) unlimitedGun.OnNeedsReload += Gun_OnNeedsReload;

			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}


		void OnDisable()
		{
			StopListeningToEvents();
		}

		protected virtual void StopListeningToEvents()
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

		protected virtual void FixedUpdate()
		{
			Debug.Log(currentState.ToString());
			if (!isActive)
			{
				Debug.Log("not active", this);
				return;
			}
			if (isPressingShoot && currentState != weaponState.attacking)
			{
				Debug.Log("starting attacking", this);
				StartAttacking();
			}
			else if (currentState == weaponState.idle) Aim();
		}

		protected virtual void Aim()
		{
			if (!body.doableArms.CanDoActivity(this) && !body.doableArms.isDoingAbility(this)) return;
			Debug.Log("try to aim, weaponstate: " + currentState, this);
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			PlayAnimationClipWithoutEvent(CurrentGun.GetClipNameFromDegrees(), 1);
		}

		protected override void AnimationComplete()
		{

			base.AnimationComplete();
		}

		protected void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			isPressingShoot = true;
			StartAttacking();
		}

		protected void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
		}

		public bool SwitchGuns(bool toPrimary)
		{

			Debug.Log("try to switch guns: " + toPrimary + " must reload " + primaryGun.MustReload(), this);
			if (!CanSwapGuns()) return false;
			Debug.Log("switch guns to primary: " + toPrimary, this);
			if (unlimitedGun == null) return false;
			CurrentGun = toPrimary ? primaryGun : unlimitedGun;
			anim.SetBool(UnitAnimations.IsUsingPrimary, toPrimary);
			OnSwitchGun?.Invoke(toPrimary);
			StopAbility();
			TryToActivate();
			return true;
		}

		public bool CanSwapGuns()
		{
			if (CurrentGun is PrimaryGun || primaryGun.CanSwap()) return true;
			Debug.Log( "cannot switch to primary gun because it cannot reload", this);
			return false;
		}

		public bool SwapGuns() => SwitchGuns(CurrentGun is not PrimaryGun);
	}
}
namespace __SCRIPTS
{
}
