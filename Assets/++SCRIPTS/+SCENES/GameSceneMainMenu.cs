public class GameSceneMainMenu : GameScene
{
	
	private void Start()
	{
		Players.ClearAllJoinedPlayers();
		Players.OnPlayerJoins += PlayerOnJoins;
		Players.SetActionMaps(Players.UIActionMap);
	}

	private void PlayerOnJoins(Player player)
	{
		//Debug.Log("Player " + player.playerIndex + " has Joined");
		Players.OnPlayerJoins -= PlayerOnJoins;
		SFX.sounds.press_start_sounds.PlayRandom();
		SceneLoader.I.GoToScene(ASSETS.Scenes.characterSelect);
	}

}