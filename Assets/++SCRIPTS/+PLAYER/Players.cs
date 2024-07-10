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
	public static string UIActionMap = "UI";
	public static string PlayerActionMap = "PlayerMovement";

	public static event Action OnAllPlayersDead;
	public static event Action<Player> OnPlayerJoins;
	public static event Action<Player> OnPlayerJoinsInGame;


	public static void SetActionMaps(string actionMap)
	{
		foreach (var player in AllJoinedPlayers)
		{
			Debug.Log("Setting Action Map to " + actionMap + " for player " + player.playerIndex);
			SetActionMap(player, actionMap);
		}
		
	}
	
	private static void SetActionMap(Player player, string actionMap)
	{
		player.input.SwitchCurrentActionMap(actionMap);
	}
	private void Start()
	{
		_inputManager = GetComponent<PlayerInputManager>();
		_inputManager.onPlayerJoined += Input_OnPlayerJoins;
		
		Player.OnPlayerDies += Player_PlayerDies;

		
		
		_enemyPlayer = gameObject.AddComponent<Player>();
		_enemyPlayer.Join(null, EnemyPlayerData, 5);
		SetActionMaps(UIActionMap);
	}

	private void GameEvents_OnReturnToMainMenu()
	{
		ClearAllPlayers();
	}

	public static void ClearAllPlayers()
	{
		foreach (var player in AllJoinedPlayers)
		{
			player.gameObject.SetActive(false);
		}

		AllJoinedPlayers.Clear();
		Debug.Log("Main Menu - All Players Cleared");
	}

	private void Input_OnPlayerJoins(PlayerInput newPlayerInput)
	{
		var joiningPlayer = newPlayerInput.GetComponent<Player>();
		JoinPlayer(newPlayerInput, joiningPlayer);
	}


	private void JoinPlayer(PlayerInput newPlayerInput, Player joiningPlayer)
	{
		if (AllJoinedPlayers.Contains(joiningPlayer)) return;
		AllJoinedPlayers.Add(joiningPlayer);
		joiningPlayer.Join(newPlayerInput, playerPresets[newPlayerInput.playerIndex], newPlayerInput.playerIndex);
		OnPlayerJoins?.Invoke(joiningPlayer);
		Debug.Log("PLAYER" + newPlayerInput.name + newPlayerInput.playerIndex + " JOINS FROM INPUT MANAGER");
	}

	private static void Player_PlayerDies(Player deadPlayer)
	{
		Debug.Log("PLAYER" + deadPlayer.name + deadPlayer.playerIndex+" has died");
		if (AllPlayersAreDead()) OnAllPlayersDead?.Invoke();
	}

	private static bool AllPlayersAreDead()
	{
		var playersAlive = AllJoinedPlayers.Where(t => t.state == Player.State.Alive).ToList();
		if (playersAlive.Count > 0)
		{
			Debug.Log("players still alive: " + playersAlive);
			return false;
		}

		Debug.Log("all players are dead");
		return true;
	}

	

	public static List<Player> GetPlayersWhoSelectedACharacter()
	{
		return AllJoinedPlayers.Where(t => t.state == Player.State.Selected).ToList();
	}

	
}