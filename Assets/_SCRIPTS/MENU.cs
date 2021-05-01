using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
	public class MENU:Singleton<MENU>  {


		[SerializeField] private static List<Player> Players = new List<Player>();

		private static bool isRunning;

		private void Update()
		{
			if (!isRunning) return;
			HandlePlayerInputs();
		}

		public static void StartMainMenu()
		{
			isRunning = true;
			Players = GAME.GetPlayers();
			I.gameObject.SetActive(true);
		}
		private void HandlePlayerInputs()
		{
			foreach (var player in GAME.GetPlayers())
			{

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


			}
		}
		private void PlayerPressA(Player player)
		{

			JoinPlayer(player);

		}

		private void JoinPlayer(Player player)
		{
			Debug.Log("Player has joined" + player.playerIndex);
			CHARS.I.StartCharacterSelectionScreen(player);
			gameObject.SetActive(false);
			isRunning = false;
		}
	}
}
