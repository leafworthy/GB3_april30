using System;
using System.Collections.Generic;
using __SCRIPTS;
using __SCRIPTS.RisingText;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public interface IService
{
	public void StartService();
}

public static class Services
{

	public static void ResetStatics()
	{
		Debug.Log("[Services] resetting services statics");
		_services.Clear();
		_playerStatsManager = null;
		_sfx = null;
		_objectMaker = null;
		_levelManager = null;
		_playerManager = null;
		_enemyManager = null;
		_pauseManager = null;
		_sceneLoader = null;
		_assetManager = null;
		_risingText = null;
		_hudManager = null;
		_lootTable = null;
		_gameManager = null;
	}

	public static void RegisterServices()
	{
		Debug.Log("starting services");
		ResetStatics();

		_sceneLoader = Get<SceneLoader>();
		_assetManager = Get<AssetManager>();
		_sfx = Get<SFX>();
		_playerManager = Get<Players>();
		_levelManager = Get<LevelManager>();
		_objectMaker = Get<ObjectMaker>();
		_hudManager = Get<HUDManager>();
		_playerStatsManager = Get<PlayerStatsManager>();
		_enemyManager = Get<EnemyManager>();
		_lootTable = Get<LootTable>();
		_pauseManager = Get<PauseManager>();
		_risingText = Get<RisingTextCreator>();

		_sceneLoader.StartService();
		_assetManager.StartService();
		_sfx.StartService();
		_playerManager.StartService();
		_levelManager.StartService();
		_objectMaker.StartService();
		_hudManager.StartService();
		_playerStatsManager.StartService();
		_enemyManager.StartService();
		_lootTable.StartService();
		_pauseManager.StartService();
		_risingText.StartService();

		_levelManager.StartGame();
	}

	public static PlayerStatsManager playerStatsManager => _playerStatsManager ??= Get<PlayerStatsManager>();
	static PlayerStatsManager _playerStatsManager;
	public static SFX sfx => _sfx ?? Get<SFX>();
	static SFX _sfx;
	public static ObjectMaker objectMaker => _objectMaker ??= Get<ObjectMaker>();
	static ObjectMaker _objectMaker;
	public static LevelManager levelManager => _levelManager ??= Get<LevelManager>();
	static LevelManager _levelManager;
	public static Players playerManager => _playerManager ??= Get<Players>();
	static Players _playerManager;
	public static EnemyManager enemyManager => _enemyManager ??= Get<EnemyManager>();
	static EnemyManager _enemyManager;
	public static HUDManager hudManager => _hudManager ??= Get<HUDManager>();
	static HUDManager _hudManager;
	public static PauseManager pauseManager => _pauseManager ??= Get<PauseManager>();
	static PauseManager _pauseManager;
	public static SceneLoader sceneLoader => _sceneLoader ??= Get<SceneLoader>();
	static SceneLoader _sceneLoader;
	public static AssetManager assetManager => _assetManager ??= Get<AssetManager>();
	static AssetManager _assetManager;
	public static RisingTextCreator risingText => _risingText ??= Get<RisingTextCreator>();
	static RisingTextCreator _risingText;

	public static LootTable lootTable => _lootTable ??= Get<LootTable>();
	static LootTable _lootTable;
	public static GameManager gameManager => _gameManager ??= Get<GameManager>();
	static GameManager _gameManager;

	static readonly Dictionary<Type, object> _services = new();

	static void Register<T>(T instance) where T : class
	{
		Debug.Log($"[ServiceLocator] Registering instance of {typeof(T)}");
		var type = typeof(T);

		_services[type] = instance;
	}

	static T Get<T>() where T : MonoBehaviour
	{
		var type = typeof(T);

		if (_services.TryGetValue(type, out var service)) return (T) service;

		var findItem = UnityEngine.Object.FindFirstObjectByType<T>();
		if (findItem != null)
		{
			Register(findItem);
			return findItem;
		}

		if (findItem != null) throw new InvalidOperationException($"ServiceLocator: Service of type {type.Name} is not registered.");
		Debug.LogWarning($"ServiceLocator: Service of type {type.Name} not found.");
		//var go = new GameObject(type.Name);
		//var newService = go.AddComponent<T>();
		//Register(newService);
		//return newService;
		return null;
	}

	public static class LoadGameManager
	{
		static bool hasStarted;
		static InputAction ResetAction;
		static string SceneName => "0_GameManagerScene";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void LoadGameManagerAtStart()
		{
			if (hasStarted) return;
			hasStarted = true;
			Debug.Log("game launcher launching");
			ResetAction = new InputAction(binding: "<Keyboard>/f1");
			ResetAction.performed += PressedReset;
			ResetAction.Enable();
			Debug.Log("checking if game manager is in scene");

			LoadNewGameManager();
			//RegisterServices();
		}

		static void ResetGameManager()
		{


			Debug.LogWarning("Destoying existing game manager");
			UnityEngine.Object.Destroy(_gameManager?.gameObject);

			_gameManager = null;


			LoadNewGameManager();
		}

		static void LoadNewGameManager()
		{

			SceneManager.sceneLoaded += OnSceneLoaded;
			Debug.Log("loading new game manager");
			SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
		}

		static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded; // unsubscribe immediately
			RegisterServices();
		}
		static void PressedReset(InputAction.CallbackContext obj)
		{
			ResetGameManager();
		}
	}
}
