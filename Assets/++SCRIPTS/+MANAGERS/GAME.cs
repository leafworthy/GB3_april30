using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	private GameObject startingLevelGO;

	private void Start()
	{
		CharacterSelectionMenu.OnCharacterSelectionComplete += PlayLevel;
		PLAYERS.OnAllPlayersDead += EndGameRestart;
		StartMainMenu();
		InstanceLevel();
	}

	private void InstanceLevel()
	{
		startingLevelGO = Instantiate(ASSETS.LevelAssets.StartingLevelPrefab);
		currentLevel = startingLevelGO.GetComponent<LEVEL>();
		startingLevelGO.SetActive(false);
		Debug.Log("instancing level");
	}

	private void PlayLevel(List<Player> joiningPlayers)
	{
		if (isPlaying) return;
		isPlaying = true;
		currentPlayers = joiningPlayers;
		currentLevel.PlayLevel(currentPlayers);
		OnGameStart?.Invoke();
	}

	private void RestartLevel()
	{
		isPlaying = false;
		StartCoroutine(WaitAndRestartLevel(currentPlayers));
	}

	private IEnumerator WaitAndRestartLevel(List<Player> joiningPlayers)
	{
		while (!isPlaying)
		{
			yield return new WaitForSeconds(1f);
			Debug.Log("restarting level...");
			PlayLevel(joiningPlayers);
		}
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
		RestartLevel();
	}

	private void EndGame()
	{
		isPlaying = false;
		Debug.Log("end game");
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
