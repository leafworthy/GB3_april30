using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDSlot : MonoBehaviour
	{
		public PlayerSetupMenu charSelectMenu;
		private Player currentPlayer;
		public GameObject characterHUD;
		private bool IsInMenu;

		public bool IsActive { get; private set; }

		public void SetPlayer(Player player)
		{

			if (IsActive && !IsInMenu)
			{
				return;
			}

			if (currentPlayer != null)
			{
				currentPlayer.OnPlayerDies -= SetCharacterHudInvisible;
			}

			IsActive = true;
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
			if(currentPlayer != null)currentPlayer.OnPlayerDies -= SetCharacterHudInvisible;
			IsActive = false;
		}



		private void SetCharacterHudVisible(Character currentCharacter)
		{
			if (currentPlayer == null) return;
			SetPlayer(currentPlayer);
			charSelectMenu.gameObject.SetActive(false);
			characterHUD.gameObject.SetActive(true);

		}


		private void SetCharacterHudInvisible(Player player, bool b)
		{
			IsActive = false;
			currentPlayer.OnPlayerDies -= SetCharacterHudInvisible;
			currentPlayer = null;
			characterHUD.gameObject.SetActive(false);

		}

		public void OpenCharacterSelectMenu(Player player)
		{
			currentPlayer = player;
			characterHUD.gameObject.SetActive(false);
			charSelectMenu.StartSetupMenu(player);
			IsActive = true;
			IsInMenu = true;
		}


	}
}
