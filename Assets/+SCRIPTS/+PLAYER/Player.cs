using System;
using __SCRIPTS.UpgradeS;
using GangstaBean.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __SCRIPTS
{
	[Serializable]
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

		public CharacterButton CurrentButton;
		public int buttonIndex;

		//SET DURING CHARACTER SELECT
		public Character CurrentCharacter;
		public Color playerColor;

		private PlayerSayer sayer;

		public int playerIndex;
		public bool hasKey;

		public event Action<Player> OnPlayerDies;
		public event Action<Player> OnPlayerLeavesUpgradeSetupMenu;

		private PlayerUpgrades playerUpgrades;
		private int buildingLayer;
		public int BuildingLayer => Services.assetManager.LevelAssets.BuildingLayer;

		public bool IsPlayer() => data != null && data.isPlayer;

		private void Start()
		{
			playerUpgrades = GetComponent<PlayerUpgrades>();
			gameObject.transform.SetParent(GameManager.I.gameObject.transform);
		}

		public void OnPlayerDied(Player player)
		{
			SetState(State.Dead);
			OnPlayerDies?.Invoke(this);
			Services.playerStatsManager.SetStatAmount(this, PlayerStat.StatType.TimeSurvived, Services.levelManager.GetCurrentLevelTimeElapsed());
		}

		public GameObject Spawn(Vector2 position)
		{
			SetState(State.Alive);

			var spawnedPlayerGO = Instantiate(GetPrefabFromCharacter(this));
			spawnedPlayerGO.transform.position = position;
			SetSpawnedPlayerGO(spawnedPlayerGO);

			var animations = spawnedPlayerGO.GetComponentInChildren<UnitAnimations>();
			if (animations != null) animations.SetBool(UnitAnimations.IsFallingFromSky, true);

			if (playerUpgrades != null) playerUpgrades.ApplyUpgrades(this);
			return spawnedPlayerGO;
		}

		private void SetSpawnedPlayerGO(GameObject newGO)
		{
			if (newGO == null) return;

			SpawnedPlayerGO = newGO;
			spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<Life>();
			if (spawnedPlayerDefence == null) return;

			spawnedPlayerDefence.OnDeathComplete += OnPlayerDied;
			spawnedPlayerDefence.SetPlayer(this);
		}

		private GameObject GetPrefabFromCharacter(Player player)
		{
			switch (player.CurrentCharacter)
			{
				case Character.Karrot:
					return Services.assetManager.Players.GangstaBeanPlayerPrefab;
				case Character.Bean:
					return Services.assetManager.Players.GangstaBeanPlayerPrefab;
				case Character.Brock:
					return Services.assetManager.Players.BrockLeePlayerPrefab;
				case Character.Tmato:
					return Services.assetManager.Players.TMatoPlayerPrefab;
			}

			return null;
		}

		public void Say(string message, float sayTimeInSeconds = 3)
		{
			if (sayer == null) sayer = SpawnedPlayerGO.GetComponentInChildren<PlayerSayer>();
			sayer.Say(message, sayTimeInSeconds);
		}

		public void ConnectPlayerToController(PlayerInput playerInput, PlayerData playerData, int index)
		{
			if (playerData == null) return;

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
				isUsingMouse = input.GetDevice<Mouse>() != null || input.GetDevice<Keyboard>() != null;
			}

			playerColor = data.playerColor;

			Controller = GetComponent<PlayerController>();
			if (Controller == null) return;

			Controller.InitializeAndLinkToPlayer(this);
			if (SpawnedPlayerGO != null)
			{
				foreach (var needPlayer in SpawnedPlayerGO.GetComponentsInChildren<INeedPlayer>(true))
				{
					needPlayer.SetPlayer(this);
				}
			}
		}

		public void StartSelectingCharacter()
		{
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

		public bool HasMoreMoneyThan(int amount) => Services.playerStatsManager.GetStatAmount(this, PlayerStat.StatType.TotalCash) >= amount;

		public void SpendMoney(int amount)
		{
			Services.playerStatsManager.ChangeStat(this, PlayerStat.StatType.TotalCash, -amount);
		}

		public bool isDead() => spawnedPlayerDefence.IsDead();

		public void LeaveUpgradeSetupMenu()
		{
			OnPlayerLeavesUpgradeSetupMenu?.Invoke(this);
		}
	}
}
