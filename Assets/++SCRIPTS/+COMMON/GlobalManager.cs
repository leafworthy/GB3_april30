using __SCRIPTS._SCENES;
using __SCRIPTS._UI;
using UnityEngine;

namespace __SCRIPTS._COMMON
{
	public class GlobalManager : Singleton<GlobalManager>
	{
		public static int GasGoal;
		public static bool IsInLevel;
		public static bool IsPaused;
		public static Vector3 Gravity = new(0, 4.5f, 0);
	
		private void Start()
		{
			LevelScene.OnStart += () => { IsInLevel = true; };
			LevelScene.OnStop += (t) => { IsInLevel = false; };
			PauseManager.OnPause += x => IsPaused = true;
			PauseManager.OnUnpause += x => IsPaused = false;
			GasGoal = 3;
		}

	

		public static void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}