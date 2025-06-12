using System;
using System.Collections.Generic;
using System.Linq;
using __SCRIPTS._ZOMBIESPAWN;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __SCRIPTS
{
	public class Players : Singleton<Players>
	{
		[SerializeField] private PlayerData EnemyPlayerData;
		[SerializeField] public List<PlayerData> playerPresets = new();

		private static PlayerInputManager _inputManager;
		private Player _enemyPlayer;

		public static Player EnemyPlayer => I._enemyPlayer;
		public  List<Player> AllJoinedPlayers = new();

		// Action maps
		public const string UIActionMap = "UI";
		public const string PlayerActionMap = "PlayerMovement";

		// Events
		public event Action<Player> OnPlayerGetUpgrades;
		public event Action OnAllJoinedPlayersDead;
		public event Action<Player> OnPlayerJoins;
		public event Action<Player> OnPlayerDies;


		private void OnEnable()
		{
			_inputManager = GetComponent<PlayerInputManager>();
			_inputManager.onPlayerJoined += Input_OnPlayerJoins;


			ZombieWaveManager.OnWaveEnd += PlayersGetUpgrades;

			// Create enemy player
			var enemy = new GameObject("EnemyPlayer");
			_enemyPlayer = enemy.AddComponent<Player>();
			_enemyPlayer.Join(null, EnemyPlayerData, 5);
			SetActionMaps(UIActionMap);
		}

		// Properly clean up event subscriptions
		private void OnDisable()
		{
			foreach (var player in AllJoinedPlayers)
			{
				Players.I.OnPlayerDies -= Player_PlayerDies;
			}

			if (_inputManager != null) _inputManager.onPlayerJoined -= Input_OnPlayerJoins;

			ZombieWaveManager.OnWaveEnd -= PlayersGetUpgrades;
		}

		// Handle upgrades at the end of waves
		private void PlayersGetUpgrades()
		{
			foreach (var player in AllJoinedPlayers)
			{
				OnPlayerGetUpgrades?.Invoke(player);

			}
		}

		public void PlayerOpensShoppe(Player player)
		{
			OnPlayerGetUpgrades?.Invoke(player);

		}

		// Clear joined players (typically used when starting a new game)
		public static void ClearAllJoinedPlayers()
		{
			if (I == null)
			{

				return;
			}


			var playersToDestroy = new List<Player>(I.AllJoinedPlayers);

			foreach (var player in playersToDestroy)
			{
				if (player == null) continue; // Continue instead of return to process all players

				try
				{
					// Properly cleanup player before destroying
					if (player.SpawnedPlayerGO != null)
					{
						Destroy(player.SpawnedPlayerGO);
					}
					Destroy(player.gameObject);
				}
				catch (System.Exception e)
				{

				}
			}

			I.AllJoinedPlayers.Clear();
		}

		// Handle a new player joining
		private void Input_OnPlayerJoins(PlayerInput newPlayerInput)
		{
			if (newPlayerInput == null)
			{

				return;
			}

			var joiningPlayer = newPlayerInput.GetComponent<Player>();
			if (joiningPlayer == null)
			{

				return;
			}

			JoinPlayer(newPlayerInput, joiningPlayer);
		}

		// Join a player to the game
		private void JoinPlayer(PlayerInput newPlayerInput, Player joiningPlayer)
		{
			if (newPlayerInput == null || joiningPlayer == null)
			{

				return;
			}

			// Check for duplicate players
			if (AllJoinedPlayers.Contains(joiningPlayer))
			{

				return;
			}

			// Check player limit
			if (AllJoinedPlayers.Count >= 4)
			{

				return;
			}

			// Validate player index and preset
			int playerIndex = newPlayerInput.playerIndex;
			if (playerIndex < 0 || playerIndex >= playerPresets.Count)
			{

				playerIndex = 0;
			}

			try
			{
				AllJoinedPlayers.Add(joiningPlayer);
				joiningPlayer.Join(newPlayerInput, playerPresets[playerIndex], playerIndex);

				// Subscribe to player death events with error handling
				joiningPlayer.OnPlayerDies += Player_PlayerDies;

				// Notify other systems
				OnPlayerJoins?.Invoke(joiningPlayer);


			}
			catch (System.Exception e)
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
		private static bool AllJoinedPlayersAreDead()
		{
			var playersAlive = I.AllJoinedPlayers.Where(t => t.state == Player.State.Alive).ToList();
			return playersAlive.Count <= 0;
		}

		// Set action maps for all players
		public static void SetActionMaps(string actionMap)
		{
			foreach (var player in I.AllJoinedPlayers) SetActionMap(player, actionMap);
		}

		// Set action map for a specific player
		public static void SetActionMap(Player player, string actionMap)
		{
			if (player == null) return;
			if(player.input == null)
			{
				player.input = player.GetComponent<PlayerInput>();
				return;
			}
			player.input.SwitchCurrentActionMap(actionMap);
		}
	}
}
