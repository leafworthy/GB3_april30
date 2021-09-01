using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PLAYERS : Singleton<PLAYERS>
{
	private static List<Player> players = new List<Player>();
	public static event Action OnAllPlayersDead;
	private static Player EnemyPlayer;

	private void Start()
	{
		Init();
	}

	private  void Init()
	{
		players.Clear();
		var playerAssets = Resources.LoadAll<PlayerData>("PlayerData").ToList();
		foreach (var playerData in playerAssets)
		{
			var newPlayer = gameObject.AddComponent<Player>();
			newPlayer.data = playerData;
			AddPlayer(newPlayer);
		}
	}

	private static void JoinPlayer(Player player)
	{
		if (player.hasJoined) return;
		if (!player.data.isPlayer)
		{
			EnemyPlayer = player;
		}
		player.hasJoined = true;
		OnPlayerJoin?.Invoke(player);
	}

	private static void AddPlayer(Player newPlayer)
	{
			newPlayer.OnDead += PlayerDies;
			newPlayer.PressA += JoinPlayer;
			newPlayer.PressB += JoinPlayer;
			newPlayer.PressPause += JoinPlayer;
			if (!players.Contains(newPlayer)) players.Add(newPlayer);
	}

	private static void PlayerDies(Player deadPlayer)
	{
		deadPlayer.OnDead -= PlayerDies;
		deadPlayer.isDead = true;
		CheckIfAllPlayersDead();
	}

	private static void CheckIfAllPlayersDead()
	{
		if (GetJoinedPlayers().Count(t => !t.isDead) > 0) return;
		OnAllPlayersDead?.Invoke();
	}



	public static event Action<Player> OnPlayerJoin;

	public static List<Player> GetJoinedPlayers()
	{
		return players.Where(t => t.hasJoined).ToList();
	}

	public static List<Player> GetJoiningPlayers()
	{
		return players.Where(t => t.isJoining).ToList();
	}

	public static List<Player> GetAllPlayers()
	{
		return players;
	}

	public static Player GetEnemyPlayer()
	{
		return EnemyPlayer;
	}

	public static List<Player> GetSpawnedPlayers()
	{

		return players.Where(t => t.hasSpawned).ToList();
	}
}
