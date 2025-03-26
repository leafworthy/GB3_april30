using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Players : Singleton<Players>
{
	[SerializeField] private PlayerData EnemyPlayerData;
	[SerializeField] public List<PlayerData> playerPresets = new();

	private static PlayerInputManager _inputManager;
	private Player _enemyPlayer;

	public static Player EnemyPlayer => I._enemyPlayer;
	public static readonly List<Player> AllJoinedPlayers = new();

	// Action maps
	public static readonly string UIActionMap = "UI";
	public static readonly string PlayerActionMap = "PlayerMovement";

	// Events
	public static event Action<Player> OnPlayerGetUpgrades;
	public static event Action OnAllJoinedPlayersDead;
	public static event Action<Player> OnPlayerJoins;

	private void Start()
	{
		_inputManager = GetComponent<PlayerInputManager>();
		_inputManager.onPlayerJoined += Input_OnPlayerJoins;

		Player.OnPlayerDies += Player_PlayerDies;
		ZombieWaveManager.OnWaveEnd += PlayersGetUpgrades;

		// Create enemy player
		var enemy = new GameObject("EnemyPlayer");
		_enemyPlayer = enemy.AddComponent<Player>();
		_enemyPlayer.Join(null, EnemyPlayerData, 5);
		SetActionMaps(UIActionMap);

		
	}

	// Properly clean up event subscriptions
	private void OnDestroy()
	{
		Player.OnPlayerDies -= Player_PlayerDies;

		if (_inputManager != null) _inputManager.onPlayerJoined -= Input_OnPlayerJoins;

		ZombieWaveManager.OnWaveEnd -= PlayersGetUpgrades;
	}

	// Handle upgrades at the end of waves
	private void PlayersGetUpgrades()
	{
		foreach (var player in AllJoinedPlayers)
		{
			OnPlayerGetUpgrades?.Invoke(player);
			Debug.Log("players get upgrades!");
		}
	}

	public static void PlayerOpensShoppe(Player player)
	{
			OnPlayerGetUpgrades?.Invoke(player);
			Debug.Log("player upgrading");
	}

	// Clear joined players (typically used when starting a new game)
	public static void ClearAllJoinedPlayers()
	{
		Debug.Log("cleared all players");
		foreach (var player in AllJoinedPlayers)
		{
			player.gameObject.SetActive(false);
		}

		AllJoinedPlayers.Clear();
	}

	// Handle a new player joining
	private void Input_OnPlayerJoins(PlayerInput newPlayerInput)
	{
		var joiningPlayer = newPlayerInput.GetComponent<Player>();
		JoinPlayer(newPlayerInput, joiningPlayer);
	}

	// Join a player to the game
	private void JoinPlayer(PlayerInput newPlayerInput, Player joiningPlayer)
	{
		if (AllJoinedPlayers.Contains(joiningPlayer)) return;
		AllJoinedPlayers.Add(joiningPlayer);
		joiningPlayer.Join(newPlayerInput, playerPresets[newPlayerInput.playerIndex], newPlayerInput.playerIndex);
		OnPlayerJoins?.Invoke(joiningPlayer);
		//Debug.Log("PLAYER" + newPlayerInput.name + newPlayerInput.playerIndex + " JOINS FROM INPUT MANAGER");
	}

	// Handle player death
	private static void Player_PlayerDies(Player deadPlayer)
	{
		//Debug.Log("PLAYER" + deadPlayer.name + deadPlayer.playerIndex+" has died");

		if (AllJoinedPlayersAreDead()) OnAllJoinedPlayersDead?.Invoke();
	}

	// Check if all players are dead
	private static bool AllJoinedPlayersAreDead()
	{
		var playersAlive = AllJoinedPlayers.Where(t => t.state == Player.State.Alive).ToList();
		return playersAlive.Count <= 0;
	}

	// Set action maps for all players
	public static void SetActionMaps(string actionMap)
	{
		foreach (var player in AllJoinedPlayers)
		{
			SetActionMap(player, actionMap);
		}
	}

	// Set action map for a specific player
	public static void SetActionMap(Player player, string actionMap)
	{
		player.input.SwitchCurrentActionMap(actionMap);
	}
}