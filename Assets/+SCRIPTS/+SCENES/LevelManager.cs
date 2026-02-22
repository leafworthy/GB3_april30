using System;
using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class LevelManager : MonoBehaviour, IService
	{
		static SceneDefinition restartedLevelScene;
		public bool hasWon;
		public GameLevel currentLevel;

		public event Action<GameLevel> OnStopLevel;
		public event Action<GameLevel> OnStartLevel;
		public event Action OnRestartLevel;
		public event Action<Player> OnLevelSpawnedPlayerFromLevel;
		public event Action<Player> OnLevelSpawnedPlayerFromPlayerSetupMenu;
		public event Action OnGameOver;
		float gameStartTime;
		public bool loadInGame;

		[RuntimeInitializeOnLoadMethod]
		static void ResetStatics()
		{
			restartedLevelScene = null;
		}

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

		void StartLevel(GameLevel newLevel)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			currentLevel = newLevel;
			SpawnPlayersIntoLevel();
			OnStartLevel?.Invoke(currentLevel);
			Debug.Log("start level");
			newLevel.OnGameOver += Level_OnGameOver;

			gameStartTime = Time.time;
		}

		void Level_OnGameOver(bool _hasWon)
		{
			EndGame(_hasWon);
			Debug.Log("game over");

		}

		void SpawnPlayersIntoLevel()
		{
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				SpawnPlayerFromLevel(player);
				Debug.Log( "[LEVEL MANAGER]spawned player from level at start of level for player " + player.playerIndex);
			}
		}

		public void SpawnPlayerFromLevel(Player player)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			var playerSpawnPoint = currentLevel.DefaultPlayerSpawnPoint;
			Debug.Log("player spawn point is " + playerSpawnPoint, playerSpawnPoint);
			player.Spawn(playerSpawnPoint.transform.position);
			OnLevelSpawnedPlayerFromLevel?.Invoke(player);
		}

		public void SpawnPlayerFromPlayerSetupMenu(Player player)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			player.Spawn(currentLevel.DefaultPlayerSpawnPoint.transform.position);
			OnLevelSpawnedPlayerFromPlayerSetupMenu?.Invoke(player);
		}





		void LoadLevel(SceneDefinition destinationScene)
		{
			StopLevel();
			Services.sceneLoader.GoToScene(destinationScene);
		}

		void SceneLoaderSceneReadyToStartLevel(SceneDefinition newScene)
		{
			var gameLevel = FindFirstObjectByType<GameLevel>();
			if (gameLevel == null) return;
			StartLevel(gameLevel);
		}

		void StopLevel()
		{
			if (currentLevel == null) return;
			restartedLevelScene = currentLevel.scene;
			currentLevel.StopLevel();
			currentLevel = null;
			OnStopLevel?.Invoke(currentLevel);
		}

		void StopGame()
		{
			StopLevel();
		}

		public void RestartLevel()
		{
			OnRestartLevel?.Invoke();
			StopLevel();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.restartLevel);
		}

		void ClearOldSpawnedPlayer(Player pausingPlayer)
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

		public void EndGame(bool _hasWon)
		{
			OnGameOver?.Invoke();
			hasWon = _hasWon;
			if (currentLevel != null) currentLevel.OnGameOver -= Level_OnGameOver;
			StopLevel();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.GameOverScene);
		}

		public float GetCurrentLevelTimeElapsed() => GetTimeElapsed();

		float GetTimeElapsed()
		{
			if (gameStartTime == 0) return 0f;
			return Time.time - gameStartTime;
		}

		public SceneDefinition GetFirstLevelToLoad()
		{
			if (!loadInGame) return Services.assetManager.Scenes.startingScene;
			return Services.assetManager.Scenes.testScene;
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
			LoadLevel(newScene);
		}


	}
}
