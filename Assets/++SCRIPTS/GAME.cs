using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

namespace _SCRIPTS
{
	[Serializable]
	public class GAME : Singleton<GAME>
	{

		public static Action OnGameEnd;
		public static Action OnGameStart;
		public GameObject StartingLevelPrefab;
		public LEVEL CurrentLevel;
		private CinemachineVirtualCamera cam;
		public static Vector3 Gravity = new Vector3(0, 4.5f, 0);
		private List<Player> currentPlayers;
		private bool isPlaying;

		private void Start()
		{
			CHARS.OnCharacterSelectionComplete += CHARS_OnCharacterSelectionComplete;
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
			var startingLevelGO = Instantiate(StartingLevelPrefab);
			CurrentLevel = startingLevelGO.GetComponent<LEVEL>();
			currentPlayers = joiningPlayers;
			CurrentLevel.PlayLevel(joiningPlayers);
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
			I.CurrentLevel.EndLevel();
			I.StartMainMenu();
		}

		public void EndGameRestart()
		{
			isPlaying = false;
			Debug.Log("end game restart");
			OnGameEnd?.Invoke();
			CurrentLevel.EndLevel();
			Debug.Log("level gone");
			RestartLevel();
		}

		public static List<Player> GetPlayers()
		{
			var players = new List<Player>();
			foreach (var player in I.GetComponents<Player>()) players.Add(player);
			return players;
		}
	}
}
