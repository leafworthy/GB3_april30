using System;
using System.Collections.Generic;
using UnityEngine;

public class PAUSE : Singleton<PAUSE>
{
	[SerializeField] private List<MenuButton> Buttons = new List<MenuButton>();
	[SerializeField] private GameObject visibleObject;

	private MenuButton currentButton;
	private Player pausingPlayer;

	public static bool isPaused;
	private int currentButtonIndex;
	public static event Action<Player> OnPause;
	public static event Action OnUnpause;
	private void Start()
	{
		GAME.OnGameStart += GameStart;
		GAME.OnGameEnd += GameEnd;
		visibleObject.SetActive(false);
	}

	private void GameStart()
	{
		foreach (var player in GAME.GetPlayers())
			player.PressPause += Player_OnPausePressed;
	}

	private void GameEnd()
	{
		foreach (var player in GAME.GetPlayers()){
			player.PressPause -= Player_OnPausePressed;
			Unpause();
		}
	}

	private void Player_OnPausePressed(Player player)
	{
		Debug.Log("pause pressed");
		if (!isPaused)
			Pause(player);
		else
		{
			if (player == pausingPlayer)
			{
				Unpause();
			}
		}
	}

	private void PlayerPressDown(Player obj)
	{
		Debug.Log("player press down");
		UnHighlightButton(currentButtonIndex);
		if (currentButtonIndex >= Buttons.Count - 1)
			currentButtonIndex = 0;
		else
			currentButtonIndex++;
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();

		HighlightButton(currentButtonIndex);
	}

	private void PlayerPressUp(Player obj)
	{
		Debug.Log("player press up");
		UnHighlightButton(currentButtonIndex);
		if (currentButtonIndex <= 0)
			currentButtonIndex = Buttons.Count - 1;
		else
			currentButtonIndex--;
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();

		HighlightButton(currentButtonIndex);
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
				GAME.I.EndGameRestart();
				break;
			case MenuButton.ButtonType.Resume:
				Debug.Log("resume");
				Unpause();
				break;
			case MenuButton.ButtonType.MainMenu:
				Debug.Log("main menu");
				GAME.EndGameMainMenu();
				break;
			case MenuButton.ButtonType.Quit:
				Debug.Log("quit");
				QuitGame();
				break;
		}
	}

	private static void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
	}

	private void PlayerPressB(Player player)
	{
		Unpause();
	}


	private void Unpause()
	{
		if (!isPaused) return;
		OnUnpause?.Invoke();
		Debug.Log("unpause");
		isPaused = false;
		currentButton.UnHighlight();
		Time.timeScale = 1;
		I.visibleObject.SetActive(false);
		ASSETS.sounds.pauseMenu_stop_sounds.PlayRandom();

		pausingPlayer.PressA -= PlayerPressA;
		pausingPlayer.PressB -= PlayerPressB;
		pausingPlayer.MoveUp -= PlayerPressUp;
		pausingPlayer.MoveDown -= PlayerPressDown;
		pausingPlayer = null;
	}

	private void Pause(Player player)
	{
		OnPause?.Invoke(player);
		Debug.Log("pause by " + player.currentCharacter.ToString());
		isPaused = true;
		ASSETS.sounds.pauseMenu_start_sounds.PlayRandom();

		Time.timeScale = 0;
		I.visibleObject.SetActive(true);
		I.visibleObject.SetActive(true);
		foreach (var button in Buttons) button.UnHighlight();
		currentButtonIndex = 0;
		currentButton = Buttons[currentButtonIndex];
		currentButton.Highlight();

		pausingPlayer = player;
		pausingPlayer.PressA += PlayerPressA;
		pausingPlayer.PressB += PlayerPressB;
		pausingPlayer.MoveUp += PlayerPressUp;
		pausingPlayer.MoveDown += PlayerPressDown;
	}
}
