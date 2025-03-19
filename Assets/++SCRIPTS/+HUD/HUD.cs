using UnityEngine;
using UnityEngine.Serialization;


public class HUD:MonoBehaviour
{


	private Player currentPlayer;
	private AmmoInventory _currentAmmoInventory;
	public AmmoDisplay primaryAmmoDisplay;
	public AmmoDisplay secondaryAmmoDisplay;
	public AmmoDisplay tertiaryAmmoDisplay;

	public  void SetPlayer(Player newPlayer)
	{
		currentPlayer = newPlayer;
		foreach (var needPlayer in GetComponentsInChildren<INeedPlayer>(true))
		{
			needPlayer.SetPlayer(currentPlayer);
		}
		
		

		_currentAmmoInventory = newPlayer.SpawnedPlayerGO.GetComponent<AmmoInventory>();
		if (primaryAmmoDisplay != null) primaryAmmoDisplay.SetAmmo(_currentAmmoInventory.primaryAmmo);
		if (secondaryAmmoDisplay != null) secondaryAmmoDisplay.SetAmmo(_currentAmmoInventory.secondaryAmmo);
		if(tertiaryAmmoDisplay != null) tertiaryAmmoDisplay.SetAmmo(_currentAmmoInventory.tertiaryAmmo);

	}

}