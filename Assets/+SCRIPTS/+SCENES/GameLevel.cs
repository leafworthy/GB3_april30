using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameLevel : ServiceUser
	{
		public TravelPoint defaultTravelPoint => FindFirstObjectByType<TravelPoint>();
		public SceneDefinition scene;
		public event Action OnGameOver;
		public event Action<Player> OnPlayerSpawned;
		private CityMaker cityMaker;



		private void Start()
		{
			playerManager.OnAllJoinedPlayersDead += LoseLevel;
		}

		public void SpawnPlayer(Player player, TravelPoint travelPoint = null)
		{
			if (travelPoint == null) travelPoint = defaultTravelPoint;

			playerManager.SetActionMaps(Players.PlayerActionMap);
			player.Spawn(travelPoint.transform.position, travelPoint.fallFromSky);
			OnPlayerSpawned?.Invoke(player);
		}





		public void StopLevel()
		{
			playerManager.OnAllJoinedPlayersDead -= LoseLevel;
		}

		private void LoseLevel()
		{
			OnGameOver?.Invoke();
		}

		public void StartLevel()
		{

			SpawnPlayers(defaultTravelPoint);
		}

		private void SpawnPlayers(TravelPoint travelPoint)
		{
			if (travelPoint == null) return;

			foreach (var player in playerManager.AllJoinedPlayers)
			{
				if (player == null) continue;
				if (player.SpawnedPlayerGO != null) continue;
				SpawnPlayer(player, travelPoint);
			}
		}
	}
}
