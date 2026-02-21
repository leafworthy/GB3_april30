using System.Collections.Generic;
using __SCRIPTS;
using TMPro;
using UnityEngine;

public class GameSceneGameOver : GameScene
{
	public List<PlayerStatsDisplay> displays;

	public GameObject starburst;
	public HideRevealObjects text;

	void Start()
	{
		Services.playerManager.SetActionMaps(Players.UIActionMap);
		if (Services.levelManager.hasWon)
		{
			starburst.SetActive(true);
			text.Set(0);
		}
		else
		{
			starburst.SetActive(false);
			text.Set(1);
		}
		foreach (var display in displays)
		{
			display.gameObject.SetActive(false);
		}

		for (var i = 0; i < Services.playerManager.AllJoinedPlayers.Count; i++)
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
