using System;
using UnityEngine;

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
	public PlayerData data;

	public CharacterButton currentButton;
	public GameObject SpawnedPlayerGO;
	private DefenceHandler spawnedPlayerDefence;
	private IPlayerAttackHandler attackHandler;
	private IPlayerMenuControl menuControl;

	public bool hasJoined;
	public bool hasChosenCharacter;
	public bool isDead;
	public int buttonIndex;
	public bool hasSpawned;
	public bool isJoining;
	public Character currentCharacter;

	public event Action<Player> OnDead;
	public event Action<Player> MoveRight;
	public event Action<Player> MoveLeft;
	public event Action<Player> MoveUp;
	public event Action<Player> MoveDown;
	public event Action<Player> PressA;
	public event Action<Player> PressB;
	public event Action<Player> PressPause;

	private void Start()
	{
		SetupMenuControl();

		LEVELS.OnLevelStop += CleanUp;
	}

	private void SetupMenuControl()
	{
		menuControl = data.isUsingKeyboard
			? (IPlayerMenuControl) new PlayerMenuKeyboardControl(this)
			: new PlayerMenuRemoteControl(this);

		menuControl.MenuMoveDown += OnMoveDown;
		menuControl.MenuMoveUp += OnMoveUp;
		menuControl.MenuMoveLeft += OnMoveLeft;
		menuControl.MenuMoveRight += OnMoveRight;
		menuControl.MenuPressA += OnPressA;
		menuControl.MenuPressB += OnPressB;
		menuControl.MenuPressPause += OnPressPause;
	}

	private void OnPressPause(Player obj)
	{
		PressPause?.Invoke(obj);
	}

	private void OnMoveUp(Player obj)
	{
		MoveUp?.Invoke(obj);
	}

	private void OnMoveDown(Player obj)
	{
		MoveDown?.Invoke(obj);
	}

	private void OnPressA(Player obj)
	{
		PressA?.Invoke(obj);
	}

	private void OnPressB(Player obj)
	{
		PressB?.Invoke(obj);
	}

	private void OnMoveRight(Player obj)
	{
		MoveRight?.Invoke(obj);
	}

	private void OnMoveLeft(Player obj)
	{
		MoveLeft?.Invoke(obj);
	}

	public void JoinSelection(CharacterButton firstButton)
	{
		hasJoined = true;
		firstButton.HighlightButton(this);
	}

	private void Update()
	{
		menuControl?.UpdateButtons();
	}

	private void SetSpawnedPlayerGO(GameObject newGO)
	{
		SpawnedPlayerGO = newGO;
		spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<DefenceHandler>();
		spawnedPlayerDefence.OnDead += Dead;
		var stats = SpawnedPlayerGO.GetComponent<UnitStats>();
		stats.SetPlayer(this);
	}


	private void Dead()
	{
		OnDead?.Invoke(this);
	}

	private void CleanUp()
	{
		Debug.Log("player cleanup");
		hasJoined = false;
		hasSpawned = false;
		currentButton = null;
		hasChosenCharacter = false;
		SpawnedPlayerGO = null;
		isDead = false;
		if (spawnedPlayerDefence != null)
		{
			spawnedPlayerDefence.OnDead -= Dead;
		}

		if (menuControl == null) return;
		menuControl.MenuMoveDown -= OnMoveDown;
		menuControl.MenuMoveUp -= OnMoveUp;
		menuControl.MenuMoveLeft -= OnMoveLeft;
		menuControl.MenuMoveRight -= OnMoveRight;
		menuControl.MenuPressA -= OnPressA;
		menuControl.MenuPressB -= OnPressB;
		menuControl.MenuPressPause -= OnPressPause;
		SetupMenuControl();
	}

	private void SetController()
	{
		IPlayerController newPlayerController;
		if (data.isUsingKeyboard)
			newPlayerController = SpawnedPlayerGO.AddComponent<PlayerKeyboardMouseController>();
		else
			newPlayerController = SpawnedPlayerGO.AddComponent<PlayerRemoteController>();
		newPlayerController.SetPlayer(this);
	}

	public void Spawn(Vector2 SpawnPoint)
	{
		if (hasSpawned) return;
		hasSpawned = true;

		var spawnedPlayerGO = MAKER.Make(GetPrefabFromCharacter(this), SpawnPoint);
		SetSpawnedPlayerGO(spawnedPlayerGO);
		SetController();
	}

	private static GameObject GetPrefabFromCharacter(Player player)
	{
		switch (player.currentCharacter)
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
}
