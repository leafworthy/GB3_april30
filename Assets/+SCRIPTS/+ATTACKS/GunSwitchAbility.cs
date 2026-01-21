using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class GunSwitchAbility : WeaponAbility
	{
		public Gun Primary_Weapon;
		public Gun Secondary_Weapon;
		public Gun Tertiary_Weapon;
		public Gun currentWeapon;

		bool hasInitialized;

		public override void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			if (hasInitialized)
			{
				Debug.LogWarning(" [SWITCHER] already initialized, skipping");
				return;
			}

			hasInitialized = true;

			player.Controller.InteractRightShoulder.OnPress += Player_SwapTertiary;

			player.Controller.Attack2LeftTrigger.OnPress += Player_SwapSecondary;

			player.Controller.Attack1RightTrigger.OnPress += Player_SwapPrimary;
			StartSwitchingWeapons(Primary_Weapon);
		}

		void Player_SwapPrimary(NewControlButton obj)
		{
			StartSwitchingWeapons(Primary_Weapon);
		}

		void Player_SwapSecondary(NewControlButton obj)
		{
			StartSwitchingWeapons(Secondary_Weapon);
		}

		void Player_SwapTertiary(NewControlButton obj)
		{
			StartSwitchingWeapons(Tertiary_Weapon);
		}

		void OnDestroy()
		{
			if (player == null) return;
			player.Controller.InteractRightShoulder.OnPress -= Player_SwapTertiary;

			player.Controller.Attack2LeftTrigger.OnPress -= Player_SwapSecondary;

			player.Controller.Attack1RightTrigger.OnPress -= Player_SwapPrimary;
		}

		void StartSwitchingWeapons(Gun _weaponToSwitchTo)
		{
			if (currentWeapon == null)
			{
				Debug.Log("[SWITCHER]initial weapon equip: " + _weaponToSwitchTo.name);
				SwitchCurrentWeapon(Primary_Weapon);
				return;
			}

			if (_weaponToSwitchTo == currentWeapon) return;

			Debug.Log("[SWITCHER] Start switching to: " + _weaponToSwitchTo.name);
			SwitchCurrentWeapon(_weaponToSwitchTo);
		}

		void SwitchCurrentWeapon(Gun _weaponToSwitchTo)
		{
			Debug.Log("trying to switch weapon");

			if (_weaponToSwitchTo == null)
			{
				Debug.LogError(" [SWITCHER] weapon to switch to is null!");
				return;
			}

			Debug.Log("[SWITCHER]SwitchCurrentWeapon to: " + _weaponToSwitchTo.name);
			if (_weaponToSwitchTo.CanUse())
			{
				Debug.Log("[SWITCHER] can do, switching to: " + _weaponToSwitchTo.name);
				currentWeapon = _weaponToSwitchTo;
			}
			else
				Debug.Log(" [SWITCHER] can't do, switching to primary: " + Primary_Weapon.name);

			if (currentWeapon == null)
			{
				currentWeapon = Primary_Weapon;
				Debug.Log(" [SWITCHER] current weapon was null, defaulting to primary: " + Primary_Weapon.name);
			}

			Debug.Log("[SWITCHER] doing weapon: " + currentWeapon.name);
			PullOutWeapon();
		}
	}
}
