using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneEndGameScene : GameScene
{
	public HorizontalLayoutGroup horizontalLayoutGroup;
	private MenuButtons menuButtons;
	public event Action OnPlayerPressedSelect;
	public event Action OnPlayerPressedUp;
	public event Action OnPlayerPressedDown;

	protected void Start()
	{
		DestroyOldChildren();
		menuButtons = GetComponent<MenuButtons>();
		foreach (var p in Players.AllJoinedPlayers)
		{
			var newDisplay = ObjectMaker.Make(ASSETS.ui.PlayerStatsDisplayPrefab);
			newDisplay.transform.SetParent(horizontalLayoutGroup.transform);
			var statsDisplay = newDisplay.GetComponent<PlayerStatsDisplay>();
			statsDisplay.SetPlayer(p);
		}

		ListenToJoinedPlayers();
	}

	private void DestroyOldChildren()
	{
		var children = new List<GameObject>();
		foreach (Transform child in horizontalLayoutGroup.transform)
		{
			children.Add(child.gameObject);
		}

		children.ForEach(child =>
		{
			ObjectMaker.Unmake(child);
			child.transform.SetParent(null);
		});
	}

	private void ListenToJoinedPlayers()
	{
		foreach (var joinedPlayer in Players.AllJoinedPlayers)
		{
			joinedPlayer.Controller.Select.OnPress += PlayerPressedSelect;
			joinedPlayer.Controller.UIAxis.OnUp += PlayerPressedUp;
			joinedPlayer.Controller.UIAxis.OnDown += PlayerPressedDown;
		}
	}

	private void StopListeningToJoinedPlayers()
	{
		foreach (var joinedPlayer in Players.AllJoinedPlayers)
		{
			joinedPlayer.Controller.Select.OnPress -= PlayerPressedSelect;
			joinedPlayer.Controller.UIAxis.OnUp -= PlayerPressedUp;
			joinedPlayer.Controller.UIAxis.OnDown -= PlayerPressedDown;
		}
	}

	private void PlayerPressedUp(IControlAxis controlAxis)
	{
		menuButtons.Up();
		OnPlayerPressedUp?.Invoke();
	}

	private void PlayerPressedDown(IControlAxis controlAxis)
	{
		menuButtons.Down();
		OnPlayerPressedDown?.Invoke();
	}

	private void PlayerPressedSelect(NewControlButton newControlButton)
	{
		OnPlayerPressedSelect?.Invoke();

		switch (menuButtons.GetCurrentButton().type)
		{
			case MenuButton.ButtonType.Restart:
				StopListeningToJoinedPlayers();
				LevelGameScene.CurrentLevelGameScene.RestartLevel();
				break;
			case MenuButton.ButtonType.MainMenu:
				StopListeningToJoinedPlayers();
				LevelGameScene.CurrentLevelGameScene.ExitToMainMenu();
				break;
			case MenuButton.ButtonType.Quit:
				GlobalManager.QuitGame();
				break;
		}

		menuButtons.UnhighlightButtons();
	}
}