namespace __SCRIPTS
{
	/// <summary>
	/// Handles the main menu scene functionality
	/// </summary>
	public class GameSceneMainMenu : GameScene
	{
		private ASSETS assets;
		private Players players;
		private void Start()
		{
			players = ServiceLocator.Get<Players>();
			players.ClearAllJoinedPlayers();
			players.OnPlayerJoins += PlayerOnJoins;
			players.SetActionMaps(Players.UIActionMap);
			assets = ServiceLocator.Get<ASSETS>();
		}


		private void PlayerOnJoins(Player player)
		{
			// Player has joined, go to character selection
			playerManager.OnPlayerJoins -= PlayerOnJoins;
			sfx.sounds.press_start_sounds.PlayRandom();
			sceneLoader.GoToScene(assets.Scenes.characterSelect);
		}
	}
}
