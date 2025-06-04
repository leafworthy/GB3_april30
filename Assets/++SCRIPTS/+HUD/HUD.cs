using System;
using GangstaBean.UI.HUD;
using UnityEngine;

namespace GangstaBean.UI.HUD
{
	public enum Character
	{
		None,
		Karrot,
		Bean,
		Brock,
		Tmato
	}

	public enum WeaponType
	{
		ak47,
		nade,
		glock,
		kunai,
		bat,
		chargedBat,
		knife,
		shotgun,
		mine,
		chainsaw
	}

	public class HUD : MonoBehaviour, INeedPlayer
	{
		private AmmoInventory _currentAmmoInventory;
		public AmmoDisplay primaryAmmoDisplay;
		public HideRevealObjects primaryWeaponIcon;
		public AmmoDisplay secondaryAmmoDisplay;
		public HideRevealObjects secondaryWeaponIcon;
		public AmmoDisplay tertiaryAmmoDisplay;
		public HideRevealObjects tertiaryWeaponIcon;

		public HideRevealObjects CharIcon;

		public void SetPlayer(Player newPlayer)
		{
			gameObject.SetActive(true);

			_currentAmmoInventory = newPlayer.SpawnedPlayerGO.GetComponent<AmmoInventory>();
			if (primaryAmmoDisplay != null) primaryAmmoDisplay.SetAmmo(_currentAmmoInventory.primaryAmmo);
			if (secondaryAmmoDisplay != null) secondaryAmmoDisplay.SetAmmo(_currentAmmoInventory.secondaryAmmo);
			if (tertiaryAmmoDisplay != null) tertiaryAmmoDisplay.SetAmmo(_currentAmmoInventory.tertiaryAmmo);

			var playerAmmo = newPlayer.SpawnedPlayerGO.GetComponent<AmmoInventory>();

			primaryWeaponIcon.Set((int)playerAmmo.primaryAmmo.weaponType);
			secondaryWeaponIcon.Set((int) playerAmmo.secondaryAmmo.weaponType);
			tertiaryWeaponIcon.Set((int) playerAmmo.tertiaryAmmo.weaponType);

			CharIcon.Set((int)newPlayer.CurrentCharacter); // Set to the first character icon by default
		}




	}
}
