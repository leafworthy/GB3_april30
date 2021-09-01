using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class LEVELS : Singleton<LEVELS>
{
	public static Action OnLevelStop;
	public static Action<List<Player>> OnLevelStart;
	public static Vector3 Gravity = new Vector3(0, 4.5f, 0);
	private static GameObject startingLevelGO;
	private static LevelHandler currentLevelHandler;

	private void Start()
	{
		InstanceLevel();
		currentLevelHandler.OnLevelStart += LevelStarts;
		currentLevelHandler.OnLevelStop += LevelStops;
	}

	public static void PlayLevel(List<Player> joiningPlayers)
	{
		currentLevelHandler.PlayLevel(joiningPlayers);
	}

	public static void StopLevelGoToMenu(MENUS.Type menuType)
	{
		Debug.Log("end level " + menuType);
		currentLevelHandler.StopLevel();
		MENUS.ChangeMenu(menuType);
	}
	private void LevelStarts(List<Player> joiningPlayers)
	{
		OnLevelStart?.Invoke(joiningPlayers);
	}

	private void LevelStops()
	{
		OnLevelStop?.Invoke();
	}

	private void InstanceLevel()
	{
		startingLevelGO = Instantiate(ASSETS.LevelAssets.StartingLevelPrefab);
		currentLevelHandler = startingLevelGO.GetComponent<LevelHandler>();
		startingLevelGO.SetActive(false);
	}




}
