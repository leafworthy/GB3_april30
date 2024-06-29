using System;
using UnityEngine;

public class SceneMainMenu : Scene
{
	private void Start()
	{
		Players.ClearAllPlayers();
		Players.OnPlayerJoins += PlayerOnJoins;
		Players.SetActionMaps(Players.UIActionMap);
	}

	private void PlayerOnJoins(Player player)
	{
		Debug.Log("Player " + player.playerIndex + " has Joined");
		Players.OnPlayerJoins -= PlayerOnJoins;
		GoToScene(Type.CharacterSelect);
	}

}
