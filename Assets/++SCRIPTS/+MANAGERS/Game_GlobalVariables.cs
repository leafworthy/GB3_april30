using System;
using UnityEngine;

public class Game_GlobalVariables : Singleton<Game_GlobalVariables>
{
	public static int GasGoal;
	public static bool IsInLevel;
	public static bool IsPaused;
	public static Vector3 Gravity = new(0, 4.5f, 0);
	
	private void Start()
	{
		Level.OnStart += () => { IsInLevel = true; };
		Level.OnStop += (t) => { IsInLevel = false; };
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