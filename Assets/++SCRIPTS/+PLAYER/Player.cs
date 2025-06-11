using System;
using System.Collections.Generic;
using __SCRIPTS.HUD_Displays;
using __SCRIPTS.UpgradeS;
using GangstaBean.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __SCRIPTS
{


	public class Player : MonoBehaviour
	{
		public enum State
		{
			Unjoined,
			SelectingCharacter,
			Selected,
			Alive,
			Dead
		}

		public State state;
		private PlayerData data;

		public GameObject SpawnedPlayerGO;
		public Life spawnedPlayerDefence;

		public PlayerController Controller;
		public PlayerInput input;
		public bool isUsingMouse;
		private AimAbility aimAbility;

		public CharacterButton CurrentButton;
		public int buttonIndex;

		//SET DURING CHARACTER SELECT
		public Character CurrentCharacter;
		public Color playerColor;

		private PlayerSayer sayer;

		public int playerIndex;
		public bool hasKey;
		public List<PlayerInteractable> interactables = new();
		private PlayerInteractable selectedInteractable;
		public event Action<Player> OnPlayerDies;
		public event Action<Player> OnPlayerLeavesUpgradeSetupMenu;

		private PlayerUpgrades playerUpgrades;

		public bool IsPlayer() => data.isPlayer;

		private void Start()
		{
			playerUpgrades = GetComponent<PlayerUpgrades>();
			gameObject.transform.SetParent(GameManager.I.gameObject.transform);
		}

		private void FixedUpdate()
		{
			if (SpawnedPlayerGO != null)
				SelectClosestInteractable();
		}

		public void AddInteractable(PlayerInteractable interactable)
		{
			if (interactable == null) return;
			if (interactables.Contains(interactable)) return;
			interactables.Add(interactable);
			SelectClosestInteractable();
		}

		private void SelectClosestInteractable()
		{
			if (interactables.Count == 0)
			{
				selectedInteractable = null;
				return;
			}

			var closest = interactables[0];
			if (closest == null) return;
			var closestDistance = Vector2.Distance(closest.GetInteractionPosition(), GetAimPosition());
			foreach (var interactable in interactables)
			{
				var distance = Vector2.Distance(interactable.GetInteractionPosition(), GetAimPosition());

				if (!(distance < closestDistance)) continue;
				closest = interactable;
				closestDistance = distance;
			}

			if (closest == selectedInteractable)
			{
				if (selectedInteractable.isSelected)
					return;
			}

			if (selectedInteractable != null) selectedInteractable.Deselect(this);
			selectedInteractable = closest;
			selectedInteractable.Select(this);
		}

		private Vector2 GetAimPosition()
		{
			if (aimAbility == null) aimAbility = SpawnedPlayerGO.GetComponentInChildren<AimAbility>();
			return aimAbility.GetAimPoint();
		}

		public void RemoveInteractable(PlayerInteractable interactable)
		{
			if (interactable == null) return;
			if (interactable == selectedInteractable)
			{
				interactable.Deselect(this);
				selectedInteractable = null;
			}

			interactables.Remove(interactable);
			SelectClosestInteractable();
		}

		// Method to handle player death
		public void OnPlayerDied(Player player)
		{
			state = State.Dead;
			OnPlayerDies?.Invoke(this);
			PlayerStatsManager.I.SetStatAmount(player, PlayerStat.StatType.TimeSurvived, LevelManager.I.GetCurrentLevelTimeElapsed());
		}


		public GameObject Spawn(Vector2 position, bool fallFromSky)
		{
			Debug.Log("Spawning player at " + position);
			state = State.Alive;

			// Create the character instance
			var spawnedPlayerGO = Instantiate(GetPrefabFromCharacter(this));
			spawnedPlayerGO.transform.position = position;

			SetSpawnedPlayerGO(spawnedPlayerGO);

			var animations = spawnedPlayerGO.GetComponentInChildren<Animations>();
			if (animations != null)
			{
				animations.SetBool(Animations.IsFallingFromSky, fallFromSky);
			}
			var jumpController = spawnedPlayerGO.GetComponentInChildren<JumpController>();
			if (jumpController != null)
			{
				jumpController.SetFallFromSky(fallFromSky);
			}

			// Apply any upgrades
			if (playerUpgrades != null) playerUpgrades.ApplyUpgrades(this);

			// Notify that player has spawned
			return spawnedPlayerGO;
		}

		private void SetSpawnedPlayerGO(GameObject newGO)
		{
			if (newGO == null)
			{
				Debug.LogError($"Player.SetSpawnedPlayerGO: newGO is null for player {playerIndex}");
				return;
			}

			SpawnedPlayerGO = newGO;
			
			// Safe Life component setup
			spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<Life>();
			if (spawnedPlayerDefence == null)
			{
				Debug.LogError($"Player.SetSpawnedPlayerGO: Life component missing on spawned player {playerIndex}");
				return;
			}
			
			try
			{
				spawnedPlayerDefence.OnDead += OnPlayerDied;
			}
			catch (System.Exception e)
			{
				Debug.LogError($"Player.SetSpawnedPlayerGO: Failed to subscribe to OnDead event for player {playerIndex}: {e.Message}");
			}

			// Safe component initialization with error handling
			var needPlayerComponents = newGO.GetComponents<INeedPlayer>();
			foreach (var component in needPlayerComponents)
			{
				if (component == null) continue;
				
				try
				{
					component.SetPlayer(this);
					Debug.Log($"Set player {playerIndex} to {component.GetType().Name}");
				}
				catch (System.Exception e)
				{
					Debug.LogError($"Player.SetSpawnedPlayerGO: Failed to set player on component {component.GetType().Name} for player {playerIndex}: {e.Message}");
				}
			}

			// Safe PlayerSayer setup
			try
			{
				sayer = SpawnedPlayerGO.GetComponentInChildren<PlayerSayer>();
				if (sayer == null)
				{
					Debug.LogWarning($"Player.SetSpawnedPlayerGO: PlayerSayer component not found for player {playerIndex}");
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"Player.SetSpawnedPlayerGO: Failed to get PlayerSayer for player {playerIndex}: {e.Message}");
			}
		}

		private static GameObject GetPrefabFromCharacter(Player player)
		{
			switch (player.CurrentCharacter)
			{
				case Character.Karrot:
					return ASSETS.Players.GangstaBeanPlayerPrefab;
				case Character.Bean:
					return ASSETS.Players.GangstaBeanPlayerPrefab;
				case Character.Brock:
					return ASSETS.Players.BrockLeePlayerPrefab;
				case Character.Tmato:
					return ASSETS.Players.TMatoPlayerPrefab;
			}

			return null;
		}

		public void Say(string message, float sayTimeInSeconds = 3)
		{
			if (sayer == null) sayer = SpawnedPlayerGO.GetComponentInChildren<PlayerSayer>();
			sayer.Say(message, sayTimeInSeconds);
		}

		public void Join(PlayerInput playerInput, PlayerData playerData, int index)
		{
			if (playerData == null)
			{
				Debug.LogError("Player.Join: playerData is null");
				return;
			}

			data = playerData;
			
			// Handle null playerInput case
			if (playerInput == null)
			{
				if (!data.isPlayer) return;
				playerIndex = index; // Use provided index as fallback
				isUsingMouse = false; // Default to controller input
			}
			else
			{
				playerIndex = playerInput.playerIndex;
				input = playerInput;
				
				// Safe device detection with null checks
				try
				{
					isUsingMouse = (input.GetDevice<Mouse>() != null || input.GetDevice<Keyboard>() != null);
				}
				catch (System.Exception e)
				{
					Debug.LogWarning($"Player.Join: Failed to detect input device for player {playerIndex}: {e.Message}");
					isUsingMouse = false; // Default to controller
				}
			}

			playerColor = data.playerColor;

			// Safe controller initialization
			Controller = GetComponent<PlayerController>();
			if (Controller == null)
			{
				Debug.LogError($"Player.Join: PlayerController component missing on player {playerIndex}");
				return;
			}

			try
			{
				Controller.InitializeAndLinkToPlayer(this);
			}
			catch (System.Exception e)
			{
				Debug.LogError($"Player.Join: Failed to initialize controller for player {playerIndex}: {e.Message}");
				return;
			}

			SetState(State.SelectingCharacter);
		}

		public void SetState(State newState)
		{
			state = newState;
		}

		public void StopSaying()
		{
			sayer.StopSaying();
		}

		public int GetStartingCash() => data.startingCash;

		public int GetStartingGas() => data.startingGas;

		public void GainKey()
		{
			hasKey = true;
		}

		public bool HasMoreMoneyThan(int amount) => PlayerStatsManager.I.GetStatAmount(this, PlayerStat.StatType.TotalCash) >= amount;

		public void SpendMoney(int amount)
		{
			PlayerStatsManager.I.ChangeStat(this, PlayerStat.StatType.TotalCash, -amount);
		}

		public bool isDead() => spawnedPlayerDefence.IsDead();

		public void LeaveUpgradeSetupMenu()
		{
			OnPlayerLeavesUpgradeSetupMenu?.Invoke(this);
		}
	}
}
