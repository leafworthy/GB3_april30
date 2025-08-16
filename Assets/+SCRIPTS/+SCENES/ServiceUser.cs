using __SCRIPTS;
using __SCRIPTS.RisingText;
using UnityEngine;

public class ServiceUser : MonoBehaviour
{
	private PlayerStatsManager _playerStatsManager;
	protected PlayerStatsManager playerStatsManager => _playerStatsManager ?? ServiceLocator.Get<PlayerStatsManager>();
	private SFX _sfx;
	protected SFX sfx => _sfx ?? ServiceLocator.Get<SFX>();
	private ObjectMaker _objectMaker;
	protected ObjectMaker objectMaker => _objectMaker ?? ServiceLocator.Get<ObjectMaker>();

	private LevelManager _levelManager;
	protected LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();

	private Players _playerManager;
	protected Players playerManager => _playerManager ?? ServiceLocator.Get<Players>();

	private EnemyManager _enemyManager;
	protected EnemyManager enemyManager => _enemyManager ?? ServiceLocator.Get<EnemyManager>();

	private PauseManager _pauseManager;
	protected PauseManager pauseManager => _pauseManager ?? ServiceLocator.Get<PauseManager>();
	private SceneLoader _sceneLoader;
	protected SceneLoader sceneLoader => _sceneLoader ?? ServiceLocator.Get<SceneLoader>();

	private AssetManager assetManager;
	protected AssetManager AssetManager => assetManager ?? ServiceLocator.Get<AssetManager>();
	private RisingTextCreator _risingText;
	protected RisingTextCreator risingText => _risingText ?? ServiceLocator.Get<RisingTextCreator>();
}
