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
		public event Action<Player> OnPlayerGetUpgrades;
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
			_enemyPlayer.Join(null, EnemyPlayerData, 5);
			return _enemyPlayer;
		}

		// Properly clean up event subscriptions
		private void OnDisable()
		{
			foreach (var player in AllJoinedPlayers)
			{
				OnPlayerDies -= Player_PlayerDies;
			}

			if (_inputManager != null) _inputManager.onPlayerJoined -= Input_OnPlayerJoins;
		}



		public void PlayerOpensShoppe(Player player)
		{
			OnPlayerGetUpgrades?.Invoke(player);
		}

		// Clear joined players (typically used when starting a new game)
		public void ClearAllJoinedPlayers()
		{
			var playersToDestroy = new List<Player>(AllJoinedPlayers);

			foreach (var player in playersToDestroy)
			{
				if (player == null) continue; // Continue instead of return to process all players

				try
				{
					// Properly cleanup player before destroying
					if (player.SpawnedPlayerGO != null) Destroy(player.SpawnedPlayerGO);
					Destroy(player.gameObject);
				}
				catch (Exception e)
				{
				}
			}

			AllJoinedPlayers.Clear();
		}

		// Handle a new player joining
		private void Input_OnPlayerJoins(PlayerInput newPlayerInput)
		{
			Debug.Log($"PlayerInput {newPlayerInput.playerIndex} joined the game");
			if (newPlayerInput == null) return;

			var joiningPlayer = newPlayerInput.GetComponent<Player>();
			if (joiningPlayer == null) return;

			JoinPlayer(newPlayerInput, joiningPlayer);
		}

		// Join a player to the game
		private void JoinPlayer(PlayerInput newPlayerInput, Player joiningPlayer)
		{
			Debug.Log($"Player {joiningPlayer.name} is trying to join with input {newPlayerInput.playerIndex}");
			if (newPlayerInput == null || joiningPlayer == null)
			{
				Debug.Log("Invalid PlayerInput or Player component.");
				return;
			}

			// Check for duplicate players
			if (AllJoinedPlayers.Contains(joiningPlayer))
			{
				Debug.Log($"Player {joiningPlayer.name} is already joined.");
				return;
			}

			// Check player limit
			if (AllJoinedPlayers.Count >= 4) return;

			// Validate player index and preset
			var playerIndex = newPlayerInput.playerIndex;
			if (playerIndex < 0 || playerIndex >= playerPresets.Count)
			{
				Debug.Log("Invalid player index: " + playerIndex + ". Resetting to 0.");
				playerIndex = 0;
			}

			try
			{
				AllJoinedPlayers.Add(joiningPlayer);
				joiningPlayer.Join(newPlayerInput, playerPresets[playerIndex], playerIndex);

				// Subscribe to player death events with error handling
				joiningPlayer.OnPlayerDies += Player_PlayerDies;

				Debug.Log("on player joins " + joiningPlayer.name + " with input " + newPlayerInput.playerIndex);
				// Notify other systems
				OnPlayerJoins?.Invoke(joiningPlayer);
			}
			catch (Exception e)
			{
				// Remove from list if joining failed
				AllJoinedPlayers.Remove(joiningPlayer);
			}
		}

		// Handle player death
		private void Player_PlayerDies(Player deadPlayer)
		{
			OnPlayerDies?.Invoke(deadPlayer);
			if (AllJoinedPlayersAreDead()) OnAllJoinedPlayersDead?.Invoke();
		}

		// Check if all players are dead
		private bool AllJoinedPlayersAreDead()
		{
			var playersAlive = AllJoinedPlayers.Where(t => t.state == Player.State.Alive).ToList();
			return playersAlive.Count <= 0;
		}

		// Set action maps for all players
		public void SetActionMaps(string actionMap)
		{
			foreach (var player in AllJoinedPlayers)
			{
				SetActionMap(player, actionMap);
			}
		}

		// Set action map for a specific player
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

		public static void AddTestPlayer(Player testPlayer)
		{
		}
	}
}
