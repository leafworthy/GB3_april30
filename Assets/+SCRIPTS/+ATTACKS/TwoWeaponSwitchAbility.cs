using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class TwoWeaponSwitchAbility : MonoBehaviour, INeedPlayer
	{
		public GunAttack Primary_Weapon;
		public WeaponAbility Secondary_Weapon;
		public WeaponAbility currentWeapon;
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

		void StartSwitchingWeapons(WeaponAbility _weaponToSwitchTo)
		{
			if (_weaponToSwitchTo == null) return;
			if (currentWeapon == null)
			{
				Debug.Log("[SWITCHER]initial weapon equip: " + _weaponToSwitchTo.AbilityName);
				SwitchCurrentWeapon(Secondary_Weapon);
				return;
			}

			if (_weaponToSwitchTo == currentWeapon) return;
			if (!currentWeapon.canStop(null) && currentWeapon.isActive)
			{
				Debug.Log("[SWITCHER] can't switch weapons right now, busy with: " + currentWeapon.AbilityName);
				return;
			}

			Debug.Log("[SWITCHER] Start switching to: " + _weaponToSwitchTo.AbilityName);
			SwitchCurrentWeapon(_weaponToSwitchTo);
		}

		void SwitchCurrentWeapon(WeaponAbility _weaponToSwitchTo)
		{
			Debug.Log("trying to switch weapon");

			if (_weaponToSwitchTo == null)
			{
				Debug.LogError(" [SWITCHER] weapon to switch to is null!");
				return;
			}

			if (currentWeapon == null)
			{
				currentWeapon = Primary_Weapon;
				Debug.Log(" [SWITCHER] current weapon was null, defaulting to primary: " + Primary_Weapon.AbilityName);
			}

			Debug.Log("[SWITCHER]SwitchCurrentWeapon to: " + _weaponToSwitchTo.AbilityName);
			if (_weaponToSwitchTo.canDo())
			{
				Debug.Log("[SWITCHER] can do, switching to: " + _weaponToSwitchTo.AbilityName);
				currentWeapon.StopAbility();
				currentWeapon = _weaponToSwitchTo;
				currentWeapon.TryToActivate();
				OnSwitchWeapon?.Invoke(currentWeapon == Primary_Weapon ? 1 : 2);
			}
			else
			{
				currentWeapon = Primary_Weapon;
				currentWeapon.TryToActivate();
				OnSwitchWeapon?.Invoke(1);
				Debug.Log("[SWITCHER] switching to Primary_Weapon: " + currentWeapon.AbilityName);
			}
		}
	}
}
