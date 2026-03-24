using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace __SCRIPTS
{
	public class PlayerStatsManager : SerializedMonoBehaviour, IService
	{
		Dictionary<Player, PlayerStats> playerStats = new();
		public int MaxRescues { get; } = 20;

		public void StartService()
		{
			Services.levelManager.OnLevelSpawnedPlayerFromLevel += LevelSpawnedPlayersFromLevelOnLevelSpawnedPlayerFromLevelJoins;
		}

		void OnDisable()
		{
			Services.levelManager.OnLevelSpawnedPlayerFromLevel -= LevelSpawnedPlayersFromLevelOnLevelSpawnedPlayerFromLevelJoins;
			playerStats.Clear();
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

			return 0;
		}

		void GatherPlayerStats()
		{
			playerStats.Clear();
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				playerStats.Add(player, player.GetComponent<PlayerStats>());
			}
		}

		public void ChangeStat(Player player, PlayerStat.StatType statType, float change)
		{
			var match = playerStats.TryGetValue(player, out var stats);
			if (match) stats.ChangeStat(statType, change);
		}

		void LevelSpawnedPlayersFromLevelOnLevelSpawnedPlayerFromLevelJoins(Player player)
		{
			var stats = player.GetComponent<PlayerStats>();
			if (stats == null) return;

			playerStats.TryAdd(player, stats);
		}

		public void SetStatAmount(Player owner, PlayerStat.StatType statType, float value)
		{
			if (!playerStats.TryGetValue(owner, out var stats)) return;
			stats.SetStatValue(statType, value);
		}
	}
}
