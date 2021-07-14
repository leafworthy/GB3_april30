using System;
using System.Collections;
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
	public Character currentCharacter;
	public PlayerIndex playerIndex;
	public bool hasJoined;
	public CharacterButton currentButton;
	public bool hasSelectedCharacter;
	public Color playerColor;


	public bool isUsingKeyboard;
	public KeyboardControlSetup keyboardControlSetup;
	public GameObject SpawnedPlayerGO;
	private DefenceHandler spawnedPlayerDefence;
	private IAttackHandler attackHandler;
	public event Action<Player> OnDead;
	public event Action<Player> OnDying;
	public event Action<Player> OnKillEnemy;
	private IPlayerMenuControl menuControl;
	public bool isPlayer = true;


	public int buttonIndex;

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

		GAME.OnGameEnd += CleanUp;
	}

	private void SetupMenuControl()
	{
		menuControl = isUsingKeyboard
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

	public void Join(CharacterButton firstButton)
	{
		if(hasJoined) return;
		hasJoined = true;
		firstButton.HighlightButton(this);

	}

	private void Update()
	{
		menuControl?.UpdateButtons();
	}

	public void SetSpawnedPlayerGO(GameObject newGO)
	{
		Debug.Log("spawned player go set");
		SpawnedPlayerGO = newGO;
		spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<DefenceHandler>();
		spawnedPlayerDefence.OnDying += Die;
		spawnedPlayerDefence.OnDead += Dead;
		attackHandler = SpawnedPlayerGO.GetComponent<IAttackHandler>();
		attackHandler.OnKillEnemy += KillEnemy;
		var stats = SpawnedPlayerGO.GetComponent<UnitStats>();
		stats.SetPlayer(this);
	}

	private void KillEnemy()
	{
		OnKillEnemy?.Invoke(this);
	}

	private void Die()
	{
		OnDying?.Invoke(this);
	}

	private void Dead()
	{
		OnDead?.Invoke(this);
	}

	public void CleanUp()
	{
		currentCharacter = Character.None;
		hasJoined = false;
		currentButton = null;
		hasSelectedCharacter = false;
		SpawnedPlayerGO = null;
		if (spawnedPlayerDefence != null)
		{
			spawnedPlayerDefence.OnDying -= Die;
			spawnedPlayerDefence.OnDead -= Dead;
		}

		attackHandler.OnKillEnemy -= KillEnemy;
		menuControl.MenuMoveDown -= OnMoveDown;
		menuControl.MenuMoveUp -= OnMoveUp;
		menuControl.MenuMoveLeft -= OnMoveLeft;
		menuControl.MenuMoveRight -= OnMoveRight;
		menuControl.MenuPressA -= OnPressA;
		menuControl.MenuPressB -= OnPressB;
		menuControl.MenuPressPause -= OnPressPause;
	}

	public bool IsDead()
	{
		return spawnedPlayerDefence.IsDead();
	}
}
