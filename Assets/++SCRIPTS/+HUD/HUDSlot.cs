using System;
using __SCRIPTS.HUD_Displays;
using __SCRIPTS.UpgradeS;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace __SCRIPTS
{
	public class HUDSlot : MonoBehaviour
	{
		public HUD BeanHUD;
		public HUD BrockHUD;
		public HUD KarrotHUD;
		public HUD TmatoHUD;
		public PlayerSetupMenu charSelectMenu;
		public UpgradeSetupMenu upgradeSetupMenu;
		private Player currentPlayer;
	

		public void SetPlayer(Player player)
		{
			currentPlayer = player;
			foreach (var needPlayer in GetComponentsInChildren<INeedPlayer>(true))
			{
				needPlayer.SetPlayer(currentPlayer);
			}
			if(currentPlayer.CurrentCharacter != Character.None) SetCharacterHudVisible(currentPlayer.CurrentCharacter);
		}

		public void StartCharSelectMenu(Player player)
		{
			Debug.Log("here for player  " + player.playerIndex);
			player.input.uiInputModule = charSelectMenu.GetComponentInChildren<InputSystemUIInputModule>();
			DisableAllHUDS();
			charSelectMenu.gameObject.SetActive(true);
			charSelectMenu.StartSetupMenu(player);
		
			charSelectMenu.OnCharacterChosen += SetCharacterHudVisible;
		}

		void OnDisable()
		{
			charSelectMenu.OnCharacterChosen -= SetCharacterHudVisible;
		}

		public void OpenUpgradeSelectMenu(Player player)
		{
			upgradeSetupMenu.gameObject.SetActive(true);
			upgradeSetupMenu.StartUpgradeSelectMenu(player);
			PauseManager.I.OnPause += CloseAllUpgradeSelectMenus;
			upgradeSetupMenu.OnUpgradeExit += CloseUpgradeSelectMenu;
		}
		
		private void CloseAllUpgradeSelectMenus(Player p)
		{
			
			foreach (var player in Players.AllJoinedPlayers)
			{
				if (player == null) continue;
				player.LeaveUpgradeSetupMenu();
			}
			
		}

		private void CloseUpgradeSelectMenu(Player player)
		{
			PauseManager.I.OnPause -= CloseUpgradeSelectMenu;
			upgradeSetupMenu.OnUpgradeExit -= CloseUpgradeSelectMenu;
			upgradeSetupMenu.gameObject.SetActive(false);
			if(player == null) return;
			player.LeaveUpgradeSetupMenu();
		}

		public void SetCharacterHudVisible(Character currentCharacter)
		{
			if(currentPlayer == null) return;
			DisableAllHUDS();
			switch (currentCharacter)
			{
				case Character.Karrot:
					KarrotHUD.gameObject.SetActive(true);
					KarrotHUD.gameObject.GetComponent<HUD>().SetPlayer(currentPlayer);
					break;
				case Character.Bean:
					BeanHUD.gameObject.SetActive(true);
					BeanHUD.gameObject.GetComponent<HUD>().SetPlayer(currentPlayer);
					break;
				case Character.Brock:
					BrockHUD.gameObject.SetActive(true);
					BrockHUD.gameObject.GetComponent<HUD>().SetPlayer(currentPlayer);
					break;
				case Character.Tmato:
					TmatoHUD.gameObject.SetActive(true);
					TmatoHUD.gameObject.GetComponent<HUD>().SetPlayer(currentPlayer);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(currentPlayer.CurrentCharacter), currentPlayer.CurrentCharacter, null);
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
}