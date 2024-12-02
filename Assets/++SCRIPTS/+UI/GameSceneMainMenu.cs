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
		SFX.sounds.charSelect_select_sounds.PlayRandom();
		GoToScene(Type.CharacterSelect);
	}

}