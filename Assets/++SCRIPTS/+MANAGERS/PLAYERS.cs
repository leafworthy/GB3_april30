using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PLAYERS : Singleton<PLAYERS>
{
	private static List<Player> players = new List<Player>();
	public static event Action OnAllPlayersDead;
	public static event Action<Player> OnPlayerDead;
	private static Player enemyPlayer;
	private static List<GameObject> playerGOs = new List<GameObject>();



	public static void AddPlayer(Player newPlayer)
	{
		Debug.Log("player added");
		if (newPlayer.isPlayer)
		{
			newPlayer.OnDead += PlayerDies;
			if (!players.Contains(newPlayer)) players.Add(newPlayer);
		}
		else
			enemyPlayer = newPlayer;
	}

	private static void PlayerDies(Player deadPlayer)
	{
		Debug.Log("player dead");
		deadPlayer.OnDead -= PlayerDies;
		OnPlayerDead?.Invoke(deadPlayer);
		//players.Remove(deadPlayer);
		deadPlayer.isDead = true;
		if (GetNumberOfLivingPlayers() > 0) return;
		Debug.Log("all dead");
		OnAllPlayersDead?.Invoke();
	}

	private static int GetNumberOfLivingPlayers()
	{
		//if (!PlayersHaveBeenFound()) return 0;
		var count = players.Count(t => !t.isDead);
		Debug.Log(count + " player count");
		return count;
	}

	public static Player GetEnemyPlayer()
	{
		return enemyPlayer;
	}

	public static List<GameObject> GetPlayerGOs()
	{
		playerGOs.Clear();
		foreach (var player in players.Where(t => !t.isDead))
		{
			playerGOs.Add(player.SpawnedPlayerGO);
		}
		return playerGOs;
	}
}
