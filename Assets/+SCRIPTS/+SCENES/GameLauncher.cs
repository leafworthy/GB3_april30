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
	private static bool hasLaunched;
	private static bool servicesStarted;

	protected void OnEnable()
	{
		if (servicesStarted) return;
		servicesStarted = true;
		Debug.Log("GAME LAUNCHER: Initializing services...");
		sceneLoader.StartService();
		ServiceLocator.Register(sceneLoader);

		assetsManager.StartService();
		ServiceLocator.Register(assetsManager);

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



		Debug.Log("GAME LAUNCHER: All services registered successfully.");
	}

	public static class LoadGameManager
	{
		private static string SceneName => "0_GameManagerScene";


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void LoadGameManagerAtStart()
		{
			if (hasLaunched) return;
			hasLaunched = true;
			Debug.Log("GAME LAUNCHER: LoadGameManagerAtStart()");
			var manager = FindFirstObjectByType<GameManager>();
			if (manager == null)
			{
				Debug.Log("GAME LAUNCHER: loading game manager scene: " + SceneName);

				SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
			}
			else
			{
				Debug.Log("manager found");
			}

		}

	}
}
