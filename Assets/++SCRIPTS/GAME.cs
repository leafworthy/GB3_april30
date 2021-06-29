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

	private void Start()
	{
		CharacterSelectionMenu.OnCharacterSelectionComplete += CHARS_OnCharacterSelectionComplete;
		PLAYERS.OnAllPlayersDead += EndGameRestart;
		StartMainMenu();
	}

	private void CHARS_OnCharacterSelectionComplete(List<Player> joiningPlayers)
	{
		if (isPlaying) return;
		Debug.Log("GAME:Selection complete, starting level...");
		StartCoroutine(WaitAndStartLevel(joiningPlayers));

	}

	private IEnumerator WaitAndStartLevel(List<Player> joiningPlayers)
	{
		while (!isPlaying)
		{
			yield return new WaitForSeconds(.05f);
			isPlaying = true;
			Debug.Log("waited");
			PlayLevel(joiningPlayers);
		}
	}

	private void PlayLevel(List<Player> joiningPlayers)
	{
		Debug.Log("instancing level");
		var startingLevelGO = Instantiate(ASSETS.LevelAssets.StartingLevelPrefab);
		currentLevel = startingLevelGO.GetComponent<LEVEL>();
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
		I.isPlaying = false;
		OnGameEnd?.Invoke();
		I.currentLevel.EndLevel();
		I.StartMainMenu();
	}

	public void EndGameRestart()
	{
		isPlaying = false;
		Debug.Log("end game restart");
		OnGameEnd?.Invoke();
		currentLevel.EndLevel();
		Debug.Log("level gone");
		RestartLevel();
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
