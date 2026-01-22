using System;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
	public class ThreeWeaponSwitchAbility : SerializedMonoBehaviour, INeedPlayer
	{
		public event Action<WeaponAbility> OnSwitchWeapon;
		public WeaponAbility Primary_Weapon;
		public WeaponAbility Secondary_Weapon;
		public WeaponAbility Tertiary_Weapon;
		WeaponAbility currentWeapon;
		WeaponAbility weaponToSwitchTo;
		Player player;

		public void SetPlayer(Player newPlayer)
		{
			ListenToPlayer();
			StartSwitchingWeapons(Primary_Weapon);
		}

		void ListenToPlayer()
		{
			player.Controller.InteractRightShoulder.OnPress += Player_SwapTertiary;
			player.Controller.Attack2LeftTrigger.OnPress += Player_SwapSecondary;
			player.Controller.Attack1RightTrigger.OnPress += Player_SwapPrimary;
		}

		void StopListeningToPlayer()
		{
			if (player == null) return;
			player.Controller.InteractRightShoulder.OnPress -= Player_SwapTertiary;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_SwapSecondary;
			player.Controller.Attack1RightTrigger.OnPress -= Player_SwapPrimary;
		}

		void OnDestroy()
		{
			StopListeningToPlayer();
		}
		void Player_SwapPrimary(NewControlButton _)
		{
			StartSwitchingWeapons(Primary_Weapon);
		}

		void Player_SwapSecondary(NewControlButton _)
		{
			StartSwitchingWeapons(Secondary_Weapon);
		}

		void Player_SwapTertiary(NewControlButton _)
		{
			StartSwitchingWeapons(Tertiary_Weapon);
		}

		void StartSwitchingWeapons(WeaponAbility _weaponToSwitchTo)
		{
			if (currentWeapon == null)
			{
				Debug.Log("[SWITCHER]initial weapon equip: " + _weaponToSwitchTo.AbilityName);
				SwitchCurrentWeapon(Primary_Weapon);
				return;
			}

			if (_weaponToSwitchTo == currentWeapon)
			{
				Debug.Log("[SWITCHER] already using weapon: " + _weaponToSwitchTo.AbilityName);
				return;
			}

			if (!currentWeapon.canStop(currentWeapon))
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

			Debug.Log("[SWITCHER]SwitchCurrentWeapon to: " + _weaponToSwitchTo.AbilityName);
			if (_weaponToSwitchTo.canDo())
			{
				Debug.Log("[SWITCHER] can do, switching to: " + _weaponToSwitchTo.AbilityName);
				currentWeapon = _weaponToSwitchTo;
			}
			else
				Debug.Log(" [SWITCHER] can't do, switching to primary: " + Primary_Weapon.AbilityName);

			if (currentWeapon == null)
			{
				currentWeapon = Primary_Weapon;
				Debug.Log(" [SWITCHER] current weapon was null, defaulting to primary: " + Primary_Weapon.AbilityName);
			}

			Debug.Log("[SWITCHER] doing weapon: " + currentWeapon.AbilityName);
			currentWeapon.TryToDoAbility();
		}

	}
}
