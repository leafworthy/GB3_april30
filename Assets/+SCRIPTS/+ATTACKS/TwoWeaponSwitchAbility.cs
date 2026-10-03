using System;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace __SCRIPTS
{
	public class TwoWeaponSwitchAbility : SerializedMonoBehaviour, INeedPlayer
	{
		public GunAttack Primary_Weapon;
		 public GunAttack Secondary_Weapon;
		 public GunAttack currentWeapon;
		AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		AmmoInventory _ammoInventory;

		WeaponAbility weaponToSwitchTo;
		Player player;

		bool hasInitialized;

		public event Action<int> OnSwitchWeapon;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			if (hasInitialized)
			{
				Debug.LogWarning(" [SWITCHER] already initialized, skipping");
				return;
			}

			hasInitialized = true;

			player.Controller.Attack2LeftTrigger.OnPress += Player_SwapSecondary;
			player.Controller.Attack3Circle.OnPress += Player_SwapSecondary;

			player.Controller.Attack1RightTrigger.OnPress += Player_SwapPrimary;
			ammoInventory.OnPrimaryAmmoAdded += AmmoInventory_OnPrimaryAmmoAdded;
			StartSwitchingWeapons(Secondary_Weapon);
		}

		void AmmoInventory_OnPrimaryAmmoAdded(Ammo obj)
		{
			if (Primary_Weapon.CurrentGun.HasAmmoInClip()) return;
			StartSwitchingWeapons(Primary_Weapon);
		}

		void Player_SwapPrimary(NewControlButton obj)
		{
			if (!Primary_Weapon.CurrentGun.HasAnyAmmo()) return;
			StartSwitchingWeapons(Primary_Weapon);
		}

		void Player_SwapSecondary(NewControlButton obj)
		{
			StartSwitchingWeapons(Secondary_Weapon);
		}

		void OnDisable()
		{
			if (player == null) return;
			if (player.Controller == null) return;

			player.Controller.Attack2LeftTrigger.OnPress -= Player_SwapSecondary;
			player.Controller.Attack3Circle.OnPress -= Player_SwapSecondary;

			player.Controller.Attack1RightTrigger.OnPress -= Player_SwapPrimary;
		}

		void StartSwitchingWeapons(GunAttack _weaponToSwitchTo)
		{
			if (_weaponToSwitchTo == null) return;
			if (currentWeapon != null)
			{
				if (_weaponToSwitchTo == currentWeapon)
				{
					Debug.Log("[SWITCHER] Already using that weapon: " + _weaponToSwitchTo.AbilityName);
					return;
				}

				if (!currentWeapon.canStop(null) && currentWeapon.isActive)
				{
					Debug.Log("[SWITCHER] can't switch weapons right now, busy with: " + currentWeapon.AbilityName);
					return;
				}
			}
			SwitchCurrentWeapon(_weaponToSwitchTo);
		}

		void SwitchCurrentWeapon(GunAttack _weaponToSwitchTo)
		{
			Debug.Log("[SWITCHER]SwitchCurrentWeapon to: " + _weaponToSwitchTo.AbilityName);

			if (!_weaponToSwitchTo.canDo())
			{
				Debug.Log("[SWITCHER] can't switch to that weapon right now: " + _weaponToSwitchTo.AbilityName);
				return;
			}
			Debug.Log("[SWITCHER] can do, switching to: " + _weaponToSwitchTo.AbilityName);
			currentWeapon?.StopAbility();
			currentWeapon = _weaponToSwitchTo;
			currentWeapon.TryToActivate();
			Debug.Log( "[SWITCHER] activating: " + currentWeapon.AbilityName);
			OnSwitchWeapon?.Invoke(currentWeapon == Primary_Weapon ? 1 : 2);
		}
	}
}
