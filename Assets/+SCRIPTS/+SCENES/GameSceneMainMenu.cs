using UnityEngine;

namespace __SCRIPTS
{
	/// <summary>
	/// Handles the main menu scene functionality
	/// </summary>
	public class GameSceneMainMenu : GameScene
	{
		private Players players;
		private void OnEnable()
		{
			players = ServiceLocator.Get<Players>();
			players.ClearAllJoinedPlayers();
			players.OnPlayerJoins += PlayerOnJoins;
			players.SetActionMaps(Players.UIActionMap);
		}

		private void OnDisable()
		{
			players.OnPlayerJoins -= PlayerOnJoins;
		}

		private void PlayerOnJoins(Player player)
		{
			Debug.Log("GAME SCENE MAIN MENU: Player " + player.name + " has joined the main menu scene.");
			Services.playerManager.OnPlayerJoins -= PlayerOnJoins;
			Services.sfx.sounds.press_start_sounds.PlayRandom();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.characterSelect);
		}
	}
}
