using UnityEngine;
using UnityEngine.Serialization;

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
	public HUDCashDisplay hudCashDisplayForUpgrades;
	public HUDGasDisplay hudGasDisplay;
	[FormerlySerializedAs("gangstaBeanAmmoChanger")] public HUDAmmoChanger hudAmmoChanger;

	public  void SetPlayer(Player newPlayer)
	{
		currentPlayer = newPlayer;
		hudHealthDisplay.SetPlayer(currentPlayer);
		hudKillsDisplay.SetPlayer(currentPlayer);
		hudCashDisplay.SetPlayer(currentPlayer);
		hudCashDisplayForUpgrades.SetPlayer(currentPlayer);
		hudGasDisplay.SetPlayer(currentPlayer);
		if(hudAmmoChanger != null)hudAmmoChanger.SetPlayer(currentPlayer);

		_currentAmmoInventory = newPlayer.SpawnedPlayerGO.GetComponent<AmmoInventory>();
		if (primaryAmmoDisplay != null) primaryAmmoDisplay.SetAmmo(_currentAmmoInventory.primaryAmmo);
		if (secondaryAmmoDisplay != null) secondaryAmmoDisplay.SetAmmo(_currentAmmoInventory.secondaryAmmo);
		if(tertiaryAmmoDisplay != null) tertiaryAmmoDisplay.SetAmmo(_currentAmmoInventory.tertiaryAmmo);

	}

}