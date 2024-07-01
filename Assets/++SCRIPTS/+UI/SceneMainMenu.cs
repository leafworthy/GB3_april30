using __SCRIPTS._PLAYER;
using UnityEngine;

namespace __SCRIPTS._UI
{
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
}
