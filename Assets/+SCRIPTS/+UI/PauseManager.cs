using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class PauseManager : ServiceUser, IService
	{
		//STATE
		private MenuButton currentlySelectedCharacterButton;
		private Player pausingPlayer;
		public event Action<Player> OnPause;
		public event Action<Player> OnUnpause;
		public event Action OnPlayerPressedSelect;
		public event Action OnPlayerPressedUp;
		public event Action OnPlayerPressedDown;
		public MenuButtons menuButtons;
		public GameObject graphic;
		private bool isListening;
		public bool IsPaused;
		private List<Player> playersBeingListenedTo = new();


		public void StartService()
		{
			levelManager.OnPlayerSpawned += ListenToJoinedPlayer;

			// Hide menu graphic initially
			if (graphic != null)
				graphic.SetActive(false);
		}

		private void ListenToJoinedPlayer(Player newPlayer)
		{
			if (playersBeingListenedTo.Contains(newPlayer)) return;
			Debug.Log("Listening to joined player");
			playersBeingListenedTo.Add(newPlayer);
			newPlayer.Controller.Pause.OnPress += PlayerPressedPause;
			newPlayer.Controller.Unpause.OnPress += PlayerPressedPause;
			newPlayer.Controller.Select.OnPress += PlayerPressedSelect;
			newPlayer.Controller.Cancel.OnPress += PlayerPressedCancel;
			newPlayer.Controller.UIAxis.OnUp += PlayerPressedUp;
			newPlayer.Controller.UIAxis.OnDown += PlayerPressedDown;
			isListening = true;
		}

		private void OnDisable()
		{
			StopListeningToJoinedPlayers();
			levelManager.OnPlayerSpawned -= ListenToJoinedPlayer;
		}

		private void StopListeningToJoinedPlayers()
		{
			if (!isListening) return;
			isListening = false;
			foreach (var joinedPlayer in playersBeingListenedTo)
			{
				Debug.Log("Level menu stopped listening to player");
				joinedPlayer.Controller.Pause.OnPress -= PlayerPressedPause;
				joinedPlayer.Controller.Unpause.OnPress -= PlayerPressedPause;
				joinedPlayer.Controller.Select.OnPress -= PlayerPressedSelect;
				joinedPlayer.Controller.Cancel.OnPress -= PlayerPressedCancel;
				joinedPlayer.Controller.UIAxis.OnUp -= PlayerPressedUp;
				joinedPlayer.Controller.UIAxis.OnDown -= PlayerPressedDown;
			}

			playersBeingListenedTo.Clear();
		}

		private void PlayerPressedDown(IControlAxis controlAxis)
		{
			if (controlAxis.owner != pausingPlayer) return;
			menuButtons.Down();
			OnPlayerPressedDown?.Invoke();
		}

		private void PlayerPressedSelect(NewControlButton newControlButton)
		{
			if (newControlButton.owner != pausingPlayer) return;
			currentlySelectedCharacterButton = menuButtons.GetCurrentButton();
			OnPlayerPressedSelect?.Invoke();

			switch (currentlySelectedCharacterButton.type)
			{
				case MenuButton.ButtonType.Restart:
					Unpause();
					levelManager.RestartLevel();
					break;
				case MenuButton.ButtonType.Resume:
					Unpause();
					break;
				case MenuButton.ButtonType.MainMenu:
					Unpause();
					levelManager.ExitToMainMenu();
					break;
				case MenuButton.ButtonType.Quit:
					levelManager.QuitGame();
					break;
			}
		}

		private void PlayerPressedUp(IControlAxis controlAxis)
		{
			if (controlAxis.owner != pausingPlayer) return;
			menuButtons.Up();
			OnPlayerPressedUp?.Invoke();
		}

		private void PlayerPressedCancel(NewControlButton newControlButton)
		{
			if (newControlButton.owner != pausingPlayer) return;
			Unpause();
		}

		private void PlayerPressedPause(NewControlButton newControlButton)
		{
			Debug.Log("Pause pressed");
			if (IsPaused)
			{
				if (pausingPlayer != newControlButton.owner) return;
				Unpause();
			}
			else
				Pause(newControlButton.owner);
		}


		/// <summary>
		/// Pause the game with the specified player
		/// </summary>
		public void Pause(Player player)
		{
			if (IsPaused) return;
			Debug.Log("Pause");
			graphic.SetActive(true);
			OnPause?.Invoke(player);
			IsPaused = true;
			pausingPlayer = player;
			playerManager.SetActionMaps(Players.UIActionMap);

			Time.timeScale = 0;
			menuButtons.InitButtons();
		}

		/// <summary>
		/// Unpause the game
		/// </summary>
		public void Unpause()
		{
			if (!IsPaused) return;
			Debug.Log("unPause ");
			graphic.SetActive(false);
			OnUnpause?.Invoke(pausingPlayer);
			IsPaused = false;
			playerManager.SetActionMaps(Players.PlayerActionMap);

			Time.timeScale = 1;
			menuButtons.UnhighlightButtons();
			pausingPlayer = null;
		}
	}
}
