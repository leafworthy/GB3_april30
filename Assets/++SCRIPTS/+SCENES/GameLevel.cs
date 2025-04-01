using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameLevel : MonoBehaviour
	{
		public TravelPoint defaultTravelPoint;
		public SceneDefinition scene;
		public GraphNodePositioner nodePositioner;
		public event Action OnLevelRestart;
		public event Action OnGameOver;
		private List<TravelPoint> getSpawnPoints() => FindObjectsByType<TravelPoint>(FindObjectsSortMode.None).ToList();
		
		private bool hasSpawnPoint(TravelPoint travelPoint) => getSpawnPoints().Contains(travelPoint);

		private void Start()
		{
			Players.OnAllJoinedPlayersDead += LoseLevel;
			nodePositioner = GetComponent<GraphNodePositioner>();
		}

		public GameObject SpawnPlayer(Player player, TravelPoint travelPoint, bool fallFromSky)
		{
			if (!hasSpawnPoint(travelPoint)) travelPoint = defaultTravelPoint;
			Debug.Log($"Spawning player {player.playerIndex}");
			Players.SetActionMaps(Players.PlayerActionMap);
			return player.Spawn(travelPoint.transform.position, fallFromSky);
		}

		public GameObject SpawnPlayerAt(Player player, Vector2 position, bool fallFromSky)
		{
			Debug.Log($"Spawning player {player.playerIndex}");
			Players.SetActionMaps(Players.PlayerActionMap);
			return player.Spawn(position, fallFromSky);
		}

		public void StopLevel()
		{
			Players.OnAllJoinedPlayersDead -= LoseLevel;
		}

		private void LoseLevel()
		{
			OnGameOver?.Invoke();
			StopLevel();
		}

		public void StartLevel()
		{
			SpawnPlayers(LevelManager.I.RespawnTravelPoint != null ? LevelManager.I.RespawnTravelPoint : defaultTravelPoint);
			nodePositioner.StartGraphPositioning();
		}

		private void SpawnPlayers(TravelPoint travelPoint)
		{
			if (travelPoint == null)
			{
				Debug.LogError("No travel point found");
				return;
			}
			Debug.Log("fall from sky " + travelPoint.fallFromSky);
			foreach (var player in Players.AllJoinedPlayers)
			{
				if (player == null) continue;
				if (player.SpawnedPlayerGO != null) continue;
				SpawnPlayer(player, travelPoint, travelPoint.fallFromSky);
			}
		}
	}
}