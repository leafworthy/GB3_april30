using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameSceneCharacterSelection : GameScene
	{
		[SerializeField] private List<CharacterButton> Buttons = new();
		[SerializeField] private HideRevealObjects titlePressStart;

		private bool playersAllChosen;
		private bool isListening;
		private List<Player> playersBeingListenedTo = new();
		public event Action OnSelectCharacter;
		public event Action OnDeselectCharacter;
		public event Action OnPlayerMoveLeft;
		public event Action OnPlayerMoveRight;
		public event Action OnPlayerUnjoins;
		public event Action OnPlayerStartsSelecting;
		public event Action OnTryToStartGame;

		protected void OnEnable()
		{
			CleanUp();
			Players.I.OnPlayerJoins += PlayerStartsSelecting;
			foreach (var player in Players.I.AllJoinedPlayers) PlayerStartsSelectingFromMainMenu(player);

			foreach (var button in Buttons) button.SetPlayerColors();


			isActive = true;
			//Debug.Log("Character Selection Start");
		}

		private void PlayerStartsSelectingFromMainMenu(Player player)
		{
			if (playersBeingListenedTo.Contains(player)) return;
			//Debug.Log(player.PlayerName + player.playerIndex + "has joined character selection");

			player.SetState(Player.State.SelectingCharacter);
			player.CurrentButton = Buttons[0];
			player.CurrentButton.HighlightButton(player);
			//OnPlayerStartsSelecting?.Invoke();
			ListenToPlayer(player);
			HideGoGoGo();
		}

		private void OnDisable()
		{
			Players.I.OnPlayerJoins -= PlayerStartsSelecting;
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
			//Debug.Log(player.PlayerName + player.playerIndex + "has joined character selection");

			player.SetState(Player.State.SelectingCharacter);
			player.CurrentButton = Buttons[0];
			player.CurrentButton.HighlightButton(player);
			OnPlayerStartsSelecting?.Invoke();
			ListenToPlayer(player);
			HideGoGoGo();
		}

		private void PlayerUnjoins(Player player)
		{
			if (!playersBeingListenedTo.Contains(player)) return;

			//Debug.Log(player.PlayerName + " HAS UNJOINED");
			player.SetState(Player.State.Unjoined);
			player.CurrentButton.UnHighlightButton(player);
			player.CurrentButton = null;
			OnPlayerUnjoins?.Invoke();
			StopListeningToPlayer(player);
			player.Controller.Select.OnPress += OnUnjoinedPlayerPressSelect;
			Players.I.AllJoinedPlayers.Remove(player);
			//player.gameObject.SetActive(false);
		}

		private void OnUnjoinedPlayerPressSelect(NewControlButton obj)
		{
			var player = obj.owner;
			player.Controller.Select.OnPress -= OnUnjoinedPlayerPressSelect;
			
			// Add player back to AllJoinedPlayers when rejoining
			if (!Players.I.AllJoinedPlayers.Contains(player))
			{
				Players.I.AllJoinedPlayers.Add(player);
			}
			
			PlayerStartsSelecting(player);
		}

		private void ListenToPlayer(Player player)
		{
			player.Controller.Select.OnPress += PlayerPressSelect;
			player.Controller.Cancel.OnPress += PlayerPressCancel;
			player.Controller.UIAxis.OnRight += PlayerMoveRight;
			player.Controller.UIAxis.OnLeft += PlayerMoveLeft;
			playersBeingListenedTo.Add(player);
		}

		private void StopListeningToPlayer(Player player)
		{
			player.Controller.Select.OnPress -= PlayerPressSelect;
			player.Controller.Cancel.OnPress -= PlayerPressCancel;
			player.Controller.UIAxis.OnRight -= PlayerMoveRight;
			player.Controller.UIAxis.OnLeft -= PlayerMoveLeft;
			playersBeingListenedTo.Remove(player);
		}

		private void StopListeningToPlayers()
		{
			var tempList = playersBeingListenedTo.ToList();
			foreach (var player in tempList) StopListeningToPlayer(player);

			playersBeingListenedTo.Clear();
		}

		private void PlayerMoveLeft(IControlAxis controlAxis)
		{
			MoveButton(controlAxis, false);
		}

		private void PlayerMoveRight(IControlAxis controlAxis)
		{
			MoveButton(controlAxis, true);
		}

		private void MoveButton(IControlAxis controlAxis, bool toRight)
		{
			if (!isActive) return;

			var player = controlAxis.owner;
			if (player.state != Player.State.SelectingCharacter) return;

			if (toRight)
			{
				OnPlayerMoveRight?.Invoke();
				player.CurrentButton.UnHighlightButton(player);
				if (player.buttonIndex == Buttons.Count - 1)
					player.buttonIndex = 0;
				else
					player.buttonIndex++;
			}
			else
			{
				OnPlayerMoveLeft?.Invoke();
				player.CurrentButton.UnHighlightButton(player);
				if (player.buttonIndex == 0)
					player.buttonIndex = Buttons.Count - 1;
				else
					player.buttonIndex--;
			}

			player.CurrentButton = Buttons[player.buttonIndex];
			player.CurrentButton.HighlightButton(player);
		}

		private void PlayerPressSelect(NewControlButton newControlButton)
		{
			if (!isActive) return;

			var player = newControlButton.owner;

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
			CheckIfPlayersAllSelected();
			if (!playersAllChosen) return;
			OnTryToStartGame?.Invoke();
			titlePressStart.gameObject.SetActive(false);
			StopListeningToPlayers();
			ClearAllPlayerButtons();
			isActive = false;
			Players.I.OnPlayerJoins -= PlayerStartsSelecting;
			LevelManager.I.StartGame();
		}

		private void ClearAllPlayerButtons()
		{
			foreach (var player in Players.I.AllJoinedPlayers) player.CurrentButton = null;
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
			player.CurrentButton.SelectCharacter(player);
			player.CurrentCharacter = player.CurrentButton.character;
			player.SetState(Player.State.Selected);
			CheckIfPlayersAllSelected();
		}

		private void CheckIfPlayersAllSelected()
		{
			var playersStillSelecting =
				Players.I.AllJoinedPlayers.Where(t => t.state == Player.State.SelectingCharacter).ToList();
			if (playersStillSelecting.Count > 0)
			{
				HideGoGoGo();
				return;
			}

			ShowGoGoGo();
		}

		private void ShowGoGoGo()
		{
			titlePressStart.Set(1);
			playersAllChosen = true;
			Debug.Log("show");
		}

		private void HideGoGoGo()
		{
			playersAllChosen = false;
			titlePressStart.Set(0);
			Debug.Log("hide");
		}
	}
}
