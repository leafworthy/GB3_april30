using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDSlot : MonoBehaviour
	{
		public PlayerSetupMenu charSelectMenu;
		private Player currentPlayer;
		public GameObject characterHUD;

		public bool IsActive { get; private set; }

		public void SetPlayer(Player player)
		{

			if (IsActive)
			{
				Debug.Log("[HUD]here in hudslot already active", this);
				return;
			}
			if (player == null)
			{
				Debug.Log("[HUD]here in hudslot player null or active", this);
				characterHUD.gameObject.SetActive(false);
				charSelectMenu.StopSetupMenu();
				return;
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
			Debug.Log("[HUD] here in hudslot and sets player", this);
			characterHUD.gameObject.SetActive(true);
			Debug.Log("[HUD] gets here and registers to die event", this);
			currentPlayer.OnPlayerDies += SetCharacterHudInvisible;
		}

		private void OnEnable()
		{
			charSelectMenu.OnCharacterChosen += SetCharacterHudVisible;
		}

		private void OnDisable()
		{
			charSelectMenu.OnCharacterChosen -= SetCharacterHudVisible;
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
			Debug.Log("got here");
			currentPlayer = null;
			IsActive = false;
			SetPlayer(null);
			characterHUD.gameObject.SetActive(false);

		}

		public void OpenCharacterSelectMenu(Player player)
		{
			currentPlayer = player;
			characterHUD.gameObject.SetActive(false);
			charSelectMenu.StartSetupMenu(player);
			Debug.Log("[HUD] here happened");
			IsActive = true;
		}


	}
}
