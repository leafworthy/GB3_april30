using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDSlot : MonoBehaviour
	{
		Player player;
		public GameObject characterHUD;
		public CharacterSelectButtons characterSelectButtons;

		public enum HUDSlotState
		{
			not,
			selectMenu,
			characterHUD,
			claimed
		}

		void SetState(HUDSlotState hudSlotState)
		{
			currentHUDSlotState = hudSlotState;
		}

		public HUDSlotState currentHUDSlotState;

		public void SetPlayer(Player newPlayer)
		{
			if (player != null) return;
			player = newPlayer;
			player.OnPlayerDies += OnPlayerDeath;
			player.InitStats();
			characterSelectButtons.SetPlayer(newPlayer);
			characterSelectButtons.OnCharacterChosen += SetCharacterAndActivateHUD;
		}

		public void SupplyPlayerToINeedPlayerComponents()
		{
			foreach (var needPlayer in GetComponentsInChildren<INeedPlayer>(true))
			{
				needPlayer.SetPlayer(player);
			}
		}



		void SetCharacterAndActivateHUD(Character newCharacter)
		{
			if (player == null) return;
			player.CurrentCharacter = newCharacter;
			Services.levelManager.SpawnPlayerFromPlayerSetupMenu(player);
			OpenCharacterHUD(player);
		}

		void OnPlayerDeath(Player newPlayer, bool b)
		{
			Debug.Log(" [HUD SLOT] disable character hud called");
			HideCharacterHUD();
			SetState(HUDSlotState.claimed);
		}

		void HideCharacterHUD()
		{
			characterHUD.gameObject.SetActive(false);
		}

		void HideCharacterSetupMenu()
		{
			characterSelectButtons.visible.SetActive(false);
		}

		public void OpenCharacterSetupMenu(Player newPlayer)
		{
			SetPlayer(newPlayer);
			SetState(HUDSlotState.selectMenu);
			player.Controller.SetActionMap(Players.UIActionMap);
			characterSelectButtons.visible.SetActive(true);
			characterSelectButtons.ResetCharacterSelection();
			HideCharacterHUD();
		}

		public void OpenCharacterHUD(Player newPlayer)
		{
			SetPlayer(newPlayer);
			SetState(HUDSlotState.characterHUD);
			if (player == null)
			{
				Debug.Log("player null in open character hud", this);
				return;
			}
			if(player.Controller == null)
			{
				Debug.Log(" controller null   ", this);
				return;
			}
			SupplyPlayerToINeedPlayerComponents();
			player.Controller.SetActionMap(Players.PlayerActionMap);
			characterHUD.gameObject.SetActive(true);
			HideCharacterSetupMenu();
		}

		public void ResetHUDSlot()
		{
			player = null;
			currentHUDSlotState = HUDSlotState.not;
		}
	}
}
