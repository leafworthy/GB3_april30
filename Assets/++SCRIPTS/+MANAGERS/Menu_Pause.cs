using System;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Pause : Menu
{
	[SerializeField] private List<MenuButton> Buttons = new List<MenuButton>();

	private MenuButton currentButton;
	private Player pausingPlayer;

	public static bool isPaused;
	private int currentButtonIndex;
	public static event Action<Player> OnPause;
	public static event Action OnUnpause;

	private void Start()
	{
		LEVELS.OnLevelStart += ListenToPlayersForPausePress;
		LEVELS.OnLevelStop += StopListeningToPlayersForPausePress;
	}

	private void ListenToPlayersForPausePress(List<Player> players)
	{
		foreach (var player in players)
			player.PressPause += PlayerPressedPause;
	}

	private void StopListeningToPlayersForPausePress()
	{
		foreach (var player in PLAYERS.GetJoinedPlayers())
			player.PressPause -= PlayerPressedPause;
	}

	public override void StopMenu(MENUS.Type nextMenu)
	{
		Unpause();
		base.StopMenu(nextMenu);
	}


	private void PlayerPressDown(Player obj)
	{
		Debug.Log("player press down");
		UnHighlightButton(currentButtonIndex);
		IncreaseButtonIndex();
		HighlightButton(currentButtonIndex);
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void IncreaseButtonIndex()
	{
		if (currentButtonIndex >= Buttons.Count - 1)
			currentButtonIndex = 0;
		else
			currentButtonIndex++;
	}


	private void DecreaseButtonIndex()
	{
		if (currentButtonIndex <= 0)
			currentButtonIndex = Buttons.Count - 1;
		else
			currentButtonIndex--;
	}

	private void HighlightButton(int buttonIndex)
	{
		if (Buttons[buttonIndex] is null) return;
		Buttons[buttonIndex].Highlight();
	}

	private void UnHighlightButton(int buttonIndex)
	{
		if (Buttons[buttonIndex] is null) return;
		Buttons[buttonIndex].UnHighlight();
	}

	private void PlayerPressA(Player obj)
	{
		Debug.Log("player press A");
		currentButton = Buttons[currentButtonIndex];
		ASSETS.sounds.pauseMenu_select_sounds.PlayRandom();

		switch (currentButton.type)
		{
			case MenuButton.ButtonType.Restart:
				Debug.Log("restart");
				LEVELS.StopLevelGoToMenu(MENUS.Type.InGame);
				break;
			case MenuButton.ButtonType.Resume:
				Debug.Log("resume");
				Unpause();
				break;
			case MenuButton.ButtonType.MainMenu:
				Debug.Log("main menu");
				LEVELS.StopLevelGoToMenu(MENUS.Type.Main);
				break;
			case MenuButton.ButtonType.Quit:
				Debug.Log("quit");
				QuitGame();
				break;
		}
	}

	private void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
	}

	private void PlayerPressedUp(Player obj)
	{
		Debug.Log("player press up");
		UnHighlightButton(currentButtonIndex);
		DecreaseButtonIndex();
		HighlightButton(currentButtonIndex);
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void PlayerPressedB(Player player)
	{
		Unpause();
	}

	private void PlayerPressedPause(Player player)
	{
		Debug.Log("pause pressed");
		if (!isPaused)
		{
			Pause(player);
			return;
		}

		if (player != pausingPlayer) return;
		Unpause();
	}

	private void Pause(Player player)
	{
		if (isPaused) return;
		isPaused = true;

		OnPause?.Invoke(player);
		MENUS.ChangeMenu(MENUS.Type.Pause);
		Debug.Log("pause by " + player.currentCharacter.ToString());
		pausingPlayer = player;

		ASSETS.sounds.pauseMenu_start_sounds.PlayRandom();

		Time.timeScale = 0;
		InitButtons();
		ListenToPausingPlayer();
	}

	private void Unpause()
	{
		if (!isPaused) return;
		isPaused = false;

		OnUnpause?.Invoke();
		MENUS.ChangeMenu(MENUS.Type.InGame);
		Debug.Log("unpause");
		pausingPlayer = null;

		ASSETS.sounds.pauseMenu_stop_sounds.PlayRandom();

		Time.timeScale = 1;
		CleanUpButtons();
		StopListeningToPausingPlayer();
	}

	private void InitButtons()
	{
		foreach (var button in Buttons) button.UnHighlight();
		currentButtonIndex = 0;
		currentButton = Buttons[currentButtonIndex];
		currentButton.Highlight();
	}

	private void CleanUpButtons()
	{
		currentButton.UnHighlight();
	}

	private void ListenToPausingPlayer()
	{
		pausingPlayer.PressA += PlayerPressA;
		pausingPlayer.PressB += PlayerPressedB;
		pausingPlayer.MoveUp += PlayerPressedUp;
		pausingPlayer.MoveDown += PlayerPressDown;
	}

	private void StopListeningToPausingPlayer()
	{
		pausingPlayer.PressA -= PlayerPressA;
		pausingPlayer.PressB -= PlayerPressedB;
		pausingPlayer.MoveUp -= PlayerPressedUp;
		pausingPlayer.MoveDown -= PlayerPressDown;
	}
}
