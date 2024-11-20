using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum Character
{
	None,
	Karrot,
	Bean,
	Brock,
	Tmato
}

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

	public CharacterButton CurrentButton;
	public GameObject SpawnedPlayerGO;
	public PlayerController Controller;

	public Life spawnedPlayerDefence;

	public int buttonIndex;
	private UnitStats unitStats;
	[FormerlySerializedAs("currentCharacter")]
	public Character CurrentCharacter;

	private PlayerSayer sayer;
	public PlayerInput input;
	public bool isUsingMouse;
	public Color color;
	public string PlayerName = "unnamed";
	public static event Action<Player> OnPlayerDies;
	private AimAbility aimAbility;

	public PlayerStatsHandler playerStats;
	public int playerIndex;
	public bool hasKey;
	public List<PlayerInteractable> interactables = new();
	private PlayerInteractable selectedInteractable;
	public static event Action<Player> OnPlayerSpawned;

	public bool IsPlayer() => data.isPlayer;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
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
		if (interactables.Count == 0) return;
		var closest = interactables[0];
		var closestDistance = Vector2.Distance(closest.GetInteractionPosition(), GetAimPosition());
		foreach (var interactable in interactables)
		{
			var distance = Vector2.Distance(interactable.GetInteractionPosition(), GetAimPosition());

			if (!(distance < closestDistance)) continue;
			closest = interactable;
			closestDistance = distance;
		}

		if(closest == selectedInteractable)
		{
			if(selectedInteractable.isSelected) return;
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
		if (interactable == selectedInteractable) interactable.Deselect(this);
		interactables.Remove(interactable);
		SelectClosestInteractable();
	}

	private void SpawnedPlayer_Dead(Player player)
	{
		state = State.Dead;
		OnPlayerDies?.Invoke(this);
	}

	private void OnLevelStop_CleanUp(GameScene.Type type)
	{
		state = State.SelectingCharacter;
		CurrentButton = null;
		SpawnedPlayerGO = null;
		LevelGameScene.OnStop -= OnLevelStop_CleanUp;
		if (spawnedPlayerDefence != null) spawnedPlayerDefence.OnDead -= SpawnedPlayer_Dead;
	}

	public void Spawn(Vector2 SpawnPoint)
	{
		state = State.Alive;
		var spawnedPlayerGO = Instantiate(GetPrefabFromCharacter(this));
		spawnedPlayerGO.transform.position = SpawnPoint;
		SetSpawnedPlayerGO(spawnedPlayerGO);
		OnPlayerSpawned?.Invoke(this);
	}

	private void SetSpawnedPlayerGO(GameObject newGO)
	{
		SpawnedPlayerGO = newGO;
		spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<Life>();
		spawnedPlayerDefence.OnDead += SpawnedPlayer_Dead;
		unitStats = SpawnedPlayerGO.GetComponent<UnitStats>();
		unitStats.SetPlayer(this);
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
				return ASSETS.Players.BrockLeePlayerPrefab;
		}

		return null;
	}

	public void Say(string message, float sayTimeInSeconds = 3)
	{
		if (sayer == null) sayer = SpawnedPlayerGO.GetComponentInChildren<PlayerSayer>();
		Debug.Log("attempting to say: " + message);
		sayer.Say(message, sayTimeInSeconds);
	}

	public void Join(PlayerInput playerInput, PlayerData playerData, int index)
	{
		data = playerData;
		if (playerInput == null)
		{
			if (!data.isPlayer)
			{
				PlayerName = "Enemy Player" + index;
				return;
			}
		}
		else
			playerIndex = playerInput != null ? playerInput.playerIndex : 5;

		color = data.playerColor;

		PlayerName = "Player " + index;
		input = playerInput;
		Controller = GetComponent<PlayerController>();
		Controller.InitializeAndLinkToPlayer(this);
		isUsingMouse = input.GetDevice<Mouse>() != null || input.GetDevice<Keyboard>() != null;
		playerStats = new PlayerStatsHandler(this);

		LevelGameScene.OnStop += OnLevelStop_CleanUp;
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

	public int GetPlayerStatAmount(PlayerStat.StatType statType) => (int) playerStats.GetStatValue(statType);

	public void ChangePlayerStat(PlayerStat.StatType type, float change)
	{
		playerStats.ChangeStat(type, change);
	}

	public void GainKey()
	{
		hasKey = true;
	}

	public bool HasMoreMoneyThan(int amount) => playerStats.GetStatValue(PlayerStat.StatType.TotalCash) >= amount;

	public void SpendMoney(int amount)
	{
		playerStats.ChangeStat(PlayerStat.StatType.TotalCash, -amount);
	}

	public bool isDead() => spawnedPlayerDefence.IsDead();
}