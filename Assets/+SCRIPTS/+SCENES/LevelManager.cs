using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class LevelManager : MonoBehaviour, IService
	{
		SceneDefinition currentLevelSceneToRestartTo;
		public bool hasWon;

		public event Action OnStopLevel;
		public event Action OnStartLevel;
		public event Action OnRestartLevel;
		public event Action<Player> OnLevelSpawnedPlayerFromLevel;
		public event Action<Player> OnLevelSpawnedPlayerFromPlayerSetupMenu;
		public event Action OnGameOver;
		float gameStartTime;

		public void StartService()
		{
			Debug.Log("LevelManager start service");
			gameObject.SetActive(true);
			currentLevelSceneToRestartTo = null;
			Services.sceneLoader.OnSceneReadyToStartLevel += SceneLoaderSceneReadyToStartLevel;
		}

		public void StartGame()
		{
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.mainMenu);
		}

		public void StartFirstLevel()
		{
			Services.sceneLoader.GoToScene(GetFirstLevelToLoad());
		}

		void StartLevel(SceneDefinition newLevel)
		{
			Services.playerManager.SetActionMaps(Players.PlayerActionMap);
			currentLevelSceneToRestartTo = newLevel;
			SpawnPlayersIntoLevel();
			OnStartLevel?.Invoke();
			Services.objectMaker.PoolObjects();
			Debug.Log("start level");
			Services.playerManager.OnAllJoinedPlayersDead += Level_OnGameOver;
			hasWon = false;
			gameStartTime = Time.time;
		}

		void OnDisable()
		{
			Services.playerManager.OnAllJoinedPlayersDead -= Level_OnGameOver;
			OnStopLevel = null;
			OnStartLevel = null;
			OnRestartLevel = null;
			OnLevelSpawnedPlayerFromLevel = null;
			OnLevelSpawnedPlayerFromPlayerSetupMenu = null;
			OnGameOver = null;
		}

		void Level_OnGameOver()
		{
			EndGame(hasWon);
			Debug.Log("game over");
		}

		void SpawnPlayersIntoLevel()
		{
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				if (player.state == Player.State.Dead) return;
				SpawnPlayerFromLevel(player);
				Debug.Log("[LEVEL MANAGER]spawned player from level at start of level for player " + player.playerIndex);
			}
		}

		void SpawnPlayerFromLevel(Player player)
		{
			var spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();
			player.Spawn(spawnPoint.transform.position);
			OnLevelSpawnedPlayerFromLevel?.Invoke(player);
		}

		public void SpawnPlayerFromPlayerSetupMenu(Player player)
		{
			var spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();
			player.Spawn(spawnPoint.transform.position);
			OnLevelSpawnedPlayerFromPlayerSetupMenu?.Invoke(player);
		}

		void LoadLevel(SceneDefinition destinationScene)
		{
			StopLevel();
			Services.sceneLoader.GoToScene(destinationScene);
		}

		void SceneLoaderSceneReadyToStartLevel(SceneDefinition newScene)
		{
			StartLevel(newScene);
		}

		void StopLevel()
		{
			if (currentLevelSceneToRestartTo == null) return;
			Services.enemyManager.ClearEnemies();
			Services.objectMaker.DestroyAllUnits();
			OnStopLevel?.Invoke();
		}

		void StopGame()
		{
			StopLevel();
		}

		public void RestartLevel()
		{
			OnRestartLevel?.Invoke();
			StopLevel();
			hasWon = false;
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.restartLevel);
		}

		public void ExitToMainMenu()
		{
			StopGame();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.mainMenu);
		}

		public void GoBackFromRestart()
		{
			LoadLevel(currentLevelSceneToRestartTo);
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
			StopLevel();
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.GameOverScene);
		}

		public float GetCurrentLevelTimeElapsed() => GetTimeElapsed();

		float GetTimeElapsed()
		{
			if (gameStartTime == 0) return 0f;
			return Time.time - gameStartTime;
		}

		public SceneDefinition GetFirstLevelToLoad() => Services.assetManager.Scenes.startingScene;

		public void RespawnPlayer(Player pausingPlayer)
		{
			pausingPlayer.StartCharacterSelectMenu();
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

		public void SetGameWon(bool b)
		{
			Debug.Log("game set to win");
			hasWon = b;
		}
	}
}
