using GangstaBean.UI.HUD;
using __SCRIPTS.UpgradeS;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace GangstaBean.UI.HUD
{
	public class HUDSlot : MonoBehaviour
	{
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

			charSelectMenu.gameObject.SetActive(false);
		}

		public void StartCharSelectMenu(Player player)
		{
			Debug.Log("here for player  " + player.playerIndex);
			player.input.uiInputModule = charSelectMenu.GetComponentInChildren<InputSystemUIInputModule>();
			charSelectMenu.gameObject.SetActive(true);
			charSelectMenu.StartSetupMenu(player);

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
			if (player == null) return;
			player.LeaveUpgradeSetupMenu();
		}

		private void SetCharacterHudVisible(Character currentCharacter)
		{
			if (currentPlayer == null) return;
			charSelectMenu.gameObject.SetActive(false);
		}
	}
}