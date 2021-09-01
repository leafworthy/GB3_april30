using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Menu_CharacterSelection : Menu
{
	[SerializeField] private List<CharacterButton> Buttons = new List<CharacterButton>();
	[SerializeField] private GameObject GoGoGoIndicator;
	[SerializeField] private bool playersAllChosen;

	public override void StartMenu()
	{
		base.StartMenu();
		CleanUpButtons();
		HideGoGoGo();
		playersAllChosen = false;
		JoinJoinedPlayers();
		ListenToPlayers();
		SetPlayerColors();
	}

	private void JoinJoinedPlayers()
	{
		foreach (var player in PLAYERS.GetJoiningPlayers())
		{
			Debug.Log("PLAYERFOUND");
			PlayerJoins(player);
		}
	}

	private void CleanUpButtons()
	{
		foreach (var button in Buttons) button.CleanUp();
	}

	private void SetPlayerColors()
	{
		foreach (var button in Buttons) button.SetPlayerColors();
	}

	public override void StopMenu(MENUS.Type menuType)
	{

		var joinedPlayers = PLAYERS.GetJoinedPlayers();
		foreach (var player in joinedPlayers)
			Debug.Log("Player " + player.data.playerIndex + " has joined the game as " +
			          player.currentCharacter.ToString());
		ASSETS.sounds.pickup_speed_sounds.PlayRandom();
		StopListeningToPlayers();
		LEVELS.PlayLevel(joinedPlayers);
		base.StopMenu(menuType);
	}



	private void ListenToPlayers()
	{
		foreach (var player in PLAYERS.GetAllPlayers())
		{
			player.PressPause += PlayerPressB;
			player.PressA += PlayerPressA;
			player.PressB += PlayerPressB;
			player.MoveRight += PlayerMoveRight;
			player.MoveLeft += PlayerMoveLeft;
		}
	}

	private void StopListeningToPlayers()
	{
		foreach (var player in PLAYERS.GetAllPlayers())
		{
			player.PressPause -= PlayerPressB;
			player.PressA -= PlayerPressA;
			player.PressB -= PlayerPressB;
			player.MoveRight -= PlayerMoveRight;
			player.MoveLeft -= PlayerMoveLeft;
		}
	}

	private void PlayerMoveLeft(Player player)
	{
		if (!player.hasJoined) return;
		ASSETS.sounds.charSelect_move_sounds.PlayRandom();

		player.currentButton.UnHighlightButton(player);
		if (player.buttonIndex == 0)
		{
			player.buttonIndex = Buttons.Count - 1;
			Buttons[player.buttonIndex].HighlightButton(player);
		}
		else
		{
			player.buttonIndex--;
			Buttons[player.buttonIndex].HighlightButton(player);
		}
	}

	private void PlayerMoveRight(Player player)
	{
		if (!player.hasJoined) return;
		ASSETS.sounds.charSelect_move_sounds.PlayRandom();

		player.currentButton.UnHighlightButton(player);
		if (player.buttonIndex == Buttons.Count - 1)
		{
			player.buttonIndex = 0;
			Buttons[player.buttonIndex].HighlightButton(player);
		}
		else
		{
			player.buttonIndex++;
			Buttons[player.buttonIndex].HighlightButton(player);
		}
	}

	private void PlayerPressA(Player player)
	{
		if (player.hasJoined)
		{
			if (!player.hasChosenCharacter)
				PlayerSelectCharacter(player);
			else if (playersAllChosen) MENUS.ChangeMenu(MENUS.Type.InGame);
		}
		else
			PlayerJoins(player);
	}

	private void PlayerPressB(Player player)
	{
		if (player.hasJoined && player.hasChosenCharacter) DeselectCharacter(player);
	}

	private void DeselectCharacter(Player player)
	{
		ASSETS.sounds.charSelect_deselect_sounds.PlayRandom();
		player.currentButton.DeselectCharacter(player);
		CheckIfPlayersAllSelected();
	}

	private void PlayerJoins(Player player)
	{
		player.JoinSelection(Buttons[0]);
		ASSETS.sounds.press_start_sounds.PlayRandom();
	}

	private void PlayerSelectCharacter(Player player)
	{
		player.currentButton.SelectCharacter(player);
		ASSETS.sounds.charSelect_select_sounds.PlayRandom();
		CheckIfPlayersAllSelected();
	}

	private void CheckIfPlayersAllSelected()
	{
		if (PLAYERS.GetAllPlayers().Any(p => !p.hasChosenCharacter && p.hasJoined))
		{
			HideGoGoGo();
			return;
		}

		ShowGoGoGo();
	}

	private void ShowGoGoGo()
	{
		GoGoGoIndicator.SetActive(true);
		playersAllChosen = true;
	}

	private void HideGoGoGo()
	{

		playersAllChosen = false;
		GoGoGoIndicator.SetActive(false);
	}
}
