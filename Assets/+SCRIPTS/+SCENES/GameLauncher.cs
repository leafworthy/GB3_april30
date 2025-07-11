using System;
using System.Collections.Generic;
using __SCRIPTS;
using __SCRIPTS.RisingText;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public interface IService
{
	public void StartService();
}
[Serializable]
public class GameLauncher : MonoBehaviour
{
	[SerializeField] private ASSETS assetsManager;
	[SerializeField] private HUDManager hudManagerManager;
	[SerializeField] private EnemyManager enemyManager;
	[SerializeField] private LevelManager levelManager;
	[SerializeField] private Players playersManager;
	[SerializeField] private PlayerStatsManager playerStatsManager;
	[SerializeField] private LootTable lootTable;
	[SerializeField] private ObjectMaker objectMaker;
	[SerializeField] private SceneLoader sceneLoader;
	[SerializeField] private SFX sfxManager;
	[SerializeField] private PauseManager pauseManager;
	[SerializeField] private RisingTextCreator risingTextManager;
	[SerializeField] private UnitStatsManager unitStatsManager;

	protected void OnEnable()
	{
		Debug.Log("SingletonManager: Initializing services...");
		sceneLoader.StartService();
		ServiceLocator.Register(sceneLoader);

		assetsManager.StartService();
		ServiceLocator.Register(assetsManager);

		sfxManager.StartService();
		ServiceLocator.Register(sfxManager);

		unitStatsManager.StartService();
		ServiceLocator.Register(unitStatsManager);

		playersManager.StartService();
		ServiceLocator.Register(playersManager);

		//REQUIRES SceneLoader
		levelManager.StartService();
		ServiceLocator.Register(levelManager);

		//REQUIRES levelManager
		objectMaker.StartService();
		ServiceLocator.Register(objectMaker);

		//REQUIRES levelManager + playerManager
		hudManagerManager.StartService();
		ServiceLocator.Register(hudManagerManager);

		//REQUIRES levelManaager
		playerStatsManager.StartService();
		ServiceLocator.Register(playerStatsManager);

		//REQUIRES levelManager
		enemyManager.StartService();
		ServiceLocator.Register(enemyManager);

		//REQUIRES enemyManager
		lootTable.StartService();
		ServiceLocator.Register(lootTable);



		//REQUIRES levelManager
		pauseManager.StartService();
		ServiceLocator.Register(pauseManager);

		//REQUIRES objectMaker
		risingTextManager.StartService();
		ServiceLocator.Register(risingTextManager);



		Debug.Log("SingletonManager: All services registered successfully.");
	}

	public static class LoadGameManager
	{
		private static bool gameManagerLoaded;
		private static string SceneName => "0_GameManagerScene";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void LoadGameManagerAtStart()
		{
			if (gameManagerLoaded) return;
			Debug.Log("loading game manager scene: " + SceneName);
			SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
			gameManagerLoaded = true;
		}

	}
}

public static class ServiceLocator
{
	private static readonly Dictionary<Type, object> _services = new();

	public static void Register<T>(T instance) where T : class
	{
		var type = typeof(T);
		if (_services.ContainsKey(type)) Debug.LogWarning($"ServiceLocator: Service of type {type.Name} is already registered. Overwriting...");

		_services[type] = instance;
		Debug.Log($"ServiceLocator: Service of type {type.Name} registered successfully.");
	}

	public static T Get<T>() where T : class
	{
		var type = typeof(T);

		if (_services.TryGetValue(type, out var service)) return (T) service;

		throw new InvalidOperationException($"ServiceLocator: Service of type {type.Name} is not registered.");
	}


}
