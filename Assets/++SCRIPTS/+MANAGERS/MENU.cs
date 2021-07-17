using System.Collections.Generic;
using UnityEngine;

public class MENU:Singleton<MENU>  {


	[SerializeField] private static List<Player> Players = new List<Player>();

	private static bool isRunning;

	private void Update()
	{
		if (!isRunning) return;
	}

	public static void StartMainMenu()
	{
		if (GAME.I.isTesting) return;
		isRunning = true;
		Players = GAME.GetPlayers();
		I.gameObject.SetActive(true);
		foreach (var player in GAME.GetPlayers())
		{
			player.PressA += Player_OnJoin;
		}
	}

	private static void Player_OnJoin(Player player)
	{
		if(player.hasJoined) return;
		Debug.Log("Player has joined" + player.playerIndex);
		CharacterSelectionMenu.I.StartCharacterSelectionScreen(player);
		foreach (var p in GAME.GetPlayers())
		{
			p.PressA -= Player_OnJoin;
		}
		I.gameObject.SetActive(false);
		isRunning = false;
	}
}
