using System;
using __SCRIPTS._COMMON;
using __SCRIPTS._PLAYER;
using __SCRIPTS._SCENES;
using UnityEngine;

namespace __SCRIPTS._UI
{
	public class PauseManager : Singleton<PauseManager>
	{
		//STATE
		private MenuButton currentlySelectedCharacterButton;
		private Player pausingPlayer;
		public static event Action<Player> OnPause;
		public static event Action<Player> OnUnpause;
		public static event Action OnPlayerPressedSelect;
		public static event Action OnPlayerPressedUp;
		public static event Action OnPlayerPressedDown;
		public MenuButtons menuButtons;

		private void Start()
		{
			ListenToJoinedPlayers();
			LevelScene.OnStop += OnLevelStop;
		}

		private void OnLevelStop(Scene.Type type)
		{
			StopListeningToJoinedPlayers();
			LevelScene.OnStop -= OnLevelStop;
		}

		private void ListenToJoinedPlayers()
		{
			foreach (var joinedPlayer in Players.AllJoinedPlayers)
			{
				Debug.Log("level menu listening to player");
				joinedPlayer.Controller.Pause.OnPress += PlayerPressedPause;
				joinedPlayer.Controller.Unpause.OnPress += PlayerPressedPause;
				joinedPlayer.Controller.Select.OnPress += PlayerPressedSelect;
				joinedPlayer.Controller.Cancel.OnPress += PlayerPressedCancel;
				joinedPlayer.Controller.UIAxis.OnUp += PlayerPressedUp;
				joinedPlayer.Controller.UIAxis.OnDown += PlayerPressedDown;
			}
		}

		private void StopListeningToJoinedPlayers()
		{
			foreach (var joinedPlayer in Players.AllJoinedPlayers)
			{
				Debug.Log("level menu  stopped listening to player");
				joinedPlayer.Controller.Pause.OnPress -= PlayerPressedPause;
				joinedPlayer.Controller.Unpause.OnPress -= PlayerPressedPause;
				joinedPlayer.Controller.Select.OnPress -= PlayerPressedSelect;
				joinedPlayer.Controller.Cancel.OnPress -= PlayerPressedCancel;
				joinedPlayer.Controller.UIAxis.OnUp -= PlayerPressedUp;
				joinedPlayer.Controller.UIAxis.OnDown -= PlayerPressedDown;
			}
		}


		private void PlayerPressedDown(IControlAxis controlAxis)
		{
			if (controlAxis.owner != pausingPlayer) return;
			menuButtons.Down();
		}

		private void PlayerPressedSelect(NewControlButton newControlButton)
		{
			if (newControlButton.owner != pausingPlayer) return;
			currentlySelectedCharacterButton = menuButtons.GetCurrentButton();
			OnPlayerPressedSelect?.Invoke();
		
		
			switch (currentlySelectedCharacterButton.type)
			{
				case MenuButton.ButtonType.Restart:
					LevelScene.CurrentLevelScene.RestartLevel();
					break;
				case MenuButton.ButtonType.Resume:
					Unpause();
					break;
				case MenuButton.ButtonType.MainMenu:
					Unpause();
					LevelScene.CurrentLevelScene.ExitToMainMenu();
					break;
				case MenuButton.ButtonType.Quit:
					GlobalManager.QuitGame();
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
		
			if (GlobalManager.IsPaused)
			{
				if (pausingPlayer != newControlButton.owner) return;
				Unpause();
			}
			else
				Pause(newControlButton.owner);
		}
	

		private void Pause(Player player)
		{
			if (GlobalManager.IsPaused) return;
			gameObject.SetActive(true);
			OnPause?.Invoke(player);
			pausingPlayer = player;

		
			Time.timeScale = 0;
			menuButtons.InitButtons();
		}

		private void Unpause()
		{
			if (!GlobalManager.IsPaused) return;
			gameObject.SetActive(false);
			OnUnpause?.Invoke(pausingPlayer);

		
			Time.timeScale = 1;
			menuButtons.UnhighlightButtons();
			pausingPlayer = null;
		}

	

	
	}
}