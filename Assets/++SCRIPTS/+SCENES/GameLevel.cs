using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameLevel : MonoBehaviour
{
	public TravelPoint defaultTravelPoint;
	public SceneDefinition scene;
	private List<TravelPoint> getSpawnPoints() => FindObjectsByType<TravelPoint>(FindObjectsSortMode.None).ToList();

	public event Action<TravelPoint> OnLevelFinished;

	private bool hasSpawnPoint(TravelPoint travelPoint) => getSpawnPoints().Contains(travelPoint);

	private void Start()
	{
		Players.OnAllJoinedPlayersDead += LoseLevel;
	}

	public GameObject SpawnPlayer(Player player, TravelPoint travelPoint, bool fallFromSky)
	{
		if (!hasSpawnPoint(travelPoint)) travelPoint = defaultTravelPoint;
		Debug.Log($"Spawning player {player.playerIndex}");
		Players.SetActionMaps(Players.PlayerActionMap);
		return player.Spawn(travelPoint, fallFromSky);
	}

	public void StopLevel()
	{
		Players.OnAllJoinedPlayersDead -= LoseLevel;
	}

	private void LoseLevel()
	{
		OnLevelFinished?.Invoke(null);
		StopLevel();
	}

	public void StartLevel(SceneDefinition originScene)
	{
		var allSpawnPoints = getSpawnPoints();
		
		var spawnPoint = allSpawnPoints.Find(x => x.destinationScene == originScene);
		if (spawnPoint == null)
		{
			Debug.Log("No spawn point found that returns to: " + originScene.sceneName);
			spawnPoint = defaultTravelPoint;
			
			
		}

		SpawnPlayers(spawnPoint);
		
	}

	private void SpawnPlayers(TravelPoint travelPoint)
	{
		if (travelPoint == null) travelPoint = defaultTravelPoint;
		Debug.Log("fall from sky " + travelPoint.fallFromSky);
		for (var i = 0; i < Players.AllJoinedPlayers.Count; i++)
		{
			var player = Players.AllJoinedPlayers[i];
			if (player == null) continue;
			if (player.SpawnedPlayerGO != null) continue;
			SpawnPlayer(player, travelPoint, travelPoint.fallFromSky);
		}
	}
}