
using __SCRIPTS._PLAYER;
using __SCRIPTS._UI;
using UnityEngine;

namespace __SCRIPTS._HUD
{
	public class HUD:MonoBehaviour
	{


		private Player currentPlayer;
		private AmmoInventory _currentAmmoInventory;
		public HUDHealthDisplay hudHealthDisplay;
		public AmmoDisplay primaryAmmoDisplay;
		public AmmoDisplay secondaryAmmoDisplay;
		public AmmoDisplay tertiaryAmmoDisplay;
		public HUDKillsDisplay hudKillsDisplay;
		public HUDCashDisplay hudCashDisplay;
		public HUDGasDisplay hudGasDisplay;

		public  void SetPlayer(Player newPlayer)
		{
			currentPlayer = newPlayer;
			hudHealthDisplay.SetPlayer(currentPlayer);
			hudKillsDisplay.SetPlayer(currentPlayer);
			hudCashDisplay.SetPlayer(currentPlayer);
			hudGasDisplay.SetPlayer(currentPlayer);

			_currentAmmoInventory = newPlayer.SpawnedPlayerGO.GetComponent<AmmoInventory>();
			if (primaryAmmoDisplay != null) primaryAmmoDisplay.SetAmmo(_currentAmmoInventory.primaryAmmo);
			if (secondaryAmmoDisplay != null) secondaryAmmoDisplay.SetAmmo(_currentAmmoInventory.secondaryAmmo);
			if(tertiaryAmmoDisplay != null) tertiaryAmmoDisplay.SetAmmo(_currentAmmoInventory.tertiaryAmmo);

		}

	}
}
