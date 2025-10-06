using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace __SCRIPTS
{
	public class PlayerStatsManager : SerializedMonoBehaviour, IService
	{
		private static Dictionary<Player, PlayerStats> playerStats = new();

		public event Action<Player, PlayerStat.StatType, float> OnPlayerStatChange;
		private bool hasStarted;
		private int maxGas = 20;
		private LevelManager _levelManager;
		private LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();
		private Players _playerManager;
		private Players playerManager => _playerManager ?? ServiceLocator.Get<Players>();
		public int MaxGas => maxGas;

		public void StartService()
		{
			levelManager.OnLevelSpawnedPlayerFromLevel += LevelSpawnedPlayersFromLevelOnLevelSpawnedPlayerFromLevelJoins;
		}

		public float GetStatAmount(Player player, PlayerStat.StatType statType)
		{
			GatherPlayerStats();
			var match = playerStats.TryGetValue(player, out var stats);
			if (match) return stats.GetStatValue(statType);

			// If player not found, try to add them (for mid-game joins)
			var playerStatsComponent = player.GetComponent<PlayerStats>();
			if (playerStatsComponent != null)
			{
				playerStats.TryAdd(player, playerStatsComponent);
				return playerStatsComponent.GetStatValue(statType);
			}

			return 0; // Return 0 instead of -999 for better display
		}

		private void GatherPlayerStats()
		{
			if (hasStarted) return;
			playerStats.Clear();
			hasStarted = true;
			foreach (var player in playerManager.AllJoinedPlayers)
			{
				playerStats.Add(player, player.GetComponent<PlayerStats>());
			}
		}

		public void ChangeStat(Player player, PlayerStat.StatType statType, float change)
		{
			var match = playerStats.TryGetValue(player, out var stats);
			if (match)
			{
				stats.ChangeStat(statType, change);
				OnPlayerStatChange?.Invoke(player, statType, stats.GetStatValue(statType));
			}
		}

		private void LevelSpawnedPlayersFromLevelOnLevelSpawnedPlayerFromLevelJoins(Player player)
		{
			var stats = player.GetComponent<PlayerStats>();
			if (stats == null) return;

			playerStats.TryAdd(player, stats);
		}

		public void SetStatAmount(Player owner, PlayerStat.StatType statType, float value)
		{
			if (playerStats.TryGetValue(owner, out var stats))
			{
				stats.SetStatValue(statType, value);
				OnPlayerStatChange?.Invoke(owner, statType, stats.GetStatValue(statType));
			}
		}
	}
}
