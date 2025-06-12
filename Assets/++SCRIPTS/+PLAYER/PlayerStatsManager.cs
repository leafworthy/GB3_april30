using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerStatsManager : Singleton<PlayerStatsManager>
	{
		private static Dictionary<Player, PlayerStats> playerStats = new();

		public event Action <Player, PlayerStat.StatType , float> OnPlayerStatChange;
		private bool hasStarted;
		private int maxGas = 20;
		public int MaxGas
		{
			get { return maxGas; }
		}

		private void Start()
		{
			LevelManager.I.OnPlayerSpawned += Players_OnPlayerJoins;
		}

		public float GetStatAmount(Player player,PlayerStat.StatType statType)
		{
			GatherPlayerStats();
			var match = playerStats.TryGetValue(player, out var stats);
			if (match)
			{
				return stats.GetStatValue(statType);
			}

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
			foreach (var player in Players.I.AllJoinedPlayers)
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
			else
			{

			}
		}


		private void Players_OnPlayerJoins(Player player)
		{
			var stats = player.GetComponent<PlayerStats>();
			if(stats == null)
			{

				return;
			}

			playerStats.TryAdd(player, stats);

		}

		public void SetStatAmount(Player owner, PlayerStat.StatType statType, float value)
		{
			if (playerStats.TryGetValue(owner, out var stats))
			{
				stats.SetStatValue(statType, value);
				OnPlayerStatChange?.Invoke(owner, statType, stats.GetStatValue(statType));
			}
			else
			{

			}
		}
	}
}
