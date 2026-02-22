using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
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
		chainsaw,
		shield
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

		[Button]
		public void SetButtons()
		{
			primaryAmmoDisplay.SetButton(WeaponButton.buttons.R2);
			secondaryAmmoDisplay.SetButton(WeaponButton.buttons.L2);
			tertiaryAmmoDisplay.SetButton(WeaponButton.buttons.Circle);
		}

		[Button]
		public void SetButtonsTmato()
		{
			primaryAmmoDisplay.SetButton(WeaponButton.buttons.R2);
			secondaryAmmoDisplay.SetButton(WeaponButton.buttons.Square);
			tertiaryAmmoDisplay.SetButton(WeaponButton.buttons.L2);
		}
		public void SetPlayer(Player newPlayer)
		{
			gameObject.SetActive(true);
			if(newPlayer.CurrentCharacter == Character.Tmato)SetButtonsTmato();
			else SetButtons();

			_currentAmmoInventory = newPlayer.SpawnedPlayerGO.GetComponent<AmmoInventory>();
			if (primaryAmmoDisplay != null)
			{
				primaryAmmoDisplay.SetAmmo(_currentAmmoInventory.primaryAmmo);
			}
			if (secondaryAmmoDisplay != null)
			{
				secondaryAmmoDisplay.SetAmmo(_currentAmmoInventory.secondaryAmmo);
			}
			if (tertiaryAmmoDisplay != null)
			{
				tertiaryAmmoDisplay.SetAmmo(_currentAmmoInventory.tertiaryAmmo);
			}

			var playerAmmo = newPlayer.SpawnedPlayerGO.GetComponent<AmmoInventory>();

			primaryWeaponIcon.Set((int)playerAmmo.primaryAmmo.weaponType);
			secondaryWeaponIcon.Set((int) playerAmmo.secondaryAmmo.weaponType);
			tertiaryWeaponIcon.Set((int) playerAmmo.tertiaryAmmo.weaponType);

			CharIcon.Set((int)newPlayer.CurrentCharacter); // Set to the first character icon by default
		}




	}
}
