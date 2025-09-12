using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class LevelManager : MonoBehaviour, IService
	{
		private static SceneDefinition restartedLevelScene;
		public GameLevel currentLevel;

		public event Action<GameLevel> OnStopLevel;
		public event Action<GameLevel> OnStartLevel;
		public event Action<Player> OnLevelSpawnedPlayer;
		public event Action OnGameOver;
		public event Action OnWinGame;
		private float gameStartTime;
		public bool loadInGame;
		public bool canJoinInGame;

		public void StartService()
		{
			Debug.Log("start wtf");
			gameObject.SetActive(true);
			Services.sceneLoader.OnSceneReadyToStartLevel += SceneLoaderSceneReadyToStartLevel;
			canJoinInGame = false;
			Debug.Log("join in game false");
			if (loadInGame) StartGame(GetFirstLevelToLoad());
			else StartGame(Services.assetManager.Scenes.mainMenu);
		}

		public void StartGame(SceneDefinition startingScene)
		{
			Debug.Log("LEVEL MANAGER: StartGame with scene: " + startingScene.sceneName);
			Services.sceneLoader.GoToScene(startingScene);
		}

		private void StartLevel(GameLevel newLevel)
		{
			canJoinInGame = true;
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			currentLevel = newLevel;
			currentLevel.OnGameOver += newLevel_GameOver;
			Debug.Log("LEVEL MANAGER: OnStartLevel");
			SpawnPlayersIntoLevel(currentLevel.DefaultPlayerSpawnPoint);
			OnStartLevel?.Invoke(currentLevel);

			gameStartTime = Time.time;
		}


		private void SpawnPlayersIntoLevel(PlayerSpawnPoint playerSpawnPoint)
		{
			Debug.Log("LEVEL MANAGER: joined players count: " + Services.playerManager.AllJoinedPlayers.Count);
			if (playerSpawnPoint == null)
			{
				Debug.Log("travel point is null");
				return;
			}

			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				Debug.Log("trying to spawn" + player?.name + " at " + playerSpawnPoint.name);
				SpawnPlayerFromInGame(player);
			}
		}

		public void SpawnPlayerFromInGame(Player player)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			player.Spawn(currentLevel.DefaultPlayerSpawnPoint.transform.position);
			Debug.Log("LEVEL MANAGER: OnLevelSpawnedPlayer: " + player.name);
			OnLevelSpawnedPlayer?.Invoke(player);
		}

		private void newLevel_GameOver()
		{
			OnGameOver?.Invoke();
			GoToGameOverScreen();
		}

		private void GoToGameOverScreen()
		{
			StopLevel();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.GameOverScene);
		}

		private void LoadLevel(SceneDefinition destinationScene)
		{
			StopLevel();
			Services.sceneLoader.GoToScene(destinationScene);
		}

		private void SceneLoaderSceneReadyToStartLevel(SceneDefinition newScene)
		{
			Debug.Log("LEVEL MANAGER: scene loader ready to start level: " + newScene.sceneName);
			var gameLevel = FindFirstObjectByType<GameLevel>();
			if (gameLevel == null) return;
			canJoinInGame = true;
			StartLevel(gameLevel);
		}

		private void StopLevel()
		{
			if (currentLevel == null) return;
			restartedLevelScene = currentLevel.scene;
			canJoinInGame = false;
			currentLevel.StopLevel();
			currentLevel.OnGameOver -= newLevel_GameOver;
			currentLevel = null;
			OnStopLevel?.Invoke(currentLevel);
		}

		private void StopGame()
		{
			StopLevel();
		}

		public void RestartLevel()
		{
			StopLevel();
			Services.objectMaker.DestroyAllUnits(null);

			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.restartLevel);
		}

		public void ExitToMainMenu()
		{
			StopGame();

			// Explicitly clear object pools when exiting to main menu
			Services.objectMaker.DestroyAllUnits(null);

			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.mainMenu);
		}

		public void GoBackFromRestart()
		{
			if (restartedLevelScene == null)
			{
				// Fallback to the main starting scene if we lost the restart reference
				Debug.Log("loading starting scene because restartedLevelScene is null");
				LoadLevel(Services.assetManager.Scenes.startingScene);
				return;
			}

			LoadLevel(restartedLevelScene);
		}



		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
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
			if (!loadInGame) return Services.assetManager.Scenes.startingScene;
			return Services.assetManager.Scenes.testScene;
		}
	}
}
