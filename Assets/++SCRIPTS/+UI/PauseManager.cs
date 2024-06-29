using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PauseManager : Singleton<PauseManager>
{
	//STATE
	private MenuButton currentlySelectedCharacterButton;
	private Player pausingPlayer;
	public static event Action<Player> OnPause;
	public static event Action<Player> OnUnpause;
	public MenuButtons menuButtons;

	private void Start()
	{
		ListenToJoinedPlayers();
		Level.OnStop += OnLevelStop;
	}

	private void OnLevelStop(Scene.Type type)
	{
		StopListeningToJoinedPlayers();
		Level.OnStop -= OnLevelStop;
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
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void PlayerPressedSelect(NewControlButton newControlButton)
	{
		if (newControlButton.owner != pausingPlayer) return;
		currentlySelectedCharacterButton = menuButtons.GetCurrentButton();
		ASSETS.sounds.pauseMenu_select_sounds.PlayRandom();
		
		switch (currentlySelectedCharacterButton.type)
		{
			case MenuButton.ButtonType.Restart:
				Level.CurrentLevel.RestartLevel();
				break;
			case MenuButton.ButtonType.Resume:
				Unpause();
				break;
			case MenuButton.ButtonType.MainMenu:
				Unpause();
				Level.CurrentLevel.ExitToMainMenu();
				break;
			case MenuButton.ButtonType.Quit:
				Game_GlobalVariables.QuitGame();
				break;
		}
	}


	private void PlayerPressedUp(IControlAxis controlAxis)
	{
		if (controlAxis.owner != pausingPlayer) return;
		menuButtons.Up();
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void PlayerPressedCancel(NewControlButton newControlButton)
	{
		if (newControlButton.owner != pausingPlayer) return;
		Unpause();
	}

	private void PlayerPressedPause(NewControlButton newControlButton)
	{
		
		if (Game_GlobalVariables.IsPaused)
		{
			if (pausingPlayer != newControlButton.owner) return;
			Unpause();
		}
		else
			Pause(newControlButton.owner);
	}
	

	private void Pause(Player player)
	{
		if (Game_GlobalVariables.IsPaused) return;
		gameObject.SetActive(true);
		OnPause?.Invoke(player);
		pausingPlayer = player;

		ASSETS.sounds.pauseMenu_start_sounds.PlayRandom();
		Time.timeScale = 0;
	menuButtons.InitButtons();
	}

	private void Unpause()
	{
		if (!Game_GlobalVariables.IsPaused) return;
		gameObject.SetActive(false);
		OnUnpause?.Invoke(pausingPlayer);

		ASSETS.sounds.pauseMenu_stop_sounds.PlayRandom();
		Time.timeScale = 1;
		menuButtons.UnhighlightButtons();
		pausingPlayer = null;
	}

	

	
}