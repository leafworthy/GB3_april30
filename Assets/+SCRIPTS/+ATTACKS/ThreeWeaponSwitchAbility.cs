using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class ThreeWeaponSwitchAbility : MonoBehaviour, INeedPlayer
	{
		public WeaponAbility Primary_Weapon;
		public WeaponAbility Secondary_Weapon;
		public WeaponAbility Tertiary_Weapon;
		public WeaponAbility currentWeapon;

		private WeaponAbility weaponToSwitchTo;
		private Player player;

		private bool hasInitialized;
		private bool isPressingTertiary;
		private bool isPressingSecondary;
		private bool isPressingPrimary;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			if (hasInitialized)
			{
				Debug.LogWarning(" [SWITCHER] already initialized, skipping");
				return;
			}

			hasInitialized = true;

			player.Controller.InteractRightShoulder.OnPress += Player_SwapTertiary;
			player.Controller.InteractRightShoulder.OnRelease += Player_ReleaseTertiary;

			player.Controller.Attack2LeftTrigger.OnPress += Player_SwapSecondary;
			player.Controller.Attack2LeftTrigger.OnRelease += Player_ReleaseSecondary;

			player.Controller.Attack1RightTrigger.OnPress += Player_SwapPrimary;
			player.Controller.Attack1RightTrigger.OnRelease += Player_ReleasePrimary;
			StartSwitchingWeapons(Primary_Weapon);
		}

		private void Player_ReleasePrimary(NewControlButton obj)
		{
			isPressingPrimary = false;
		}

		private void Player_ReleaseSecondary(NewControlButton obj) => isPressingSecondary = false;

		private void Player_ReleaseTertiary(NewControlButton obj) => isPressingTertiary = false;

		private void Player_SwapPrimary(NewControlButton obj)
		{
			isPressingPrimary = true;
			StartSwitchingWeapons(Primary_Weapon);
		}

		private void Player_SwapSecondary(NewControlButton obj)
		{
			isPressingSecondary = true;
			StartSwitchingWeapons(Secondary_Weapon);
		}

		private void Player_SwapTertiary(NewControlButton obj)
		{
			isPressingTertiary = true;
			StartSwitchingWeapons(Tertiary_Weapon);
		}

		private void Update()
		{
			//if (isPressingTertiary && weaponToSwitchTo != Tertiary_Weapon) StartSwitchingWeapons(Tertiary_Weapon);
			//if (isPressingSecondary && weaponToSwitchTo != Secondary_Weapon) StartSwitchingWeapons(Secondary_Weapon);
			//if (isPressingPrimary && weaponToSwitchTo != Primary_Weapon) StartSwitchingWeapons(Primary_Weapon);
		}

		private void OnDestroy()
		{
			if (player == null) return;
			player.Controller.InteractRightShoulder.OnPress -= Player_SwapTertiary;
			player.Controller.InteractRightShoulder.OnRelease -= Player_ReleaseTertiary;

			player.Controller.Attack2LeftTrigger.OnPress -= Player_SwapSecondary;
			player.Controller.Attack2LeftTrigger.OnRelease -= Player_ReleaseSecondary;

			player.Controller.Attack1RightTrigger.OnPress -= Player_SwapPrimary;
			player.Controller.Attack1RightTrigger.OnRelease -= Player_ReleasePrimary;
		}

		private void StartSwitchingWeapons(WeaponAbility _weaponToSwitchTo)
		{
			if (currentWeapon == null)
			{
				Debug.Log("[SWITCHER]initial weapon equip: " + _weaponToSwitchTo.AbilityName);
				SwitchCurrentWeapon(Primary_Weapon);
				return;
			}

			if (_weaponToSwitchTo == currentWeapon) return;
			if (!currentWeapon.canStop(null))
			{
				Debug.Log("[SWITCHER] can't switch right now, busy with: " + currentWeapon.AbilityName);
				return;
			}

			Debug.Log("[SWITCHER] Start switching to: " + _weaponToSwitchTo.AbilityName);
			weaponToSwitchTo = _weaponToSwitchTo;
			SwitchCurrentWeapon(weaponToSwitchTo);
		}

		private void SwitchCurrentWeapon(WeaponAbility _weaponToSwitchTo)
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
			currentWeapon.Try();
		}
	}
}
