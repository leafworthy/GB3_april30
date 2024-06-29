using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneEndScreen : Scene
{
	public HorizontalLayoutGroup horizontalLayoutGroup;
	private MenuButtons menuButtons;

	protected void Start()
	{
		DestroyOldChildren();
		menuButtons = GetComponent<MenuButtons>();
		foreach (var p in Players.AllJoinedPlayers)
		{
			Debug.Log(p.playerIndex + "set in endscreen");
			var newDisplay = Maker.Make(ASSETS.ui.PlayerStatsDisplayPrefab);
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
			Maker.Unmake(child);
			child.transform.SetParent(null);
		});
	}

	private void ListenToJoinedPlayers()
	{
		foreach (var joinedPlayer in Players.AllJoinedPlayers)
		{
			Debug.Log("endscreen listening to player" + joinedPlayer.playerIndex);
			joinedPlayer.Controller.Select.OnPress += PlayerPressedSelect;
			joinedPlayer.Controller.UIAxis.OnUp += PlayerPressedUp;
			joinedPlayer.Controller.UIAxis.OnDown += PlayerPressedDown;
		}
	}

	private void StopListeningToJoinedPlayers()
	{
		foreach (var joinedPlayer in Players.AllJoinedPlayers)
		{
			Debug.Log("endscreen stopped listening to player" + joinedPlayer.playerIndex);
			joinedPlayer.Controller.Select.OnPress -= PlayerPressedSelect;
			joinedPlayer.Controller.UIAxis.OnUp -= PlayerPressedUp;
			joinedPlayer.Controller.UIAxis.OnDown -= PlayerPressedDown;
		}
	}

	private void PlayerPressedUp(IControlAxis controlAxis)
	{
		menuButtons.Up();
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void PlayerPressedDown(IControlAxis controlAxis)
	{
		menuButtons.Down();
		ASSETS.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void PlayerPressedSelect(NewControlButton newControlButton)
	{
		ASSETS.sounds.pauseMenu_select_sounds.PlayRandom();

		switch (menuButtons.GetCurrentButton().type)
		{
			case MenuButton.ButtonType.Restart:
				StopListeningToJoinedPlayers();
				Level.CurrentLevel.RestartLevel();
				break;
			case MenuButton.ButtonType.MainMenu:
				StopListeningToJoinedPlayers();
				Level.CurrentLevel.ExitToMainMenu();
				break;
			case MenuButton.ButtonType.Quit:
				Game_GlobalVariables.QuitGame();
				break;
		}

		menuButtons.UnhighlightButtons();
	}
}