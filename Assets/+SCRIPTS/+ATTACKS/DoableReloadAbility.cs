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
		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;
		private IAimableGunAttack gunAttack => _gunAttack ??= GetComponent<IAimableGunAttack>();
		private IAimableGunAttack _gunAttack;

		private bool isEmpty =>
			gunAttack.IsUsingPrimaryGun ? ammoInventory.unlimitedAmmo.hasAmmoInReserveOrClip() : ammoInventory.primaryAmmo.hasAmmoInReserveOrClip();

		private float reloadTime = .5f;
		public override string VerbName => "Reloading";
		private Ammo GetCorrectAmmoType() => gunAttack.currentGun.Ammo;
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo()
		{
			if (!base.canDo()) return false;
			if (GetCorrectAmmoType().clipIsFull())
			{
				Debug.Log("clip is full");
				return false;
			}
			if (!GetCorrectAmmoType().hasReserveAmmo())
			{
				Debug.Log("no reserve ammo");
				return false;
			}

			Debug.Log("reload ability can do");
			return true;
		}

		protected override void DoAbility()
		{
			StartReloading();
		}

		private void StartReloading()
		{
			Debug.Log("start reloading");
			Invoke(nameof(Reload), reloadTime);
			anim.SetBool(UnitAnimations.IsBobbing, false);

			if (!gunAttack.IsUsingPrimaryGun)
			{
				Debug.Log("playing glock reload animation");
				PlayAnimationClip(gunAttack.AimDir.x > 0 ? ReloadGlockAnimationClip : ReloadGlockAnimationLeftClip, 1);
			}
			else
				PlayAnimationClip(ReloadAKAnimationClip, 1);
		}

		private void Reload()
		{
			Debug.Log("reload happens");
			GetCorrectAmmoType().Reload();
			OnReload?.Invoke();
		}

		public override void Stop()
		{
			player.Controller.ReloadTriangle.OnPress -= Player_Reload;
			base.Stop();
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
		}

		private void Player_Reload(NewControlButton btn)
		{
			Do();
		}
	}
}
