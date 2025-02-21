using System;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class HUDSlot : MonoBehaviour
{
	public HUD BeanHUD;
	public HUD BrockHUD;
	public HUD KarrotHUD;
	public HUD TmatoHUD;
	public PlayerSetupMenu charSelectMenu;
	public UpgradeSetupMenu upgradeSetupMenu;

	public void StartCharSelectMenu(Player player)
	{
		Debug.Log("here for player  " + player.playerIndex);
		player.input.uiInputModule = charSelectMenu.GetComponentInChildren<InputSystemUIInputModule>();
		DisableAllHUDS();
		charSelectMenu.gameObject.SetActive(true);
		charSelectMenu.Setup(player,this);
		charSelectMenu.OnCharacterChosen += SetCharacter;
	}

	public void StartUpgradeSelectMenu(Player player)
	{
		upgradeSetupMenu.gameObject.SetActive(true);
		upgradeSetupMenu.Setup(player, this);
		upgradeSetupMenu.OnUpgradePurchased += CloseUpgradeSelectMenu;
	}

	private void CloseUpgradeSelectMenu(Player player)
	{
		upgradeSetupMenu.gameObject.SetActive(false);
	}

	public void SetCharacter(Player player)
	{
		DisableAllHUDS();
		switch (player.CurrentCharacter)
		{
			case Character.Karrot:
				KarrotHUD.gameObject.SetActive(true);
				KarrotHUD.gameObject.GetComponent<HUD>().SetPlayer(player);
				break;
			case Character.Bean:
				BeanHUD.gameObject.SetActive(true);
				BeanHUD.gameObject.GetComponent<HUD>().SetPlayer(player);
				break;
			case Character.Brock:
				BrockHUD.gameObject.SetActive(true);
				BrockHUD.gameObject.GetComponent<HUD>().SetPlayer(player);
				break;
			case Character.Tmato:
				TmatoHUD.gameObject.SetActive(true);
				TmatoHUD.gameObject.GetComponent<HUD>().SetPlayer(player);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(player.CurrentCharacter), player.CurrentCharacter, null);
		}

	}

	private void DisableAllHUDS()
	{
		charSelectMenu.gameObject.SetActive(false);
		BeanHUD.gameObject.SetActive(false);
		BrockHUD.gameObject.SetActive(false);
		KarrotHUD.gameObject.SetActive(false);
		TmatoHUD.gameObject.SetActive(false);
	}


	
}