using System;
using System.Collections.Generic;
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
		public event Action<Player> OnLevelSpawnedPlayerFromLevel;
		public event Action<Player> OnLevelSpawnedPlayerFromPlayerSetupMenu;
		public event Action OnGameOver;
		public event Action OnWinGame;
		private float gameStartTime;
		public bool loadInGame;

		public void StartService()
		{
			gameObject.SetActive(true);
			Services.sceneLoader.OnSceneReadyToStartLevel += SceneLoaderSceneReadyToStartLevel;
			if (loadInGame) StartGame(GetFirstLevelToLoad());
			else StartGame(Services.assetManager.Scenes.mainMenu);
		}

		public void StartGame(SceneDefinition startingScene)
		{
			Services.sceneLoader.GoToScene(startingScene);
		}

		private void StartLevel(GameLevel newLevel)
		{
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
				SpawnPlayerFromLevel(player);
			}
		}

		public void SpawnPlayerFromLevel(Player player)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			player.Spawn(CursorManager.GetCamera().transform.position);
			OnLevelSpawnedPlayerFromLevel?.Invoke(player);
		}

		public void SpawnPlayerFromPlayerSetupMenu(Player player)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			player.Spawn(CursorManager.GetCamera().transform.position);
			OnLevelSpawnedPlayerFromPlayerSetupMenu?.Invoke(player);
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
		}

		public void RestartLevel()
		{
			StopLevel();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.restartLevel);
		}





		private void ClearOldSpawnedPlayer(Player pausingPlayer)
		{
			Services.objectMaker.Unmake(pausingPlayer.SpawnedPlayerGO);
			if (pausingPlayer == null) return;
			pausingPlayer.Unalive();
		}
		public void ExitToMainMenu()
		{
			StopGame();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.mainMenu);
		}

		public void GoBackFromRestart()
		{
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
			SpawnPlayerFromLevel(pausingPlayer);
		}



		public void UnspawnPlayer(Player unspawnPlayer)
		{
			if (unspawnPlayer == null) return;
			unspawnPlayer.Unalive();
		}

		public void AdvanceToNextLevel(SceneDefinition newScene)
		{
			if (newScene == null) return;
			Debug.Log("advancing to next level: " + newScene, this);
			LoadLevel(newScene);
		}
	}
}
