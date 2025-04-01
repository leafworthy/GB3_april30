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
	    for (int i = 0; i < Players.AllJoinedPlayers.Count; i++)
	    {
		    
			    Debug.Log("stats for player " + Players.AllJoinedPlayers[i].playerIndex);
			    displays[i].gameObject.SetActive(true);
			    displays[i].SetPlayer(Players.AllJoinedPlayers[i]);
			    Players.AllJoinedPlayers[i].Controller.Select.OnPress += ContinuePress;
		    
		    
	    }

	    GameManager.I.DisableGameCamera();
    }

    private void ContinuePress(NewControlButton obj)
    {
	    foreach (var player in Players.AllJoinedPlayers)
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
