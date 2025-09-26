using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDSlot : MonoBehaviour
	{
		public PlayerSetupMenu charSelectMenu;
		private Player currentPlayer;
		public GameObject characterHUD;


		public void SetPlayer(Player player)
		{
			if (player == null)
			{
				characterHUD.gameObject.SetActive(false);
				return;
			}
			currentPlayer = player;

			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null)
			{
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
			currentPlayer.OnPlayerDies += SetCharacterHudInvisible;
		}

		private void OnEnable()
		{
			charSelectMenu.OnCharacterChosen += SetCharacterHudVisible;
		}

		private void OnDisable()
		{
			charSelectMenu.OnCharacterChosen -= SetCharacterHudVisible;
		}



		private void SetCharacterHudVisible(Character currentCharacter)
		{
			if (currentPlayer == null) return;
			SetPlayer(currentPlayer);
			charSelectMenu.gameObject.SetActive(false);
			characterHUD.gameObject.SetActive(true);

		}

		private void SetCharacterHudInvisible(Player player)
		{
			if (currentPlayer == null) return;
			if (currentPlayer != player) return;
			currentPlayer = null;
			SetPlayer(null);
			charSelectMenu.gameObject.SetActive(false);
			characterHUD.gameObject.SetActive(false);

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
