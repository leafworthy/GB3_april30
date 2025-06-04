namespace GangstaBean.Scenes
{
	/// <summary>
	/// Handles the main menu scene functionality
	/// </summary>
	public class GameSceneMainMenu : GameScene
	{
		private void Start()
		{
			// Reset player state
			Players.ClearAllJoinedPlayers();
			Players.I.OnPlayerJoins += PlayerOnJoins;
			Players.SetActionMaps(Players.UIActionMap);
		}


		private void PlayerOnJoins(Player player)
		{
			// Player has joined, go to character selection
			Players.I.OnPlayerJoins -= PlayerOnJoins;
			SFX.I.sounds.press_start_sounds.PlayRandom();
			SceneLoader.I.GoToScene(ASSETS.Scenes.characterSelect);
		}
	}
}