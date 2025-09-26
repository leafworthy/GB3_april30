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
			Services.playerManager.OnPlayerJoins += PlayerStartsSelecting;
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				PlayerStartsSelectingWithoutSFX(player);
			}

			foreach (var button in Buttons)
			{
				button.SetPlayerColors();
			}

			isActive = true;
		}

		private void PlayerStartsSelectingWithoutSFX(Player player)
		{
			StartSelecting(player);
		}

		private void OnDisable()
		{
			Services.playerManager.OnPlayerJoins -= PlayerStartsSelecting;
		}

		private void CleanUp()
		{
			foreach (var button in Buttons)
			{
				button.CleanUp();
			}

			HideGoGoGo();
			playersAllChosen = false;
			playersBeingListenedTo.Clear();
		}

		private void PlayerStartsSelecting(Player player)
		{
			if (StartSelecting(player)) return;
			OnPlayerStartsSelecting?.Invoke();
		}

		private bool StartSelecting(Player player)
		{
			if (playersBeingListenedTo.Contains(player)) return true;
			player.SetState(Player.State.SelectingCharacter);
			player.CurrentButton = Buttons[0];
			player.CurrentButton.HighlightButton(player);
			ListenToPlayer(player);
			HideGoGoGo();
			return false;
		}

		private void PlayerUnjoins(Player player)
		{
			if (!playersBeingListenedTo.Contains(player)) return;

			player.SetState(Player.State.Unjoined);
			player.CurrentButton.UnHighlightButton(player);
			player.CurrentButton = null;
			OnPlayerUnjoins?.Invoke();
			StopListeningToPlayer(player);
			player.Controller.Select.OnPress += OnUnjoinedPlayerPressSelect;
			Services.playerManager.AllJoinedPlayers.Remove(player);
		}

		private void OnUnjoinedPlayerPressSelect(NewControlButton obj)
		{
			var player = obj.owner;
			player.Controller.Select.OnPress -= OnUnjoinedPlayerPressSelect;

			if (!Services.playerManager.AllJoinedPlayers.Contains(player)) Services.playerManager.AllJoinedPlayers.Add(player);

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
			foreach (var player in tempList)
			{
				StopListeningToPlayer(player);
			}

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
			OnTryToStartGame?.Invoke(); //SFX
			titlePressStart.gameObject.SetActive(false);
			StopListeningToPlayers();
			ClearAllPlayerButtons();
			isActive = false;
			Services.playerManager.OnPlayerJoins -= PlayerStartsSelecting;
			Services.levelManager.StartGame(Services.levelManager.GetFirstLevelToLoad());
		}

		private void ClearAllPlayerButtons()
		{
			foreach (var player in Services.playerManager.AllJoinedPlayers)
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
			player.CurrentButton.SelectCharacter(player);
			player.CurrentCharacter = player.CurrentButton.character;
			player.SetState(Player.State.Selected);
			CheckIfPlayersAllSelected();
		}

		private void CheckIfPlayersAllSelected()
		{
			var playersStillSelecting = Services.playerManager.AllJoinedPlayers.Where(t => t.state == Player.State.SelectingCharacter).ToList();
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
		}

		private void HideGoGoGo()
		{
			playersAllChosen = false;
			titlePressStart.Set(0);
		}
	}
}
