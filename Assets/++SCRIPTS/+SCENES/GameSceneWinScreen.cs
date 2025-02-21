using UnityEngine.Video;

public class GameSceneWinScreen : GameScene
{
	public VideoPlayer vid;
	protected void Start()
	{
		vid = GetComponent<VideoPlayer>();
		vid.loopPointReached += GoToMainMenu;
		
	}

	private void GoToMainMenu(VideoPlayer source)
	{ 
		GoToScene(Type.MainMenu);
	}


}