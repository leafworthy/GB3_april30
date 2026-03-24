using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __SCRIPTS
{
	public class Players : MonoBehaviour, IService
	{
		[SerializeField] PlayerData EnemyPlayerData;
		[SerializeField] PlayerData NPCPlayerData;
		[SerializeField] public List<PlayerData> playerPresets = new();

		PlayerInputManager _inputManager;
		public Player enemyPlayer => _enemyPlayer ??= CreateEnemyPlayer();
		Player _enemyPlayer;
		public Player NPCPlayer => _NPCPlayer ??= CreateNPCPlayer();
		Player _NPCPlayer;
		public List<Player> AllJoinedPlayers = new();


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

			SetActionMaps(UIActionMap);
		}

		Player CreateEnemyPlayer()
		{
			var enemy = new GameObject("EnemyPlayer");
			_enemyPlayer = enemy.AddComponent<Player>();
			_enemyPlayer.ConnectPlayerToController(null, EnemyPlayerData, 5);
			return _enemyPlayer;
		}

		Player CreateNPCPlayer()
		{
			var NPC = new GameObject("NPCPlayer");
			_NPCPlayer = NPC.AddComponent<Player>();
			_NPCPlayer.ConnectPlayerToController(null, NPCPlayerData, 6);
			return _NPCPlayer;
		}

		void OnDisable()
		{
			foreach (var player in AllJoinedPlayers)
			{
				player.OnPlayerDies -= Player_PlayerDies;
			}

			if (_inputManager != null) _inputManager.onPlayerJoined -= Input_OnPlayerJoins;
			 OnAllJoinedPlayersDead = null;
			 OnPlayerJoins = null;
			 OnPlayerDies = null;
			 mainPlayer = null;
			 _enemyPlayer = null;
			 _NPCPlayer = null;
			 AllJoinedPlayers.Clear();
			 playerPresets.Clear();
		}

		public void ClearAllJoinedPlayers()
		{
			var playersToDestroy = new List<Player>(AllJoinedPlayers);

			foreach (var player in playersToDestroy)
			{
				if (player == null) continue;
				player.OnPlayerDies -= Player_PlayerDies;
				if (player.SpawnedPlayerGO != null) Destroy(player.SpawnedPlayerGO);
				Destroy(player.gameObject);
			}

			AllJoinedPlayers.Clear();
		}

		void Input_OnPlayerJoins(PlayerInput newPlayerInput)
		{
			if (Services.pauseManager.IsPaused) return;
			var joiningPlayer = newPlayerInput.GetComponent<Player>();
			if (joiningPlayer == null) return;
			AddPlayerToJoinedPlayers(newPlayerInput, joiningPlayer);
		}

		void AddPlayerToJoinedPlayers(PlayerInput newPlayerInput, Player joiningPlayer)
		{
			if (AllJoinedPlayers.Contains(joiningPlayer))
			{
				Debug.LogWarning("already joined player tried to join again", this);
				return;
			}

			if (AllJoinedPlayers.Count >= 4)
			{
				Debug.Log( "4 players already joined", this);
				return;
			}

			joiningPlayer.ConnectPlayerToController(newPlayerInput, playerPresets[newPlayerInput.playerIndex], newPlayerInput.playerIndex);
			if (mainPlayer == null) SetMainPlayer(joiningPlayer);
			AllJoinedPlayers.Add(joiningPlayer);
			OnPlayerJoins?.Invoke(joiningPlayer);
			joiningPlayer.OnPlayerDies += Player_PlayerDies;
		}

		void Player_PlayerDies(Player deadPlayer, bool forRespawn = false)
		{
			PlayerDead(deadPlayer);
			deadPlayer.OnPlayerDies -= Player_PlayerDies;
			if (forRespawn) return;
			if (AllJoinedPlayersAreDead())
			{
				Debug.Log("all players dead");
				OnAllJoinedPlayersDead?.Invoke();
			}
		}

		void PlayerDead(Player deadPlayer)
		{
			Debug.Log("player dead");
			if (deadPlayer.IsMainPlayer())
			{
				Debug.Log("main character died");
				var nextPlayer = AllJoinedPlayers.FirstOrDefault(t => t != deadPlayer && t.state == Player.State.Alive);
				if (nextPlayer == null)
				{
					Debug.Log("no other players to switch to");
					return;
				}

				if (nextPlayer == mainPlayer)
				{
					Debug.Log("next player is already main player");
					return;
				}

				SetMainPlayer(nextPlayer);
			}

			OnPlayerDies?.Invoke(deadPlayer);
		}

		void SetMainPlayer(Player nextPlayer)
		{
			mainPlayer = nextPlayer;
			if (mainPlayer == null) return;
			if (mainPlayer.input == null) return;
			mainPlayer.SetIsMainPlayer(true);
		}

		bool AllJoinedPlayersAreDead()
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
