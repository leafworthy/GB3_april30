using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack : WeaponAbility
	{
		public Gun CurrentGun => currentGun;
		public AnimationClip simpleShootAnimationClip;
		private Gun currentGun;
		private float currentCooldownTime;
		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;

		public Gun primaryGun => _primaryGun ??= GetComponent<PrimaryGun>();
		private Gun _primaryGun;
		public Gun unlimitedGun => _unlimitedGun ??= GetComponent<UnlimitedGun>();
		private Gun _unlimitedGun;
		private IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _aimAbility;
		public Vector2 AimDir => aimAbility.AimDir;
		public override string AbilityName => "Gun Attack " + currentState.ToString();
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle || currentState == weaponState.resuming;
		public bool IsUsingPrimaryGun => currentGun is PrimaryGun;
		private bool isPressingShoot;
		private JumpAbility jumpAbility  => _jumpAbility ??= GetComponent<JumpAbility>();
		private JumpAbility _jumpAbility;
		public event Action<bool> OnSwitchGun;

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
			if (!isActive || currentState != weaponState.idle || !jumpAbility.IsResting) return;

			if (!CurrentGun.Shoot(AimDir)) return;
			SetState(weaponState.attacking);
			PlayShootAnimation();

		}

		private void PlayShootAnimation()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			if (currentGun.simpleShoot)

				PlayAnimationClip(simpleShootAnimationClip.name, CurrentGun.AttackRate, 1);
			else
				PlayAnimationClip(CurrentGun.GetClipNameFromDegrees(), CurrentGun.AttackRate, .25f,1);
		}
		private void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			Stop();
			if (currentGun is not PrimaryGun) return;
			if (!currentGun.MustReload()) return;
			if(currentGun.CanReload()) OnNeedsReload?.Invoke();
			else  SwitchGuns(false);
		}

		protected override void DoAbility()
		{
			switch (currentState)
			{
				case weaponState.resuming:
					StartIdle();
					return;
				default:
					PullOut();
					break;
			}
		}

		protected override void PullOut()
		{
			SetState(weaponState.pullOut);
			PlayAnimationClip(currentGun.pullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			currentGun = primaryGun;




			StopListeningToEvents();
			ListenToEvents();
			Try();
		}

		void SwitchToPrimaryGunIfUnlimited(Ammo obj)
		{
			if (currentGun is PrimaryGun) return;
			SwitchGuns(true);
		}

		void SwitchToUnlimitedGun()
		{
			Debug.Log("try to switch to unlimited gun", this);
			if (currentGun is not PrimaryGun) return;
			SwitchGuns(false);
		}

		private void Gun_OnNeedsReload()
		{
			Stop();
			OnNeedsReload?.Invoke();
		}


		private void ListenToEvents()
		{
			if (currentGun != null) currentGun.OnEmpty += SwitchToUnlimitedGun;
			if (ammoInventory != null) ammoInventory.OnPrimaryAmmoAdded += SwitchToPrimaryGunIfUnlimited;
			if (primaryGun != null) primaryGun.OnNeedsReload += Gun_OnNeedsReload;
			if (unlimitedGun != null) unlimitedGun.OnNeedsReload += Gun_OnNeedsReload;

			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		private void OnDestroy()
		{
			StopListeningToEvents();
		}

		private void OnDisable()
		{
			StopListeningToEvents();
		}

		private void StopListeningToEvents()
		{
			if (primaryGun != null) primaryGun.OnNeedsReload -= Gun_OnNeedsReload;
			if (unlimitedGun != null) unlimitedGun.OnNeedsReload -= Gun_OnNeedsReload;
			if (currentGun != null) currentGun.OnEmpty -= SwitchToUnlimitedGun;
			if (ammoInventory != null) ammoInventory.OnPrimaryAmmoAdded -= SwitchToPrimaryGunIfUnlimited;


			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.Attack1RightTrigger == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		private void FixedUpdate()
		{
			if (!isActive) return;
			if (isPressingShoot && currentState != weaponState.attacking) StartAttacking();
			else if (currentState == weaponState.idle) Aim();
		}

		private void Aim()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			PlayAnimationClip(currentGun.GetClipNameFromDegrees(), 0, 1);
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


		private void SwitchGuns(bool toPrimary)
		{
			//if (toPrimary && primaryGun.MustReload() && currentGun is not PrimaryGun) return;
			Debug.Log("switch guns to primary: " + toPrimary, this);
			currentGun = toPrimary ? primaryGun : unlimitedGun;
			OnSwitchGun?.Invoke(toPrimary);
			Stop();
			Try();
		}

		public void SwapGuns()
		{
			SwitchGuns(!(currentGun is PrimaryGun));
		}
	}
}
