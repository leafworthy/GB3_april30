using System;
using __SCRIPTS;
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
		PlayerData data;
		PlayerStatsSavedBetweenScenes statsBetweenScenes;

		public GameObject SpawnedPlayerGO;
		public ICanAttack spawnedPlayerAttacker;
		public IGetAttacked spawnedPlayerDefence;
		public IHaveUnitStats spawnedPlayerStats;

		public PlayerController Controller;
		public PlayerInput input;
		public bool isUsingMouse;

		public CharacterButton CurrentButton;
		public int buttonIndex;

		//SET DURING CHARACTER SELECT
		public Character CurrentCharacter;
		public Color playerColor;

		PlayerSayer sayer;

		public int playerIndex;
		public bool hasKey;

		public event Action<Player, bool> OnPlayerDies;
		public event Action OnPlayerSpawned;
		public event Action<Player> OnPlayerLeavesUpgradeSetupMenu;

		PlayerUpgrades playerUpgrades;
		int buildingLayer;
		bool isMainPlayer;

		public void SetIsMainPlayer(bool value)
		{
			isMainPlayer = value;
		}

		public int BuildingLayer => Services.assetManager.LevelAssets.BuildingLayer;

		public bool IsHuman() => isHuman;
		bool isHuman => data != null && data.isPlayer;

		void Start()
		{
			playerUpgrades = GetComponent<PlayerUpgrades>();
			gameObject.transform.SetParent(GameManager.I.gameObject.transform);
		}

		public void OnPlayerDied(Player player, bool isRespawning)
		{
			spawnedPlayerDefence.OnDeathComplete -= OnPlayerDied;
			SetState(State.Dead);
			OnPlayerDies?.Invoke(this, isRespawning);
			Services.playerStatsManager.SetStatAmount(this, PlayerStat.StatType.TimeSurvived, Services.levelManager.GetCurrentLevelTimeElapsed());
		}

		public GameObject Spawn(Vector2 position)
		{
			Services.sceneLoader.OnSceneAboutToChange += SceneLoader_OnSceneAboutToChange;
			SetState(State.Alive);

			var spawnedPlayerGO = Instantiate(GetPrefabFromCharacter(this));
			spawnedPlayerGO.transform.position = position;
			SetSpawnedPlayerGO(spawnedPlayerGO);

			var animations = spawnedPlayerGO.GetComponentInChildren<UnitAnimations>();
			if (animations != null) animations.SetBool(UnitAnimations.IsFallingFromSky, true);
			OnPlayerSpawned?.Invoke();
			statsBetweenScenes?.ApplyToPlayer(spawnedPlayerGO);
			if (playerUpgrades != null) playerUpgrades.ApplyUpgrades(this);
			return spawnedPlayerGO;
		}

		void SceneLoader_OnSceneAboutToChange()
		{
			Services.sceneLoader.OnSceneAboutToChange -= SceneLoader_OnSceneAboutToChange;
			if (SpawnedPlayerGO == null) return;
			if (spawnedPlayerDefence == null) return;
			if (spawnedPlayerDefence.IsDead())
			{
				statsBetweenScenes = null;
				return;
			}
			statsBetweenScenes = new PlayerStatsSavedBetweenScenes(SpawnedPlayerGO);
		}

		void SetSpawnedPlayerGO(GameObject newGO)
		{
			if (newGO == null) return;

			if (spawnedPlayerDefence != null) spawnedPlayerDefence.OnDeathComplete -= OnPlayerDied;
			SpawnedPlayerGO = newGO;
			spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<IGetAttacked>();
			spawnedPlayerAttacker = SpawnedPlayerGO.GetComponent<ICanAttack>();
			spawnedPlayerStats = SpawnedPlayerGO.GetComponent<IHaveUnitStats>();
			if (spawnedPlayerDefence == null) return;

			spawnedPlayerDefence.OnDeathComplete += OnPlayerDied;
			spawnedPlayerDefence.SetPlayer(this);
		}

		GameObject GetPrefabFromCharacter(Player player)
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

		public bool isDead()
		{
			if (spawnedPlayerDefence == null) return true;
			return spawnedPlayerDefence.IsDead();
		}

		public void LeaveUpgradeSetupMenu()
		{
			OnPlayerLeavesUpgradeSetupMenu?.Invoke(this);
		}

		public bool IsMainPlayer() => isMainPlayer;

		public LayerMask GetEnemyLayer() => IsHuman() ? Services.assetManager.LevelAssets.EnemyLayer : Services.assetManager.LevelAssets.PlayerLayer;

		public void Unalive()
		{
			if (spawnedPlayerDefence == null) return;
			spawnedPlayerDefence.DieNow();
		}
	}
}

public class PlayerStatsSavedBetweenScenes
{
	float CurrentHealth;
	int ammo1reserve;
	int ammo1clip;
	int ammo2reserve;
	int ammo2clip;
	int ammo3reserve;
	int ammo3clip;
	bool isPrimary;

	public PlayerStatsSavedBetweenScenes(GameObject spawnedPlayerGO)
	{
		Debug.Log("player stats saved");
		var life = spawnedPlayerGO.GetComponent<Life>();
		if (life == null) return;
		CurrentHealth = life.CurrentHealth;
		var ammo = life.GetComponent<AmmoInventory>();
		ammo1reserve = ammo.primaryAmmo.reserveAmmo;
		ammo1clip = ammo.primaryAmmo.AmmoInClip;
		ammo2reserve = ammo.secondaryAmmo.reserveAmmo;
		ammo2clip = ammo.secondaryAmmo.AmmoInClip;
		ammo3reserve = ammo.tertiaryAmmo.reserveAmmo;
		ammo3clip = ammo.tertiaryAmmo.AmmoInClip;
		var gunAttack = spawnedPlayerGO.GetComponentInChildren<GunAttack>();
		if (gunAttack != null) isPrimary = gunAttack.IsUsingPrimaryGun;
	}

	public void ApplyToPlayer(GameObject spawnedPlayerGO)
	{
		Debug.Log("Applying saved stats to player" + "\nHealth: " + "\nAmmo1: " + ammo1clip + "/" + ammo1reserve + "\nAmmo2: " + ammo2clip + "/" +
		          ammo2reserve + "\nAmmo3: " + ammo3clip + "/" + ammo3reserve);
		var life = spawnedPlayerGO.GetComponent<Life>();
		if (life != null) life.Stats.CurrentHealth = CurrentHealth;
		var ammoInventory = spawnedPlayerGO.GetComponent<AmmoInventory>();
		ammoInventory.primaryAmmo.SetAmmoInClip(ammo1clip);
		ammoInventory.primaryAmmo.SetAmmoReserve(ammo1reserve);
		ammoInventory.secondaryAmmo.SetAmmoInClip(ammo2clip);
		ammoInventory.secondaryAmmo.SetAmmoReserve(ammo2reserve);
		ammoInventory.tertiaryAmmo.SetAmmoInClip(ammo3clip);
		ammoInventory.tertiaryAmmo.SetAmmoReserve(ammo3reserve);
		var gunAttack = spawnedPlayerGO.GetComponentInChildren<GunAttack>();
		if (gunAttack != null) gunAttack.SwitchGuns(isPrimary);
	}
}
