using __SCRIPTS;
using __SCRIPTS.RisingText;
using UnityEngine;

public static class Services
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void ResetStatics()
	{
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
	}

	static PlayerStatsManager _playerStatsManager;
	public static PlayerStatsManager playerStatsManager => _playerStatsManager ?? ServiceLocator.Get<PlayerStatsManager>();
	static SFX _sfx;
	public static SFX sfx => _sfx ?? ServiceLocator.Get<SFX>();
	static ObjectMaker _objectMaker;
	public static ObjectMaker objectMaker => _objectMaker ?? ServiceLocator.Get<ObjectMaker>();

	static LevelManager _levelManager;
	public static LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();

	static Players _playerManager;
	public static Players playerManager => _playerManager ?? ServiceLocator.Get<Players>();

	static EnemyManager _enemyManager;
	public static EnemyManager enemyManager => _enemyManager ?? ServiceLocator.Get<EnemyManager>();

	public static HUDManager hudManager => _hudManager ?? ServiceLocator.Get<HUDManager>();
	static HUDManager _hudManager;
	static PauseManager _pauseManager;
	public static PauseManager pauseManager => _pauseManager ?? ServiceLocator.Get<PauseManager>();
	static SceneLoader _sceneLoader;
	public static SceneLoader sceneLoader => _sceneLoader ?? ServiceLocator.Get<SceneLoader>();

	static AssetManager _assetManager;
	public static AssetManager assetManager => _assetManager ?? ServiceLocator.Get<AssetManager>();
	static RisingTextCreator _risingText;
	public static RisingTextCreator risingText => _risingText ?? ServiceLocator.Get<RisingTextCreator>();
}
