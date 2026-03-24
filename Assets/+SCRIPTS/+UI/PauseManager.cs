using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class PauseManager : MonoBehaviour, IService
	{
		//STATE
		MenuButton currentlySelectedCharacterButton;
		Player pausingPlayer;
		public event Action<Player> OnPause;
		public event Action<Player> OnUnpause;
		public event Action OnPlayerPressedSelect;
		public event Action OnPlayerPressedUp;
		public event Action OnPlayerPressedDown;
		public MenuButtons menuButtons;
		public GameObject graphic;
		public bool IsPaused;
		List<Player> playersBeingListenedTo = new();

		public void StartService()
		{
			Debug.Log("pause manager start");
			Services.levelManager.OnLevelSpawnedPlayerFromLevel += ListenToJoinedLevelSpawnedPlayer;
			Services.levelManager.OnLevelSpawnedPlayerFromPlayerSetupMenu += ListenToJoinedLevelSpawnedPlayer;

			if (graphic != null)
				graphic.SetActive(false);
		}

		void ListenToJoinedLevelSpawnedPlayer(Player newPlayer)
		{
			if (playersBeingListenedTo.Contains(newPlayer)) return;
			playersBeingListenedTo.Add(newPlayer);
			newPlayer.Controller.Pause.OnPress += PlayerPressedPause;
			newPlayer.Controller.Unpause.OnPress += PlayerPressedPause;
			newPlayer.Controller.Select.OnPress += PlayerPressedSelect;
			newPlayer.Controller.Cancel.OnPress += PlayerPressedCancel;
			newPlayer.Controller.UIAxis.OnUp += PlayerPressedUp;
			newPlayer.Controller.UIAxis.OnDown += PlayerPressedDown;
		}

		void OnDisable()
		{
			StopListeningToJoinedPlayers();
			Services.levelManager.OnLevelSpawnedPlayerFromLevel -= ListenToJoinedLevelSpawnedPlayer;
			Services.levelManager.OnLevelSpawnedPlayerFromPlayerSetupMenu -= ListenToJoinedLevelSpawnedPlayer;
			 OnPause = null;
			 OnUnpause = null;
			 OnPlayerPressedSelect = null;
			 OnPlayerPressedUp = null;
			 OnPlayerPressedDown = null;
			 menuButtons = null;
			 pausingPlayer = null;
			 currentlySelectedCharacterButton = null;
		}

		void StopListeningToJoinedPlayers()
		{

			foreach (var joinedPlayer in playersBeingListenedTo)
			{
				joinedPlayer.Controller.Pause.OnPress -= PlayerPressedPause;
				joinedPlayer.Controller.Unpause.OnPress -= PlayerPressedPause;
				joinedPlayer.Controller.Select.OnPress -= PlayerPressedSelect;
				joinedPlayer.Controller.Cancel.OnPress -= PlayerPressedCancel;
				joinedPlayer.Controller.UIAxis.OnUp -= PlayerPressedUp;
				joinedPlayer.Controller.UIAxis.OnDown -= PlayerPressedDown;
			}

			playersBeingListenedTo.Clear();
		}

		void PlayerPressedDown(IControlAxis controlAxis)
		{
			if (controlAxis.owner != pausingPlayer) return;

			menuButtons.Down();
			OnPlayerPressedDown?.Invoke();
		}

		void PlayerPressedSelect(NewControlButton newControlButton)
		{
			if (newControlButton.owner != pausingPlayer) return;
			currentlySelectedCharacterButton = menuButtons.GetCurrentButton();
			OnPlayerPressedSelect?.Invoke();

			switch (currentlySelectedCharacterButton.type)
			{
				case MenuButton.ButtonType.Restart:
					Unpause();
					Services.levelManager.RestartLevel();
					break;
				case MenuButton.ButtonType.Resume:
					Unpause();
					break;
				case MenuButton.ButtonType.MainMenu:
					Unpause();
					Services.levelManager.ExitToMainMenu();
					break;
				case MenuButton.ButtonType.Quit:
					Services.levelManager.QuitGame();
					break;
				case MenuButton.ButtonType.Respawn:
					var keepPlayer = pausingPlayer;
					Unpause();
					Services.levelManager.UnspawnPlayer(keepPlayer);
					Services.levelManager.RespawnPlayer(keepPlayer);
					break;
				case MenuButton.ButtonType.Unspawn:
					var unspawnPlayer = pausingPlayer;
					Unpause();
					Services.levelManager.UnspawnPlayer(unspawnPlayer);
					break;
			}
		}

		void PlayerPressedUp(IControlAxis controlAxis)
		{
			if (controlAxis.owner != pausingPlayer) return;

			menuButtons.Up();
			OnPlayerPressedUp?.Invoke();
		}

		void PlayerPressedCancel(NewControlButton newControlButton)
		{
			if (newControlButton.owner != pausingPlayer) return;
			Unpause();
		}

		void PlayerPressedPause(NewControlButton newControlButton)
		{
			if (IsPaused)
			{
				if (pausingPlayer != newControlButton.owner) return;
				Unpause();
			}
			else
				Pause(newControlButton.owner);
		}

		void Pause(Player player)
		{
			if (IsPaused) return;
			graphic.SetActive(true);
			OnPause?.Invoke(player);
			IsPaused = true;
			pausingPlayer = player;
			Services.playerManager.SetActionMaps(Players.UIActionMap);

			Time.timeScale = 0;
			menuButtons.InitButtons();
		}

		void Unpause()
		{
			if (!IsPaused) return;
			graphic.SetActive(false);
			OnUnpause?.Invoke(pausingPlayer);
			IsPaused = false;
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);

			Time.timeScale = 1;
			menuButtons.UnhighlightButtons();
			pausingPlayer = null;
		}
	}
}
