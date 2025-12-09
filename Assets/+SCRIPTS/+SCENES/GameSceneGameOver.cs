using System.Collections.Generic;
using __SCRIPTS;
using UnityEngine;

public class GameSceneGameOver : GameScene
{
	public SceneDefinition gameManagerScene;
    public List<PlayerStatsDisplay> displays;
    void Start()
    {
	    Services.playerManager.SetActionMaps(Players.UIActionMap);
	    foreach (var display in displays)
	    {
		    display.gameObject.SetActive(false);
	    }
	    for (int i = 0; i < Services.playerManager.AllJoinedPlayers.Count; i++)
	    {

			    displays[i].gameObject.SetActive(true);
			    displays[i].SetPlayer(Services.playerManager.AllJoinedPlayers[i]);
			    Services.playerManager.AllJoinedPlayers[i].Controller.Select.OnPress += ContinuePress;


	    }

    }

    void ContinuePress(NewControlButton obj)
    {
	    foreach (var player in Services.playerManager.AllJoinedPlayers)
	    {
			player.Controller.Select.OnPress -= ContinuePress;
	    }

	    // Use the original level manager system
	    Services.levelManager.ExitToMainMenu();
    }

}
