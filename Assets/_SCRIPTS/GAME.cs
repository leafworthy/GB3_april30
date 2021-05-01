using System;
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
		public GameObject StartingLevelPrefab;
		private LEVEL StartingLevel;
		private CinemachineVirtualCamera cam;
		public static Vector3 Gravity = new Vector3(0, 4.5f, 0);

		private void Start()
		{
			CHARS.OnCharacterSelectionComplete += CHARS_OnCharacterSelectionComplete;
			StartGame();
		}

		private void CHARS_OnCharacterSelectionComplete(List<Player> joiningPlayers)
		{
			Debug.Log("GAME:Selection complete, starting level...");
			var startingLevelGO = Instantiate(StartingLevelPrefab);
			StartingLevel = startingLevelGO.GetComponent<LEVEL>();
			StartingLevel.PlayLevel(joiningPlayers);
		}

		public void StartGame()
		{
			Debug.Log("Start of GAME");
			MENU.StartMainMenu();
		}

		public void EndGame()
		{
			OnGameEnd?.Invoke();
			StartGame();
		}

		public static List<Player> GetPlayers()
		{
			var players = new List<Player>();
			foreach (var player in I.GetComponents<Player>()) players.Add(player);
			return players;
		}
	}
}
