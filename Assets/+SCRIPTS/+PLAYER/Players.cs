using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __SCRIPTS
{
	public class Players : MonoBehaviour, IService
	{
		[SerializeField] private PlayerData EnemyPlayerData;
		[SerializeField] public List<PlayerData> playerPresets = new();

		private PlayerInputManager _inputManager;
		private Player _enemyPlayer;
		public Player enemyPlayer => _enemyPlayer ?? CreateEnemyPlayer();
		public List<Player> AllJoinedPlayers = new();
		private PlayerInput testPlayerInput;
		private Player testPlayer;

		// Action maps
		public const string UIActionMap = "UI";
		public const string PlayerActionMap = "PlayerMovement";

		// Events
		public event Action OnAllJoinedPlayersDead;
		public event Action<Player> OnPlayerJoins;
		public event Action<Player> OnPlayerDies;



		public void StartService()
		{
			_inputManager = GetComponent<PlayerInputManager>();
			_inputManager.onPlayerJoined += Input_OnPlayerJoins;

			// Create enemy player
			CreateEnemyPlayer();
			SetActionMaps(UIActionMap);
		}

		private Player CreateEnemyPlayer()
		{
			var enemy = new GameObject("EnemyPlayer");
			_enemyPlayer = enemy.AddComponent<Player>();
			_enemyPlayer.ConnectPlayerToController(null, EnemyPlayerData, 5);
			return _enemyPlayer;
		}

		private void OnDisable()
		{
			foreach (var player in AllJoinedPlayers)
			{
				player.OnPlayerDies -= Player_PlayerDies;
			}

			if (_inputManager != null) _inputManager.onPlayerJoined -= Input_OnPlayerJoins;
		}


		public void ClearAllJoinedPlayers()
		{
			var playersToDestroy = new List<Player>(AllJoinedPlayers);

			foreach (var player in playersToDestroy)
			{
				if (player == null) continue;
					if (player.SpawnedPlayerGO != null) Destroy(player.SpawnedPlayerGO);
					Destroy(player.gameObject);

			}

			AllJoinedPlayers.Clear();
			Debug.Log("clear all players");
		}

		private void Input_OnPlayerJoins(PlayerInput newPlayerInput)
		{
			Debug.Log($"PlayerInput {newPlayerInput.playerIndex} joined the input");
			var joiningPlayer = newPlayerInput.GetComponent<Player>();
			if (joiningPlayer == null) return;
			AddPlayerToJoinedPlayers(newPlayerInput, joiningPlayer);
		}

		private void AddPlayerToJoinedPlayers(PlayerInput newPlayerInput, Player joiningPlayer)
		{
			Debug.Log($"Player {joiningPlayer.name} is trying to join with input {newPlayerInput.playerIndex}");

			if (AllJoinedPlayers.Contains(joiningPlayer))
			{
				Debug.Log($"Player {joiningPlayer.name} is already joined.");
				return;
			}

			if (AllJoinedPlayers.Count >= 4)
			{
				Debug.Log("too many players joined, max is 4");
				return;
			}

			joiningPlayer.ConnectPlayerToController(newPlayerInput, playerPresets[newPlayerInput.playerIndex], newPlayerInput.playerIndex);

			AllJoinedPlayers.Add(joiningPlayer);
			Debug.Log("PLAYERS: player joins" + joiningPlayer.name + " with input " + newPlayerInput.playerIndex);
			OnPlayerJoins?.Invoke(joiningPlayer);
			joiningPlayer.OnPlayerDies += Player_PlayerDies;
		}


		private void Player_PlayerDies(Player deadPlayer)
		{
			OnPlayerDies?.Invoke(deadPlayer);
			Debug.Log("player dies, players left: " + AllJoinedPlayers.Count(t => t.state == Player.State.Alive));
			if (AllJoinedPlayersAreDead()) OnAllJoinedPlayersDead?.Invoke();
		}

		private bool AllJoinedPlayersAreDead()
		{
			var playersAlive = AllJoinedPlayers.Where(t => t.state == Player.State.Alive).ToList();
			Debug.Log(playersAlive.Count <= 0 ? "all players dead" : "players still alive: " + playersAlive.Count);
			return playersAlive.Count <= 0;
		}

		public void SetActionMaps(string actionMap)
		{
			Debug.Log("PLAYERS: SetActionMaps: " + actionMap);
			foreach (var player in AllJoinedPlayers)
			{
				SetActionMap(player, actionMap);
			}
		}

		public static void SetActionMap(Player player, string actionMap)
		{
			if (player == null) return;
			if (player.input == null)
			{
				player.input = player.GetComponent<PlayerInput>();
				return;
			}

			player.input.SwitchCurrentActionMap(actionMap);
		}

	}
}
