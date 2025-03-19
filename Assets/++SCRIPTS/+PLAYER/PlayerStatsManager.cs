using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : Singleton<PlayerStatsManager>
{
	private Dictionary<Player, PlayerStats> playerStats = new();

	public event Action <Player, PlayerStat.StatType , float> OnPlayerStatChange;
	private bool hasStarted;
	private void OnEnable()
	{
		Players.OnPlayerJoins += Players_OnPlayerJoins;
		Player.OnPlayerDies += Player_PlayerDies;
	}

	public float GetStatAmount(Player player,PlayerStat.StatType statType)
	{
		UpdatePlayerStats();
		var match = playerStats.TryGetValue(player, out var stats);
		if (match)
		{
			return stats.GetStatValue(statType);
		}

		return -999;
	}

	private void UpdatePlayerStats()
	{
		if (hasStarted) return;
		playerStats.Clear();
		hasStarted = true;
		foreach (var player in Players.AllJoinedPlayers)
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
			Debug.Log("Player not found in PlayerStatsManager");
		}
	}
	private void Player_PlayerDies(Player player)
	{
		if (playerStats.TryGetValue(player, out var stats))
		{
			stats.ResetStats();
		}
		else
		{
			Debug.LogError("Player not found in PlayerStatsManager");
		}
	}

	private void Players_OnPlayerJoins(Player player)
	{
		var stats = player.GetComponent<PlayerStats>();
		if(stats == null)
		{
			Debug.LogError("Player does not have PlayerStats component");
			return;
		}

		playerStats.TryAdd(player, stats);
		
	}

}