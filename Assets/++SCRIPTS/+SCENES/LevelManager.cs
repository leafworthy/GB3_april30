using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
	private SceneDefinition _originPoint;
	private static string originSpawnPointID;
	private static string destinationSpawnPointID;
	private static SceneDefinition restartedLevelScene;
	private static GameLevel currentLevel;

	public static event Action<GameLevel> OnStopLevel;
	public static event Action<GameLevel> OnStartLevel;
	public static event Action<Player> OnPlayerSpawned;

	private static TravelPoint _currentTravelPoint;
	private static Dictionary<Character,GameObject> persistentCharacters;


	private void CleanUpPersistentCharacters()
	{
		foreach (var character in persistentCharacters)
		{
			Destroy(character.Value);
		}

		persistentCharacters.Clear();
	}
	
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
		SceneLoader.I.GoToScene(ASSETS.Scenes.gangstaBeanHouse);
		SceneLoader.OnSceneLoaded += SceneLoader_SceneLoaded;
		_currentTravelPoint = null;
	}

	private void StartLevel(GameLevel newLevel)
	{
		Debug.Log("start level" + newLevel.name);
		Players.SetActionMaps(Players.PlayerActionMap);
		currentLevel = newLevel;
		currentLevel.StartLevel(_originPoint);
		
		currentLevel.OnLevelFinished += newLevel_OnLevelFinished;
		OnStartLevel?.Invoke(currentLevel);
	}

	private void newLevel_OnLevelFinished(TravelPoint travelPoint)
	{
		if (travelPoint == null)
		{
			//death, restart
			RestartLevel();
		}
		StartNextLevel(travelPoint);
	}

	public void StartNextLevel(TravelPoint travelPoint)
	{
		_currentTravelPoint = travelPoint;
		LoadLevel(SceneLoader.I.GetCurrentSceneDefinition(), travelPoint.destinationScene);
	}

	private void LoadLevel(SceneDefinition originScene, SceneDefinition destinationScene)
	{
		StopLevel();
		_originPoint = originScene;
		SceneLoader.I.GoToScene(destinationScene);
	}

	
	private void SceneLoader_SceneLoaded(SceneDefinition newScene)
	{
		Debug.Log(newScene.sceneName + " scene loaded, starting level");
		var gameLevel = FindFirstObjectByType<GameLevel>();
		if(gameLevel == null) Debug.LogError("no game level in scene");
		
		StartLevel(gameLevel);
	}


	public void StopLevel()
	{
		if (currentLevel == null) return;
		currentLevel.StopLevel();
		OnStopLevel?.Invoke(currentLevel);
		currentLevel.OnLevelFinished -= newLevel_OnLevelFinished;
		currentLevel = null;
	}

	private void StopGame()
	{
		StopLevel();
		SceneLoader.OnSceneLoaded -= SceneLoader_SceneLoaded;
	}

	public void RestartLevel()
	{
		StopLevel();
		restartedLevelScene = currentLevel.scene;
		SceneLoader.I.GoToScene(ASSETS.Scenes.restartLevel);
	}

	public void ExitToMainMenu()
	{
		StopGame();
		SceneLoader.I.GoToScene(ASSETS.Scenes.mainMenu);
	}

	public void GoBackFromRestart()
	{
		if (restartedLevelScene == null)
		{
			Debug.LogError("no restart scene");
		}
		LoadLevel(restartedLevelScene,restartedLevelScene);
		SceneLoader.I.GoToScene(restartedLevelScene);
	}

	public void SpawnPlayerFromCharacterSelectScreen(Player owner)
	{
		if (currentLevel == null)
		{
			Debug.LogError("no current level");
			return;
		}
		RegisterPersistentCharacter( owner.CurrentCharacter, currentLevel.SpawnPlayer(owner, _currentTravelPoint, true));
		OnPlayerSpawned?.Invoke(owner);
	}

	public void QuitGame()
	{
		SceneLoader.I.QuitGame();
	}
}