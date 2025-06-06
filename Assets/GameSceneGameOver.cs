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
	    Players.SetActionMaps(Players.UIActionMap);
	    foreach (var display in displays)
	    {
		    display.gameObject.SetActive(false);
	    }
	    for (int i = 0; i < Players.I.AllJoinedPlayers.Count; i++)
	    {

			    Debug.Log("stats for player " + Players.I.AllJoinedPlayers[i].playerIndex);
			    displays[i].gameObject.SetActive(true);
			    displays[i].SetPlayer(Players.I.AllJoinedPlayers[i]);
			    Players.I.AllJoinedPlayers[i].Controller.Select.OnPress += ContinuePress;


	    }

    }

    private void ContinuePress(NewControlButton obj)
    {
	    foreach (var player in Players.I.AllJoinedPlayers)
	    {
			player.Controller.Select.OnPress -= ContinuePress;
	    }

	    Destroy(GameManager.I.gameObject);
	    UnityEngine.Debug.Log("Game Manager Destroyed");

	    SceneManager.LoadScene(gameManagerScene.sceneName, LoadSceneMode.Additive);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
