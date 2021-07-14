using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSelectionMenu : Singleton<CharacterSelectionMenu>
{
	[SerializeField] private List<CharacterButton> Buttons = new List<CharacterButton>();
	[SerializeField] private List<Player> Players = new List<Player>();
	[SerializeField] private GameObject doneIndicator;
	[SerializeField] private bool playersAllSelected;
	public GameObject GraphicObject;
	public static event Action<List<Player>> OnCharacterSelectionComplete;
	public bool isRunning;

	private void ResetPlayersAndButtons()
	{
		isRunning = false;
		playersAllSelected = false;

		foreach (var button in Buttons) button.CleanUp();
	}

	private void SetPlayerColors()
	{
		foreach (var button in Buttons) button.SetPlayerColors(Players);
	}

	public void StartCharacterSelectionScreen(Player firstPlayer)
	{
		if (GAME.I.isTesting) return;
		GraphicObject.SetActive(true);
		Players.Clear();
		Players = GAME.GetPlayers();
		ListenToPlayers();

		SetPlayerColors();
		ResetPlayersAndButtons();
		HideGoGoGo();
		isRunning = true;
		JoinPlayer(firstPlayer);
	}

	private void StopCharacterSelectionScreen()
	{
		if(!isRunning) return;

		var joinedPlayers = Players.Where(t => t.hasJoined).ToList();
		foreach (var player in joinedPlayers)
			Debug.Log("Player " + player.playerIndex + " has joined the game as " +
			          player.currentCharacter.ToString());
		ASSETS.sounds.pickup_speed_sounds.PlayRandom();
		StopListeningToPlayers();
		OnCharacterSelectionComplete?.Invoke(joinedPlayers);
		isRunning = false;
		GraphicObject.SetActive(false);
	}

	private void ListenToPlayers()
	{
		foreach (var player in Players)
		{
			player.PressPause += PlayerPressB;
			player.PressA += PlayerPressA;
			player.PressB += PlayerPressB;
			player.MoveRight += PlayerMoveRight;
			player.MoveLeft += PlayerMoveLeft;
			player.MoveUp += PlayerMoveUp;
			player.MoveDown += PlayerMoveDown;
		}
	}

	private void StopListeningToPlayers()
	{
		foreach (var player in Players)
		{
			player.PressPause -= PlayerPressB;
			player.PressA -= PlayerPressA;
			player.PressB -= PlayerPressB;
			player.MoveRight -= PlayerMoveRight;
			player.MoveLeft -= PlayerMoveLeft;
			player.MoveUp -= PlayerMoveUp;
			player.MoveDown -= PlayerMoveDown;
		}
	}

	private void PlayerMoveDown(Player obj)
	{
		//
	}

	private void PlayerMoveUp(Player obj)
	{
		//
	}

	private void PlayerMoveLeft(Player player)
	{
		if (!player.hasJoined) return;
		Debug.Log("player move left");
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

		Debug.Log("player move right");
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
			if (!player.hasSelectedCharacter)
				SelectCharacter(player);
			else if (playersAllSelected) StopCharacterSelectionScreen();
		}
		else
			JoinPlayer(player);
	}


	private void PlayerPressB(Player player)
	{
		Debug.Log("Player" + player.playerIndex + " has pressed B");

		if (player.hasJoined && player.hasSelectedCharacter) DeselectCharacter(player);
	}

	private void DeselectCharacter(Player player)
	{
		ASSETS.sounds.charSelect_deselect_sounds.PlayRandom();
		Debug.Log("Player has deselected" + player.playerIndex);
		player.currentButton.DeselectCharacter(player);
		CheckIfPlayersAllSelected();
	}

	private void JoinPlayer(Player player)
	{
		if (player.hasJoined) return;
		Debug.Log("Player has joined" + player.playerIndex);
		player.Join(Buttons[0]);
		ASSETS.sounds.press_start_sounds.PlayRandom();
	}

	private void SelectCharacter(Player player)
	{
		player.currentButton.SelectCharacter(player);
		ASSETS.sounds.charSelect_select_sounds.PlayRandom();
		AUDIO.PlaySound(ASSETS.sounds.charSelect_select_sounds.GetRandom());
		Debug.Log("Player" + player.playerIndex + " has selected" + player.currentButton.character);
		CheckIfPlayersAllSelected();
	}

	private void CheckIfPlayersAllSelected()
	{
		if (Players.Any(p => !p.hasSelectedCharacter && p.hasJoined))
		{
			HideGoGoGo();
			return;
		}

		ShowGoGoGo();


		Debug.Log("done selecting");
	}

	private void ShowGoGoGo()
	{
		doneIndicator.SetActive(true);
		playersAllSelected = true;
	}

	private void HideGoGoGo()
	{
		playersAllSelected = false;
		doneIndicator.SetActive(false);
	}
}
