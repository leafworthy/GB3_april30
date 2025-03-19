using System;
using System.Collections.Generic;
using UnityEngine;

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
	public GameObject graphic;
	private bool isListening;
	public static bool IsPaused;
	private List<Player> playersBeingListenedTo = new();

	private void Start()
	{
		LevelManager.OnStopLevel += LevelStopLevel;
		Player.OnPlayerSpawned += ListenToJoinedPlayer;
		
		// Hide menu graphic initially
		if (graphic != null)
			graphic.SetActive(false);
	}

	private void ListenToJoinedPlayer(Player newPlayer)
	{
		if(playersBeingListenedTo.Contains(newPlayer)) return;
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

	private void LevelStopLevel(GameLevel gameLevel)
	{
		StopListeningToJoinedPlayers();
		LevelManager.OnStopLevel -= LevelStopLevel;
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
				
				LevelManager.I.RestartLevel(); // Fallback
				break;
			case MenuButton.ButtonType.Resume:
				Unpause();
				break;
			case MenuButton.ButtonType.MainMenu:
				Unpause();
				LevelManager.I.ExitToMainMenu();
				break;
			case MenuButton.ButtonType.Quit:
				LevelManager.I.QuitGame();
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
		if (PauseManager.IsPaused)
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
		if (PauseManager.IsPaused) return;
		Debug.Log("Pause");
		graphic.SetActive(true);
		OnPause?.Invoke(player);
		PauseManager.IsPaused = true;
		pausingPlayer = player;
		Players.SetActionMaps(Players.UIActionMap);
		
		Time.timeScale = 0;
		menuButtons.InitButtons();
	}

	/// <summary>
	/// Unpause the game
	/// </summary>
	public void Unpause()
	{
		if (!PauseManager.IsPaused) return;
		Debug.Log("unPause ");
		graphic.SetActive(false);
		OnUnpause?.Invoke(pausingPlayer);
		PauseManager.IsPaused = false;
		Players.SetActionMaps(Players.PlayerActionMap);
		
		Time.timeScale = 1;
		menuButtons.UnhighlightButtons();
		pausingPlayer = null;
	}
}