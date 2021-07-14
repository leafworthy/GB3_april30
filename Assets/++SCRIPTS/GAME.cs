using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

[Serializable]
public class GAME : Singleton<GAME>
{

	public static Action OnGameEnd;
	public static Action OnGameStart;
	public static Vector3 Gravity = new Vector3(0, 4.5f, 0);

	private LEVEL currentLevel;
	private List<Player> currentPlayers;
	private bool isPlaying;
	public  bool isTesting;

	private void Start()
	{
		CharacterSelectionMenu.OnCharacterSelectionComplete += CHARS_OnCharacterSelectionComplete;
		PLAYERS.OnAllPlayersDead += EndGameRestart;
		StartMainMenu();
		Debug.Log("instancing level");

	}

	private void CHARS_OnCharacterSelectionComplete(List<Player> joiningPlayers)
	{
		var startingLevelGO = Instantiate(ASSETS.LevelAssets.StartingLevelPrefab);
		currentLevel = startingLevelGO.GetComponent<LEVEL>();
		if (isPlaying) return;
		Debug.Log("GAME:Selection complete, starting level...");
		isPlaying = true;
		PlayLevel(joiningPlayers);

	}

	private IEnumerator WaitAndStartLevel(List<Player> joiningPlayers)
	{
		while (!isPlaying)
		{
			yield return new WaitForSeconds(.05f);
			Debug.Log("restarting level...");
			isPlaying = true;
			PlayLevel(joiningPlayers);
		}
	}

	private void PlayLevel(List<Player> joiningPlayers)
	{
		currentLevel.gameObject.SetActive(true);
		currentPlayers = joiningPlayers;
		currentLevel.PlayLevel(joiningPlayers);
		OnGameStart?.Invoke();
	}

	private void RestartLevel()
	{
		isPlaying = false;
		StartCoroutine(WaitAndStartLevel(currentPlayers));
	}

	public void StartMainMenu()
	{
		MENU.StartMainMenu();
	}

	public static void EndGameMainMenu()
	{
		I.EndGame();
		I.currentLevel.EndLevel();
		I.StartMainMenu();
	}

	public void EndGameRestart()
	{
		EndGame();
		currentLevel.EndLevel();
		Debug.Log("level gone");
		RestartLevel();
	}

	private void EndGame()
	{
		isPlaying = false;
		Debug.Log("end game restart");
		OnGameEnd?.Invoke();
	}

	public static List<Player> GetPlayers()
	{
		return I.GetComponents<Player>().ToList();
	}

	public static List<IPlayerController> getPlayerControllersInLevel()
	{
		return I.currentLevel.playerControllersInLevel;
	}
}
