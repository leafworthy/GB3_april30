using UnityEngine.Video;

namespace GangstaBean.Scenes
{
	/// <summary>
	/// Handles the victory screen with video playback
	/// </summary>
	public class GameSceneWinScreen : GameScene
	{
		public VideoPlayer vid;
	
		protected void Start()
		{
			vid = GetComponent<VideoPlayer>();
			vid.loopPointReached += GoToMainMenu;
		}
	
		private void OnDestroy()
		{
			if (vid != null)
				vid.loopPointReached -= GoToMainMenu;
		}

		private void GoToMainMenu(VideoPlayer source)
		{ 
			SceneLoader.I.GoToScene( ASSETS.Scenes.GameOverScene );
		}
	}
}