using UnityEngine;

public class GameSceneMainMenu : GameScene
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