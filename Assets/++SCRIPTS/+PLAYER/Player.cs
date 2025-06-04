using System;
using System.Collections.Generic;
using GangstaBean.UI.HUD;
using GangstaBean.Player.Upgrades;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GangstaBean.Player
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
		}

		private void LevelStopLevelCleanUp(GameLevel gameLevel)
		{
			// Otherwise clean up normally
			state = State.SelectingCharacter;
			CurrentButton = null;
			SpawnedPlayerGO = null;
			LevelManager.I.OnStopLevel -= LevelStopLevelCleanUp;
			if (spawnedPlayerDefence != null) spawnedPlayerDefence.OnDead -= OnPlayerDied;
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

			// Subscribe to level stop event for cleanup
			LevelManager.I.OnStopLevel += LevelStopLevelCleanUp;

			// Notify that player has spawned
			return spawnedPlayerGO;
		}

		private void SetSpawnedPlayerGO(GameObject newGO)
		{
			SpawnedPlayerGO = newGO;
			spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<Life>();
			spawnedPlayerDefence.OnDead += OnPlayerDied; // Updated to use new method name

			foreach (var component in newGO.GetComponents<INeedPlayer>())
			{
				component.SetPlayer(this);
				Debug.Log("set player " + this.playerIndex + " to " + component.GetType().Name);
			}

			sayer = SpawnedPlayerGO.GetComponentInChildren<PlayerSayer>();
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
			data = playerData;
			if (playerInput == null)
			{
				if (!data.isPlayer) return;
			}
			else
				playerIndex = playerInput != null ? playerInput.playerIndex : 5;

			playerColor = data.playerColor;

			input = playerInput;
			Controller = GetComponent<PlayerController>();
			Controller.InitializeAndLinkToPlayer(this);
			isUsingMouse = input.GetDevice<Mouse>() != null || input.GetDevice<Keyboard>() != null;
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
