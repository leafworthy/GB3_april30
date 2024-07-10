using UnityEngine;

public class GlobalManager : Singleton<GlobalManager>
{
	public static int GasGoal;
	public static bool IsInLevel;
	public static bool IsPaused;
	public static Vector3 Gravity = new(0, 4.5f, 0);
	
	private void Start()
	{
	
		GasGoal = 3;
	}

	private void Update()
	{
		//Debug.Log(IsInLevel);
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