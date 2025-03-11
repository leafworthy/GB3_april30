public class LevelManager : Singleton<LevelManager>
{
	public SceneDefinition currentGameScene;
	public SceneDefinition lastGameScene;

	void Awake()
	{
		LevelGameScene.OnStop += LevelGameScene_OnStop;
		LevelGameScene.OnStart += LevelGameScene_OnStart;
	}

	private void LevelGameScene_OnStart(SceneDefinition sceneDefinition)
	{
		currentGameScene = sceneDefinition;
	}

	private void LevelGameScene_OnStop(SceneDefinition nextScene)
	{
		lastGameScene = currentGameScene;
	}

	public void RestartCurrentLevel()
	{
		if (currentGameScene != null)
		{
			SceneLoader.I.GoToScene(ASSETS.Scenes.restartLevel);
		}
	}
}