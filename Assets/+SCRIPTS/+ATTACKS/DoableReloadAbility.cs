using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableReloadAbility : Ability
	{

		public event Action OnReload;
		public AnimationClip ReloadAKAnimationClip;
		public AnimationClip ReloadGlockAnimationClip;
		public AnimationClip ReloadGlockAnimationLeftClip;
		public bool isReloading { get; private set; }
		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;
		private IAimableGunAttack gunAttack => _gunAttack ??= GetComponent<IAimableGunAttack>();
		private IAimableGunAttack _gunAttack;

		private bool isEmpty =>
			gunAttack.IsUsingPrimaryGun ? ammoInventory.unlimitedAmmo.hasAmmoInReserveOrClip() : ammoInventory.primaryAmmo.hasAmmoInReserveOrClip();

		private float reloadTime = .5f;
		public override string VerbName => "Reloading";
		private Ammo GetCorrectAmmoType() => gunAttack.IsUsingPrimaryGun ? ammoInventory.unlimitedAmmo : ammoInventory.primaryAmmo;
		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => false;

		public override bool canDo()
		{
			if (!base.canDo()) return false;
			if (GetCorrectAmmoType().clipIsFull()) return false;
			if (!GetCorrectAmmoType().hasReserveAmmo()) return false;
			return true;
		}

		protected override void DoAbility()
		{
			StartReloading();
		}

		private void StartReloading()
		{
			Invoke(nameof(Reload), reloadTime);
			anim.SetBool(UnitAnimations.IsBobbing, false);
			isReloading = true;

			if (gunAttack.IsUsingPrimaryGun)
				anim.Play(gunAttack.AimDir.x > 0 ? ReloadGlockAnimationClip.name : ReloadGlockAnimationLeftClip.name, 1, 0);
			else
				anim.Play(ReloadAKAnimationClip.name, 1, 0);
		}

		private void Reload()
		{
			GetCorrectAmmoType().Reload();
			OnReload?.Invoke();
		}

		private void StopReloading()
		{
			if (!isReloading) return;
			isReloading = false;
			body.doableArms.Stop(this);
			//try shooting
		}

		public override void Stop()
		{
			base.Stop();
			StopReloading();
			StopListeningToPlayer();
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			ListenToPlayer();
		}

		private void ListenToPlayer()
		{
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
		}

		private void Player_Reload(NewControlButton btn)
		{
			Do();
		}

		private void StopListeningToPlayer()
		{
			player.Controller.ReloadTriangle.OnPress -= Player_Reload;
		}
	}
}
