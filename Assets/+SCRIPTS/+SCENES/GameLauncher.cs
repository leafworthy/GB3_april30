using System;
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
	[FormerlySerializedAs("assetsManager"),SerializeField] private AssetManager assetManagerManager;
	[SerializeField] private HUDManager hudManager;
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
	private static bool hasLaunched;
	private static bool servicesStarted;

	protected void OnEnable()
	{
		if (servicesStarted) return;
		servicesStarted = true;
		sceneLoader.StartService();
		ServiceLocator.Register(sceneLoader);

		assetManagerManager.StartService();
		ServiceLocator.Register(assetManagerManager);

		sfxManager.StartService();
		ServiceLocator.Register(sfxManager);

		playersManager.StartService();
		ServiceLocator.Register(playersManager);

		//REQUIRES SceneLoader
		levelManager.StartService();
		ServiceLocator.Register(levelManager);

		//REQUIRES levelManager
		objectMaker.StartService();
		ServiceLocator.Register(objectMaker);

		//REQUIRES levelManager + playerManager
		hudManager.StartService();
		ServiceLocator.Register(hudManager);

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



	}

	public static class LoadGameManager
	{
		private static string SceneName => "0_GameManagerScene";


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void LoadGameManagerAtStart()
		{
			if (hasLaunched) return;
			hasLaunched = true;
			var manager = FindFirstObjectByType<GameManager>();
			if (manager == null)
			{

				SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
			}

		}

	}
}
