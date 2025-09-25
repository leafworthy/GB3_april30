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

		public Player mainPlayer;

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
		}

		private void Input_OnPlayerJoins(PlayerInput newPlayerInput)
		{
			if (Services.pauseManager.IsPaused) return;
			var joiningPlayer = newPlayerInput.GetComponent<Player>();
			if (joiningPlayer == null) return;
			AddPlayerToJoinedPlayers(newPlayerInput, joiningPlayer);
		}

		private void AddPlayerToJoinedPlayers(PlayerInput newPlayerInput, Player joiningPlayer)
		{
			if (AllJoinedPlayers.Contains(joiningPlayer)) return;

			if (AllJoinedPlayers.Count >= 4) return;

			joiningPlayer.ConnectPlayerToController(newPlayerInput, playerPresets[newPlayerInput.playerIndex], newPlayerInput.playerIndex);
			if(mainPlayer == null) SetMainPlayer(joiningPlayer);
			AllJoinedPlayers.Add(joiningPlayer);
			OnPlayerJoins?.Invoke(joiningPlayer);
			joiningPlayer.OnPlayerDies += Player_PlayerDies;
		}

		private void Player_PlayerDies(Player deadPlayer)
		{
			if (deadPlayer.IsMainPlayer())
			{
				Debug.Log("main character died");
				var nextPlayer = AllJoinedPlayers.FirstOrDefault(t => t != deadPlayer && t.state == Player.State.Alive);
				if (nextPlayer == null)
				{
					Debug.Log("no other players to switch to");
					return;
				}
				if(nextPlayer == mainPlayer)
				{
					Debug.Log("next player is already main player");
					return;
				}
				SetMainPlayer(nextPlayer);
			}

			Debug.Log("OnPlayerDies called");
			OnPlayerDies?.Invoke(deadPlayer);

			if (AllJoinedPlayersAreDead()) OnAllJoinedPlayersDead?.Invoke();
		}

		private void SetMainPlayer(Player nextPlayer)
		{
			mainPlayer = nextPlayer;
			if (mainPlayer == null) return;
			if (mainPlayer.input == null) return;
			mainPlayer.SetIsMainPlayer(true);
		}

		private bool AllJoinedPlayersAreDead()
		{
			var playersAlive = AllJoinedPlayers.Where(t => t.state == Player.State.Alive).ToList();
			return playersAlive.Count <= 0;
		}

		public void SetActionMaps(string actionMap)
		{
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
