using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _SCRIPTS
{
	public class CHARS : Singleton<CHARS>
	{
		[SerializeField] private List<CharacterButton> Buttons = new List<CharacterButton>();
		[SerializeField] private List<Player> Players = new List<Player>();
		[SerializeField] private GameObject doneIndicator;
		[SerializeField] private bool playersAllSelected;
		public GameObject GraphicObject;
		public static event Action<List<Player>> OnCharacterSelectionComplete;
		public bool isRunning;

		private void CleanUp()
		{
			isRunning = false;
			playersAllSelected = false;
			foreach (var player in Players)
			{
				player.CleanUp();
			}

			foreach (var button in Buttons)
			{
				button.CleanUp();
			}
		}

		private void SetPlayerColors()
		{
			foreach (var button in Buttons)
			{
				button.SetPlayerColors(Players);
			}
		}
		public void StartCharacterSelectionScreen(Player firstPlayer)
		{
			GraphicObject.SetActive(true);
			Players.Clear();
			Players = GAME.GetPlayers();
			SetPlayerColors();
			CleanUp();
			HideGoGoGo();
			isRunning = true;
			JoinPlayer(firstPlayer);
			firstPlayer.A_Pressed = true;
		}

		private void Update()
		{
			if (!isRunning) return;
			HandlePlayerInputs();
		}

		private void HandlePlayerInputs()
		{
			foreach (var player in Players)
			{
				if (GamePad.GetButton(CButton.B, player.playerIndex))
				{
					if (!player.B_Pressed)
					{
						player.B_Pressed = true;
						PlayerPressB(player);
					}
				}
				else
				{
					player.B_Pressed = false;
				}

				if (GamePad.GetButton(CButton.A, player.playerIndex))
				{
					if (!player.A_Pressed)
					{
						player.A_Pressed = true;
						PlayerPressA(player);
					}
				}
				else
				{
					player.A_Pressed = false;
				}

				if (player.hasSelectedCharacter) continue;
				var dpad = GamePad.GetAxis(CAxis.LX, player.playerIndex);
				if (dpad > .5)
				{
					if (!player.Right_Pressed)
					{
						PlayerMoveRight(player);
						player.Right_Pressed = true;
					}
				}
				else
					player.Right_Pressed = false;

				if (dpad < -.5)
				{
					if (!player.Left_Pressed)
					{
						PlayerMoveLeft(player);
						player.Left_Pressed = true;
					}
				}
				else
					player.Left_Pressed = false;
			}
		}

		private void PlayerMoveLeft(Player player)
		{
			Debug.Log("Player" + player.playerIndex + " has moved left");
			if (!player.hasJoined) return;
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
				{
					SelectCharacter(player);
				}
				else if(playersAllSelected)
				{
					CharacterSelectionComplete();
				}
			}
			else
			{
				JoinPlayer(player);
			}
		}



		private void PlayerPressB(Player player)
		{
			Debug.Log("Player" + player.playerIndex + " has pressed B");

			if (player.hasJoined && player.hasSelectedCharacter) DeselectCharacter(player);
		}

		private void DeselectCharacter(Player player)
		{
			Debug.Log("Player has deselected" + player.playerIndex);
			player.currentButton.DeselectCharacter(player);
			CheckIfPlayersAllSelected();
		}

		public void JoinPlayer(Player player)
		{
			Debug.Log("Player has joined" + player.playerIndex);
			player.Join(Buttons[0]);
		}

		private void SelectCharacter(Player player)
		{
			player.currentButton.SelectCharacter(player);
			Debug.Log("Player" + player.playerIndex + " has selected" + player.currentButton.character);
			CheckIfPlayersAllSelected();
		}

		private void CheckIfPlayersAllSelected()
		{
			foreach (var p in Players)
				if (!p.hasSelectedCharacter && p.hasJoined)
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

		private void CharacterSelectionComplete()
		{
			var joinedPlayers = Players.Where(t => t.hasJoined).ToList();
			foreach (var player in joinedPlayers)
			{
				Debug.Log("Player "+player.playerIndex + " has joined the game as " +player.currentCharacter.ToString());
			}
			OnCharacterSelectionComplete?.Invoke(joinedPlayers);
			isRunning = false;
			GraphicObject.SetActive(false);
		}
	}
}
