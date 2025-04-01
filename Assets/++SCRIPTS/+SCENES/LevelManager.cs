using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class LevelManager : Singleton<LevelManager>
	{
		public TravelPoint RespawnTravelPoint;
		public bool IsInGame;
		private static string originSpawnPointID;
		private static string destinationSpawnPointID;
		private static SceneDefinition restartedLevelScene;
		private static GameLevel currentLevel;

		public static event Action<GameLevel> OnStopLevel;
		public static event Action<GameLevel> OnStartLevel;
		public static event Action<Player> OnPlayerSpawned;
		public static event Action OnGameOver;

		private static TravelPoint _currentTravelPoint;
		private static Dictionary<Character,GameObject> persistentCharacters;

		private void RegisterPersistentCharacter(Character character, GameObject characterPrefab)
		{
			if (persistentCharacters == null)
			{
				persistentCharacters = new Dictionary<Character, GameObject>();
			}

			if (persistentCharacters.TryAdd(character, characterPrefab)) return;
			Debug.Log("Character already registered");
		}
		protected void Start()
		{
			gameObject.SetActive(true);
		}

		public void StartGame()
		{
			SceneLoader.I.GoToScene(ASSETS.Scenes.startingScene);
			SceneLoader.OnSceneReadyToStartLevel += SceneLoaderSceneReadyToStartLevel;
			
			_currentTravelPoint = null;
		}

		private void StartLevel(GameLevel newLevel)
		{
			Debug.Log("start level" + newLevel.name);
			Players.SetActionMaps(Players.PlayerActionMap);
			currentLevel = newLevel;
			currentLevel.StartLevel();
			IsInGame = true;
			currentLevel.OnGameOver += newLevel_GameOver;
			currentLevel.OnLevelRestart += newLevel_OnLevelRestart;
			OnStartLevel?.Invoke(currentLevel);
		}

	

		private void newLevel_OnLevelRestart()
		{
			RestartLevel();
		}

		private void newLevel_GameOver()
		{
			OnGameOver?.Invoke();
			LoadLevel(ASSETS.Scenes.GameOverScene);
		}

		public void StartNextLevel(TravelPoint travelPoint)
		{
			_currentTravelPoint = travelPoint;
			LoadLevel(null);
		}

		private void LoadLevel(SceneDefinition destinationScene)
		{
			StopLevel();
			SceneLoader.I.GoToScene(destinationScene);
		}

	
		private void SceneLoaderSceneReadyToStartLevel(SceneDefinition newScene)
		{
			Debug.Log(newScene.sceneName + " scene loaded, starting level");
			var gameLevel = FindFirstObjectByType<GameLevel>();
			if(gameLevel == null)
			{
			
				Debug.Log("no game level in scene");
				return;
			}
		
			StartLevel(gameLevel);
		}


		public void StopLevel()
		{
			if (currentLevel == null) return;
			restartedLevelScene = currentLevel.scene;
			currentLevel.StopLevel();
			currentLevel.OnGameOver -= newLevel_GameOver;
			currentLevel.OnLevelRestart -= newLevel_OnLevelRestart;
			currentLevel = null;
			IsInGame = false;
			OnStopLevel?.Invoke(currentLevel);
		}

		private void StopGame()
		{
			StopLevel();
			SceneLoader.OnSceneReadyToStartLevel -= SceneLoaderSceneReadyToStartLevel;
		}
		

		public void RestartLevel()
		{
			StopLevel();
			SceneLoader.I.GoToScene(ASSETS.Scenes.restartLevel);
		}

		public void ExitToMainMenu()
		{
			StopGame();
			
			SceneLoader.I.GoToScene(ASSETS.Scenes.GameOverScene);
		}

		public void GoBackFromRestart()
		{
			if (restartedLevelScene == null)
			{
				Debug.LogError("no restart scene");
			}
			LoadLevel(restartedLevelScene);
		}

	
		public void SpawnPlayerFromInGame(Player owner)
		{
			if (Players.AllJoinedPlayers.Count <= 0)
			{
				Debug.LogError("no players in game");
				return;
			}
			var p1 =  Players.AllJoinedPlayers[0];
			if (p1 == null) return;
			
			RegisterPersistentCharacter(owner.CurrentCharacter, currentLevel.SpawnPlayerAt(owner, p1.SpawnedPlayerGO.transform.position, true));
			OnPlayerSpawned?.Invoke(owner);
		}

		public void QuitGame()
		{
			SceneLoader.I.QuitGame();
		}

		public void WinGame()
		{
			OnWinGame?.Invoke();
			LoadLevel(ASSETS.Scenes.WinScene);
		}

		public void StartWinningGame()
		{
			var graphNodePositioner = FindFirstObjectByType<GridCulling>();
			graphNodePositioner.StopCulling();
		}

		public static event Action OnWinGame;
	}
}