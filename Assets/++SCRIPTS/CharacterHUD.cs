using UnityEngine;

public class CharacterHUD:MonoBehaviour
{


	private Player currentPlayer;
	private AmmoHandler currentAmmoHandler;
	public HealthDisplay healthDisplay;
	public AmmoDisplay primaryAmmoDisplay;
	public AmmoDisplay secondaryAmmoDisplay;
	public AmmoDisplay tertiaryAmmoDisplay;
	public ZombieDisplay zombieDisplay;
	public CashDisplay cashDisplay;


	public  void SetPlayer(Player newPlayer)
	{
		 currentPlayer = newPlayer;
		 healthDisplay.SetPlayer(currentPlayer);
		 zombieDisplay.SetPlayer(currentPlayer);
		 cashDisplay.SetPlayer(currentPlayer);

		 currentAmmoHandler = newPlayer.SpawnedPlayerGO.GetComponent<AmmoHandler>();
		 primaryAmmoDisplay.SetAmmo(currentAmmoHandler.primaryAmmo);
		 secondaryAmmoDisplay.SetAmmo(currentAmmoHandler.secondaryAmmo);
		 tertiaryAmmoDisplay.SetAmmo(currentAmmoHandler.tertiaryAmmo);

	}
}
