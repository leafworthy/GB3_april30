public class SINGLETONS : Singleton<SINGLETONS>
{
	private void Start()
	{
		StartSingletons();
	}

	private void StartSingletons()
	{
		//APP
		ASSETS.AddToScene(gameObject);
		MAKER.AddToScene(gameObject);
		MENUS.AddToScene(gameObject);
		AUDIO.AddToScene(gameObject);
		LEVELS.AddToScene(gameObject);
		DRAW.AddToScene(gameObject);

		//IN GAME
		HUDS.AddToScene(gameObject);
		ENEMIES.AddToScene(gameObject);
		PLAYERS.AddToScene(gameObject);
		CURSOR.AddToScene(gameObject);
		STUNNER.AddToScene(gameObject);
		SHAKER.AddToScene(gameObject);
	}
}
