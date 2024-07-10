using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSceneCharacterSelection : GameScene
{
	[SerializeField] private List<CharacterButton> Buttons = new();
	[SerializeField] private GameObject GoGoGoIndicator;

	private bool playersAllChosen;
	private bool isListening;
	private List<Player> playersBeingListenedTo = new();
	public static event Action OnSelectCharacter;
	public static event Action OnDeselectCharacter;
	public static event Action OnPlayerMoveLeft;
	public static event Action OnPlayerMoveRight;
	public static event Action OnPlayerUnjoins;
	public static event Action OnPlayerStartsSelecting;
	public static event Action OnTryToStartGame;
	protected void Start()
	{
		CleanUp();
		Players.OnPlayerJoins += PlayerStartsSelecting;
		foreach (var player in Players.AllJoinedPlayers)
		{
			Debug.Log("Start seletion");
			PlayerStartsSelecting(player);
		}
		foreach (var button in Buttons) button.SetPlayerColors();
		isActive = true;
		Debug.Log("Character Selection Start");
	}

	private void OnDisable()
	{

		Players.OnPlayerJoins -= PlayerStartsSelecting;
	}

	private void CleanUp()
	{
		foreach (var button in Buttons) button.CleanUp();
		HideGoGoGo();
		playersAllChosen = false;
		playersBeingListenedTo.Clear();
	}

	private void PlayerStartsSelecting(Player player)
	{
		if (playersBeingListenedTo.Contains(player)) return;
		Debug.Log(player.PlayerName + player.playerIndex + "has joined character selection");
		
		player.SetState(Player.State.SelectingCharacter);
		player.CurrentButton = Buttons[0];
		player.CurrentButton.HighlightButton(player);

		OnPlayerStartsSelecting?.Invoke();
		ListenToPlayer(player);
	}

	private void PlayerUnjoins(Player player)
	{
		if (!playersBeingListenedTo.Contains(player)) return;

		Debug.Log(player.PlayerName + " HAS UNJOINED");
		player.SetState(Player.State.Unjoined);
		player.CurrentButton.UnHighlightButton(player);
		player.CurrentButton = null;
		OnPlayerUnjoins?.Invoke();
		StopListeningToPlayer(player);
		player.Controller.Select.OnPress += OnUnjoinedPlayerPressSelect;
		player.gameObject.SetActive(false);
	}

	private void OnUnjoinedPlayerPressSelect(NewControlButton obj)
	{
		var player = obj.owner;
		player.Controller.Select.OnPress -= OnUnjoinedPlayerPressSelect;
		PlayerStartsSelecting(player);
	}

	private void ListenToPlayer(Player player)
	{
		Debug.Log("listening to player " + player.playerIndex);
		player.Controller.Select.OnPress += PlayerPressSelect;
		player.Controller.Cancel.OnPress += PlayerPressCancel;
		player.Controller.UIAxis.OnRight += PlayerMoveRight;
		player.Controller.UIAxis.OnLeft += PlayerMoveLeft;
		playersBeingListenedTo.Add(player);
	}

	private void StopListeningToPlayer(Player player)
	{
		Debug.Log("stop listening to player " + player.playerIndex);
		player.Controller.Select.OnPress -= PlayerPressSelect;
		player.Controller.Cancel.OnPress -= PlayerPressCancel;
		player.Controller.UIAxis.OnRight -= PlayerMoveRight;
		player.Controller.UIAxis.OnLeft -= PlayerMoveLeft;
		playersBeingListenedTo.Remove(player);
	}

	private void StopListeningToPlayers()
	{
		Debug.Log("character selection stop listening to players");
		var tempList = playersBeingListenedTo.ToList();
		foreach (var player in tempList)
		{
			Debug.Log(player.playerIndex + " has stopped being listened to");
			StopListeningToPlayer(player);
		}

		playersBeingListenedTo.Clear();
	}

	private void PlayerMoveLeft(IControlAxis controlAxis)
	{
		if (!isActive)
		{
			Debug.Log("is inactive but trying anyways");
			return;
		}
		var player = controlAxis.owner;
		if (player.state != Player.State.SelectingCharacter) return;

		OnPlayerMoveLeft?.Invoke();
		player.CurrentButton.UnHighlightButton(player);
		if (player.buttonIndex == 0)
			player.buttonIndex = Buttons.Count - 1;
		else
			player.buttonIndex--;

		player.CurrentButton = Buttons[player.buttonIndex];
		player.CurrentButton.HighlightButton(player);
	}

	private void PlayerMoveRight(IControlAxis controlAxis)
	{
		if (!isActive)
		{
			Debug.Log("is inactive but trying anyways");
			return;
		}
		var player = controlAxis.owner;
		if (player.state != Player.State.SelectingCharacter) return;

		
		player.CurrentButton.UnHighlightButton(player);
		if (player.buttonIndex == Buttons.Count - 1)
			player.buttonIndex = 0;
		else
			player.buttonIndex++;

		player.CurrentButton = Buttons[player.buttonIndex];
		player.CurrentButton.HighlightButton(player);
	}

	private void PlayerPressSelect(NewControlButton newControlButton)
	{
		if (!isActive)
		{
			Debug.Log("is inactive but trying anyways");
			return;
		}

		var player = newControlButton.owner;
		Debug.Log(player.PlayerName + " has pressed Select from the state:" + player.state);

		switch (player.state)
		{
			case Player.State.Selected:
				TryToStartGame(player);
				break;
			case Player.State.SelectingCharacter:
				SelectCharacter(player);
				break;
			case Player.State.Unjoined:
				PlayerStartsSelecting(player);
				break;
		}
	}

	private void TryToStartGame(Player player)
	{
		Debug.Log(player.PlayerName + player.playerIndex + " is trying to start the game");
		CheckIfPlayersAllSelected();
		if (!playersAllChosen) return;
		OnTryToStartGame?.Invoke();
		StopListeningToPlayers();
		ClearAllPlayerButtons();
		isActive = false;
		Players.OnPlayerJoins -= PlayerStartsSelecting;
		Debug.Log("Starting game, stopping listening to players");
		GoToScene(Type.InLevel);
	}

	private void ClearAllPlayerButtons()
	{
		foreach (var player in Players.AllJoinedPlayers)
		{
			player.CurrentButton = null;
		}
	}

	private void PlayerPressCancel(NewControlButton newControlButton)
	{
		var player = newControlButton.owner;
		switch (player.state)
		{
			case Player.State.Selected:
				DeselectCharacter(player);
				break;
			case Player.State.SelectingCharacter:
				PlayerUnjoins(player);
				break;
		}
	}

	private void DeselectCharacter(Player player)
	{
		OnDeselectCharacter?.Invoke();
		player.CurrentButton.DeselectCharacter(player);
		player.CurrentCharacter = player.CurrentButton.character;
		player.SetState(Player.State.SelectingCharacter);
		CheckIfPlayersAllSelected();
	}

	private void SelectCharacter(Player player)
	{
		OnSelectCharacter?.Invoke();
		Debug.Log("select character function");
		player.CurrentButton.SelectCharacter(player);
		player.CurrentCharacter = player.CurrentButton.character;
		player.SetState(Player.State.Selected);
		CheckIfPlayersAllSelected();
	}

	private void CheckIfPlayersAllSelected()
	{
		var playersStillSelecting =  playersBeingListenedTo.Where(t => t.state == Player.State.SelectingCharacter).ToList();
		if (playersStillSelecting.Count > 0)
		{
			HideGoGoGo();
			Debug.Log("PLAYERS JOINED BUT HAVEN'T SELECTED");
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