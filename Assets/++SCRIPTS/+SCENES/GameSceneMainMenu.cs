/// <summary>
/// Handles the main menu scene functionality
/// </summary>
public class GameSceneMainMenu : GameScene
{
	private void OnEnable()
	{
		// Reset player state
		Players.ClearAllJoinedPlayers();
		Players.OnPlayerJoins += PlayerOnJoins;
		Players.SetActionMaps(Players.UIActionMap);
	}

	private void OnDisable()
	{
		// Cleanup event subscription
		Players.OnPlayerJoins -= PlayerOnJoins;
	}

	private void PlayerOnJoins(Player player)
	{
		// Player has joined, go to character selection
		Players.OnPlayerJoins -= PlayerOnJoins;
		SFX.sounds.press_start_sounds.PlayRandom();
		SceneLoader.I.GoToScene(ASSETS.Scenes.characterSelect);
	}
}