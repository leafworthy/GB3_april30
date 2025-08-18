using System.Collections.Generic;
using __SCRIPTS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneGameOver : GameScene
{
	public SceneDefinition gameManagerScene;
    public List<PlayerStatsDisplay> displays;
    void Start()
    {
	    playerManager.SetActionMaps(Players.UIActionMap);
	    foreach (var display in displays)
	    {
		    display.gameObject.SetActive(false);
	    }
	    for (int i = 0; i < playerManager.AllJoinedPlayers.Count; i++)
	    {

			    Debug.Log("stats for player " + playerManager.AllJoinedPlayers[i].playerIndex);
			    displays[i].gameObject.SetActive(true);
			    displays[i].SetPlayer(playerManager.AllJoinedPlayers[i]);
			    playerManager.AllJoinedPlayers[i].Controller.OnSelect_Pressed += ContinuePress;


	    }

    }

    private void ContinuePress(NewControlButton obj)
    {
	    foreach (var player in playerManager.AllJoinedPlayers)
	    {
			player.Controller.OnSelect_Pressed -= ContinuePress;
	    }

	    // Use the original level manager system
	    levelManager.ExitToMainMenu();
    }

}
