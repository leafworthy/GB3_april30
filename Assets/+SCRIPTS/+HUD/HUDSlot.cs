using __SCRIPTS.HUD_Displays;
using __SCRIPTS.UpgradeS;
using GangstaBean.Core;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace __SCRIPTS
{
	public class HUDSlot : MonoBehaviour
	{
		public PlayerSetupMenu charSelectMenu;
		public UpgradeSetupMenu upgradeSetupMenu;
		private Player currentPlayer;
		public GameObject characterHUD;
		

		public void SetPlayer(Player player)
		{
			Debug.Log("slot set to player: " + player.name);
			currentPlayer = player;

			// Ensure player Stats are initialized before setting up HUD components
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null)
			{
				// Force initialization if not already done
				playerStats.InitStats();
			}

			foreach (var needPlayer in GetComponentsInChildren<INeedPlayer>(true))
			{
				// Skip the character select menu since it's already handled
				if (needPlayer.GetType() == typeof(PlayerSetupMenu)) continue;
				needPlayer.SetPlayer(currentPlayer);
			}

			charSelectMenu.gameObject.SetActive(false);
			characterHUD.gameObject.SetActive(true);
		}

		private void OnEnable()
		{
			charSelectMenu.OnCharacterChosen += SetCharacterHudVisible;
		}

		private void OnDisable()
		{
			charSelectMenu.OnCharacterChosen -= SetCharacterHudVisible;
		}

		public void OpenUpgradeSelectMenu(Player player)
		{
			upgradeSetupMenu.gameObject.SetActive(true);
			upgradeSetupMenu.StartUpgradeSelectMenu(player);
			Services.pauseManager.OnPause += CloseAllUpgradeSelectMenus;
			upgradeSetupMenu.OnUpgradeExit += CloseUpgradeSelectMenu;
		}

		private void CloseAllUpgradeSelectMenus(Player p)
		{
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				if (player == null) continue;
				player.LeaveUpgradeSetupMenu();
			}
		}

		private void CloseUpgradeSelectMenu(Player player)
		{
			Services.pauseManager.OnPause -= CloseUpgradeSelectMenu;
			upgradeSetupMenu.OnUpgradeExit -= CloseUpgradeSelectMenu;
			upgradeSetupMenu.gameObject.SetActive(false);
			if (player == null) return;
			player.LeaveUpgradeSetupMenu();
		}

		private void SetCharacterHudVisible(Character currentCharacter)
		{
			if (currentPlayer == null) return;
			SetPlayer(currentPlayer);
			charSelectMenu.gameObject.SetActive(false);
			characterHUD.gameObject.SetActive(true);

		}

		public void OpenCharacterSelectMenu(Player player)
		{
			currentPlayer = player;
			characterHUD.gameObject.SetActive(false);
			charSelectMenu.gameObject.SetActive(true);
			charSelectMenu.StartSetupMenu(player);
		}
	}
}
