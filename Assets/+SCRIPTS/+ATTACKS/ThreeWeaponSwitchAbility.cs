using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
	public class ThreeWeaponSwitchAbility : MonoBehaviour, INeedPlayer
	{
		public WeaponAbility Primary_Weapon;
		public WeaponAbility F_Weapon;
		public WeaponAbility E_Weapon;
		public WeaponAbility currentWeapon;

		private WeaponAbility weaponToSwitchTo;
		private Player player;

		private bool hasInitialized = false;

		public void SetPlayer(Player _player)
		{
			player = _player;
			if (hasInitialized)
			{
				Debug.LogWarning(" [SWITCHER] already initialized, skipping");
				return;
			}
			hasInitialized = true;

			player.Controller.InteractRightShoulder.OnPress += Player_SwapWeapon_E;
			player.Controller.Attack2LeftTrigger.OnPress += Player_SwapSecondary;
			player.Controller.Attack1RightTrigger.OnPress += Player_SwapPrimary;
			StartSwitchingWeapons(Primary_Weapon);
		}

		private void Player_SwapSecondary(NewControlButton obj) => StartSwitchingWeapons(F_Weapon);
		private void Player_SwapWeapon_E(NewControlButton obj) => StartSwitchingWeapons(E_Weapon);
		private void Player_SwapPrimary(NewControlButton obj) => StartSwitchingWeapons(Primary_Weapon);

		private void OnDestroy()
		{
			if (player == null) return;
			player.Controller.InteractRightShoulder.OnPress -= Player_SwapWeapon_E;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_SwapSecondary;
			player.Controller.Attack1RightTrigger.OnPress -= Player_SwapPrimary;
		}

		private void StartSwitchingWeapons(WeaponAbility _weaponToSwitchTo)
		{
			if (currentWeapon == null)
			{
				Debug.Log( "[SWITCHER]initial weapon equip: " + _weaponToSwitchTo.AbilityName);
				SwitchCurrentWeapon(Primary_Weapon);
				return;
			}
			if (_weaponToSwitchTo == currentWeapon) return;
			if (!currentWeapon.canStop(null))
			{
				Debug.Log( "[SWITCHER] can't switch right now, busy with: " + currentWeapon.AbilityName);
				return;
			}
 			Debug.Log( "[SWITCHER] Start switching to: " + _weaponToSwitchTo.AbilityName);
			weaponToSwitchTo = _weaponToSwitchTo;
			currentWeapon.PutAway();
			currentWeapon.OnPutAwayComplete += CurrentWeapon_OnPutAwayComplete;
		}

		private void CurrentWeapon_OnPutAwayComplete()
		{
			currentWeapon.OnPutAwayComplete -= CurrentWeapon_OnPutAwayComplete;
			SwitchCurrentWeapon(weaponToSwitchTo);
		}
		private void SwitchCurrentWeapon(WeaponAbility _weaponToSwitchTo)
		{
			if (_weaponToSwitchTo == null)
			{
				Debug.LogError(" [SWITCHER] weapon to switch to is null!");
				return;
			}
			Debug.Log( "[SWITCHER]SwitchCurrentWeapon to: " + _weaponToSwitchTo.AbilityName);
			if (_weaponToSwitchTo.canDo())
			{
				Debug.Log( "[SWITCHER] can do, switching to: " + _weaponToSwitchTo.AbilityName);
				currentWeapon = _weaponToSwitchTo;
			}
			else
			{
				Debug.Log(" [SWITCHER] can't do, switching to primary: " + Primary_Weapon.AbilityName);
			}

			if (currentWeapon == null)
			{
				currentWeapon = Primary_Weapon;
				Debug.Log(" [SWITCHER] current weapon was null, defaulting to primary: " + Primary_Weapon.AbilityName);
			}
			Debug.Log( "[SWITCHER] doing weapon: " + currentWeapon.AbilityName);
			currentWeapon.Do();
		}


	}
}
