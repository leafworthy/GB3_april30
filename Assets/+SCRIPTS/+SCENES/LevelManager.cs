using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class LevelManager : ServiceUser, IService
	{
		private static SceneDefinition restartedLevelScene;
		public GameLevel currentLevel;

		public event Action<GameLevel> OnStopLevel;
		public event Action<GameLevel> OnStartLevel;
		public event Action<Player> OnPlayerSpawned;
		public event Action OnGameOver;
		public event Action OnWinGame;
		private float gameStartTime;
		public bool loadInGame;

		public void StartService()
		{
			Debug.Log("start wtf");
			gameObject.SetActive(true);
			sceneLoader.OnSceneReadyToStartLevel += SceneLoaderSceneReadyToStartLevel;
			if (loadInGame) StartGame(GetFirstLevelToLoad());
		}

		public void StartGame(SceneDefinition startingScene)
		{
			Debug.Log("start game");
			sceneLoader.GoToScene(startingScene);
		}

		private void StartLevel(GameLevel newLevel)
		{
			playerManager.SetActionMaps(Players.PlayerActionMap);
			currentLevel = newLevel;
			currentLevel.OnGameOver += newLevel_GameOver;
			currentLevel.OnPlayerSpawned += p => OnPlayerSpawned?.Invoke(p);
			currentLevel.StartLevel();

			gameStartTime = Time.time;
			Debug.Log("on level starting");
			OnStartLevel?.Invoke(currentLevel);
		}

		private void newLevel_GameOver()
		{
			OnGameOver?.Invoke();
			GoToGameOverScreen();
		}

		private void GoToGameOverScreen()
		{
			StopLevel();
			sceneLoader.GoToScene(assets.Scenes.GameOverScene);
		}

		private void LoadLevel(SceneDefinition destinationScene)
		{
			StopLevel();
			sceneLoader.GoToScene(destinationScene);
		}

		private void SceneLoaderSceneReadyToStartLevel(SceneDefinition newScene)
		{
			Debug.Log("scene loader ready to start level: " + newScene.sceneName);
			var gameLevel = FindFirstObjectByType<GameLevel>();
			if (gameLevel == null) return;

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
			sceneLoader.OnSceneReadyToStartLevel -= SceneLoaderSceneReadyToStartLevel;
		}

		public void RestartLevel()
		{
			StopLevel();
			objectMaker.DestroyAllUnits(null);

			sceneLoader.GoToScene(assets.Scenes.restartLevel);
		}

		public void ExitToMainMenu()
		{
			StopGame();

			// Explicitly clear object pools when exiting to main menu
			objectMaker.DestroyAllUnits(null);

			sceneLoader.GoToScene(assets.Scenes.mainMenu);
		}

		public void GoBackFromRestart()
		{
			if (restartedLevelScene == null)
			{
				// Fallback to the main starting scene if we lost the restart reference
				Debug.Log("loading starting scene because restartedLevelScene is null");
				LoadLevel(assets.Scenes.startingScene);
				return;
			}

			LoadLevel(restartedLevelScene);
		}

		public void SpawnPlayerFromInGame(Player owner)
		{
			Debug.Log("Spawning player from in-game: " + owner);
			currentLevel.SpawnPlayer(owner);
		}

		public void QuitGame()
		{
			sceneLoader.QuitGame();
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

		public SceneDefinition GetFirstLevelToLoad()
		{
			if (!loadInGame) return assets.Scenes.startingScene;
			return assets.Scenes.testScene;
		}


	}
}
