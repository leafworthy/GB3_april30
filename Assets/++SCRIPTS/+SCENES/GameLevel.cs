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
		public event Action OnGameOver;
		public event Action<Player> OnPlayerSpawned;
		private CityMaker cityMaker;

		private List<TravelPoint> getSpawnPoints() => FindObjectsByType<TravelPoint>(FindObjectsSortMode.None).ToList();

		private bool hasSpawnPoint(TravelPoint travelPoint) => getSpawnPoints().Contains(travelPoint);

		private void Start()
		{
			Players.I.OnAllJoinedPlayersDead += LoseLevel;

		}

		private void SpawnPlayer(Player player, TravelPoint travelPoint)
		{
			if (!hasSpawnPoint(travelPoint)) travelPoint = defaultTravelPoint;
			Debug.Log($"Spawning player {player.playerIndex}");
			Players.SetActionMaps(Players.PlayerActionMap);
			player.Spawn(travelPoint.transform.position, travelPoint.fallFromSky);
			OnPlayerSpawned?.Invoke(player);
		}

		public GameObject SpawnPlayerFromSky(Player player, Vector2 position)
		{
			Debug.Log($"Spawning player {player.playerIndex}");
			Players.SetActionMaps(Players.PlayerActionMap);
			return player.Spawn(position, true);
		}

		public void StopLevel()
		{
			Players.I.OnAllJoinedPlayersDead -= LoseLevel;
		}

		private void LoseLevel()
		{
			OnGameOver?.Invoke();
			StopLevel();
		}

		public void StartLevel()
		{
			cityMaker = FindFirstObjectByType<CityMaker>();
			cityMaker.GenerateCity();
			defaultTravelPoint = FindFirstObjectByType<TravelPoint>();
			SpawnPlayers(defaultTravelPoint);
			nodePositioner = GetComponent<GraphNodePositioner>();
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
			foreach (var player in Players.I.AllJoinedPlayers)
			{
				if (player == null) continue;
				if (player.SpawnedPlayerGO != null) continue;
				SpawnPlayer(player, travelPoint);
			}
		}


	}
}
