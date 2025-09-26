using System;
using __SCRIPTS.Cursor;
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
			gameObject.SetActive(true);
			Services.sceneLoader.OnSceneReadyToStartLevel += SceneLoaderSceneReadyToStartLevel;
			canJoinInGame = false;
			if (loadInGame) StartGame(GetFirstLevelToLoad());
			else StartGame(Services.assetManager.Scenes.mainMenu);
		}

		public void StartGame(SceneDefinition startingScene)
		{
			Services.sceneLoader.GoToScene(startingScene);
		}

		private void StartLevel(GameLevel newLevel)
		{
			canJoinInGame = true;
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			currentLevel = newLevel;
			currentLevel.OnGameOver += newLevel_GameOver;
			SpawnPlayersIntoLevel(currentLevel.DefaultPlayerSpawnPoint);
			OnStartLevel?.Invoke(currentLevel);

			gameStartTime = Time.time;
		}


		private void SpawnPlayersIntoLevel(PlayerSpawnPoint playerSpawnPoint)
		{
			if (playerSpawnPoint == null)
			{
				return;
			}

			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				SpawnPlayerFromInGame(player);
			}
		}

		public void SpawnPlayerFromInGame(Player player)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			player.Spawn(CursorManager.GetCamera().transform.position);
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

		public void StartWinningGame()
		{
		}

		public void RespawnPlayer(Player pausingPlayer)
		{
			if (currentLevel == null) return;
			ClearOldSpawnedPlayer(pausingPlayer);
			SpawnPlayerFromInGame(pausingPlayer);
		}

		private void ClearOldSpawnedPlayer(Player pausingPlayer)
		{
			Services.objectMaker.Unmake(pausingPlayer.SpawnedPlayerGO);
			UnjoinPlayer(pausingPlayer);
		}

		public void UnjoinPlayer(Player pausingPlayer)
		{
			Services.playerManager.UnjoinPlayer(pausingPlayer);
		}

		public void UnspawnPlayer(Player unspawnPlayer)
		{
			if (unspawnPlayer == null) return;
			ClearOldSpawnedPlayer(unspawnPlayer);
		}
	}
}
