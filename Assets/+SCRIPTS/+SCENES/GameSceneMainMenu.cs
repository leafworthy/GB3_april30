using UnityEngine;

namespace __SCRIPTS
{
	/// <summary>
	/// Handles the main menu scene functionality
	/// </summary>
	public class GameSceneMainMenu : GameScene
	{
		private void OnEnable()
		{
			Services.playerManager.ClearAllJoinedPlayers();
			Services.playerManager.OnPlayerJoins += PlayerOnJoins;
			Services.playerManager.SetActionMaps(Players.UIActionMap);
			Services.hudManager.ResetHUD();
		}

		private void OnDisable()
		{
			Services.playerManager.OnPlayerJoins -= PlayerOnJoins;
		}

		private void PlayerOnJoins(Player player)
		{
			Services.playerManager.OnPlayerJoins -= PlayerOnJoins;
			Services.sfx.sounds.press_start_sounds.PlayRandom();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.characterSelect);
		}
	}
}
