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
	public static void AddPlayer(Player newPlayer)
	{
		Debug.Log("player added");
		if (newPlayer.isPlayer)
		{
			players.Add(newPlayer);
			newPlayer.OnDead += PlayerDies;
		}
		else
		{
			enemyPlayer = newPlayer;
		}
	}

	private static void PlayerDies(Player deadPlayer)
	{
		Debug.Log("player dead");
		deadPlayer.OnDead -= PlayerDies;
		OnPlayerDead?.Invoke(deadPlayer);
		players.Remove(deadPlayer);
		if (GetNumberOfLivingPlayers() > 0) return;
		Debug.Log("all dead");
		OnAllPlayersDead?.Invoke();
	}

	private static int GetNumberOfLivingPlayers()
	{
		//if (!PlayersHaveBeenFound()) return 0;
		Debug.Log(players.Count + "player count");
		return players.Count(t => !t.IsDead());
	}

	public static Player GetEnemyPlayer()
	{
		return enemyPlayer;
	}
}
