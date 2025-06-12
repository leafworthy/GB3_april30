using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class LevelManager : Singleton<LevelManager>
	{
		public TravelPoint RespawnTravelPoint;
		private static string originSpawnPointID;
		private static string destinationSpawnPointID;
		private static SceneDefinition restartedLevelScene;
		public GameLevel currentLevel;

		public event Action<GameLevel> OnStopLevel;
		public event Action<GameLevel> OnStartLevel;
		public event Action<Player> OnPlayerSpawned;
		public event Action OnGameOver;
		public event Action OnWinGame;
		private float gameStartTime;

		private Dictionary<Character,GameObject> persistentCharacters;

		private void RegisterPersistentCharacter(Character character, GameObject characterPrefab)
		{
			if (persistentCharacters == null)
			{
				persistentCharacters = new Dictionary<Character, GameObject>();
			}

			if (persistentCharacters.TryAdd(character, characterPrefab)) return;

		}
		protected void Start()
		{
			gameObject.SetActive(true);
		}

		public void StartGame()
		{
			SceneLoader.I.GoToScene(ASSETS.Scenes.startingScene);
			SceneLoader.I.OnSceneReadyToStartLevel += SceneLoaderSceneReadyToStartLevel;

		}

		private void StartLevel(GameLevel newLevel)
		{

			Players.SetActionMaps(Players.PlayerActionMap);
			currentLevel = newLevel;
			currentLevel.OnGameOver += newLevel_GameOver;
			currentLevel.OnPlayerSpawned += (p) => OnPlayerSpawned?.Invoke(p);
			currentLevel.StartLevel();


		gameStartTime = Time.time;
			OnStartLevel?.Invoke(currentLevel);
		}



		private void newLevel_OnLevelRestart()
		{
			RestartLevel();
		}

		private void newLevel_GameOver()
		{
			OnGameOver?.Invoke();
			GoToGameOverScreen();
		}

		private void GoToGameOverScreen()
		{
			StopLevel();
			SceneLoader.I.GoToScene(ASSETS.Scenes.GameOverScene);
		}

		public void StartNextLevel(TravelPoint travelPoint)
		{
			LoadLevel(null);
		}

		private void LoadLevel(SceneDefinition destinationScene)
		{
			StopLevel();
			SceneLoader.I.GoToScene(destinationScene);
		}


		private void SceneLoaderSceneReadyToStartLevel(SceneDefinition newScene)
		{

			var gameLevel = FindFirstObjectByType<GameLevel>();
			if(gameLevel == null)
			{


				return;
			}

			StartLevel(gameLevel);
		}

		private void StopLevel()
		{
			if (currentLevel == null) return;
			restartedLevelScene = currentLevel.scene;

			currentLevel.StopLevel();
			currentLevel.OnGameOver -= newLevel_GameOver;
			currentLevel = null;
			OnStopLevel?.Invoke(currentLevel);
		}

		private void StopGame()
		{
			StopLevel();
			SceneLoader.I.OnSceneReadyToStartLevel -= SceneLoaderSceneReadyToStartLevel;
		}


		public void RestartLevel()
		{
			StopLevel();
			
			// Explicitly clear object pools on restart to prevent lingering objects
			if (ObjectMaker.I != null)
			{
				ObjectMaker.I.DestroyAllUnits(null);

			}
			
			SceneLoader.I.GoToScene(ASSETS.Scenes.restartLevel);
		}



		public void ExitToMainMenu()
		{
			StopGame();
			
			// Explicitly clear object pools when exiting to main menu
			if (ObjectMaker.I != null)
			{
				ObjectMaker.I.DestroyAllUnits(null);

			}
			
			SceneLoader.I.GoToScene(ASSETS.Scenes.mainMenu);
		}

		public void SetRestartScene(SceneDefinition scene)
		{
			restartedLevelScene = scene;

		}

		public void GoBackFromRestart()
		{


			if (restartedLevelScene == null)
			{

				// Fallback to the main starting scene if we lost the restart reference
				LoadLevel(ASSETS.Scenes.startingScene);
				return;
			}
			LoadLevel(restartedLevelScene);
		}


		public void SpawnPlayerFromInGame(Player owner)
		{
			if (Players.I.AllJoinedPlayers.Count <= 0)
			{

				return;
			}
			var p1 =  Players.I.AllJoinedPlayers[0];
			if (p1 == null) return;

			RegisterPersistentCharacter(owner.CurrentCharacter, currentLevel.SpawnPlayerFromSky(owner, p1.SpawnedPlayerGO.transform.position));
			OnPlayerSpawned?.Invoke(owner);
		}

		public void QuitGame()
		{
			SceneLoader.I.QuitGame();
		}

		public void WinGame()
		{
			OnWinGame?.Invoke();
		}

		public void StartWinningGame()
		{
			var graphNodePositioner = FindFirstObjectByType<GridCulling>();
			graphNodePositioner.StopCulling();
		}

		public float GetCurrentLevelTimeElapsed() => GetTimeElapsed();

		private float GetTimeElapsed()
		{
			if (gameStartTime == 0) return 0f;
			return Time.time - gameStartTime;
		}
	}
}
