using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardMouseController : MonoBehaviour, IPlayerController, IDashController
{

	private KeyboardControlSetup keySetup;

	private bool isOn = true;
	private Vector3 aimDirection;
	private Vector3 moveDirection;

	private Player owner;

	public event Action<Player> OnPauseButtonPress;
	public event Action<Player> OnPauseButtonRelease;

	public event Action<Vector3> OnRightTriggerPress;
	public event Action OnRightTriggerRelease;

	public event Action<Vector3> OnLeftTriggerPress;
	public event Action<Vector3> OnLeftTriggerRelease;

	public event Action OnDashButtonPress;
	public event Action OnDashButtonRelease;

	public event Action OnJumpPress;
	public event Action OnJumpRelease;

	public event Action<Vector3> OnAttackPress;
	public event Action OnReloadPress;
	public event Action OnAttackRelease;

	public event Action OnMoveRelease;
	public event Action<Vector3> OnMovePress;

	public event Action OnAimStickInactive;
	public event Action<Vector3> OnAim;

	public event Action OnChargePress;
	public event Action OnChargeRelease;

	private DefenceHandler defenceHandler;
	private bool isMovingUp;
	private bool isMovingRight;
	private bool isMovingLeft;
	private bool isMovingDown;
	private List<KeyboardKey> keys = new List<KeyboardKey>();
	private bool isMoving;

	private IAimHandler aimer;

	private void Start()
	{

		LEVELS.OnLevelStart += GameStart;
		LEVELS.OnLevelStop += GameEnd;
	}

	private void GameEnd()
	{
		isOn = false;
	}

	private void GameStart(List<Player> players)
	{
		isOn = true;
	}

	private void Init()
	{
		isOn = true;
		aimer = GetComponent<IAimHandler>();
		if (aimer == null)
		{
			Debug.Log("aimer is null");
			Debug.Break();
		}
		CURSOR.ShowCursor();
		Menu_Pause.OnPause += PauseStart;
		Menu_Pause.OnUnpause += PauseStop;
		defenceHandler = GetComponent<DefenceHandler>();
		defenceHandler.OnDead += Dead;
		keySetup.MoveUpKey.OnPress += OnMoveUpPress;
		keySetup.MoveUpKey.OnRelease += OnMoveUpPressRelease;
		keys.Add(keySetup.MoveUpKey);

		keySetup.MoveRightKey.OnPress += OnMoveRightPress;
		keySetup.MoveRightKey.OnRelease += OnMoveRightRelease;
		keys.Add(keySetup.MoveRightKey);

		keySetup.MoveDownKey.OnPress += OnMoveDownPress;
		keySetup.MoveDownKey.OnRelease += OnMoveDownRelease;
		keys.Add(keySetup.MoveDownKey);

		keySetup.MoveLeftKey.OnPress += OnMoveLeftPress;
		keySetup.MoveLeftKey.OnRelease += OnMoveLeftRelease;
		keys.Add(keySetup.MoveLeftKey);

		keySetup.Attack1Key.OnPress += OnAttack1Press;
		keySetup.Attack1Key.OnRelease += OnAttack1Release;
		keys.Add(keySetup.Attack1Key);

		keySetup.Attack2Key.OnPress += OnAttack2Press;
		keySetup.Attack2Key.OnRelease += OnAttack2Release;
		keys.Add(keySetup.Attack2Key);

		keySetup.JumpKey.OnPress += OnJumpPressEvent;
		keySetup.JumpKey.OnRelease += OnJumpReleaseEvent;
		keys.Add(keySetup.JumpKey);

		keySetup.Special4Key.OnPress += OnChargePressEvent;
		keySetup.Special4Key.OnRelease += OnChargeReleaseEvent;
		keys.Add(keySetup.Special4Key);

		keySetup.DashKey.OnPress += OnDashButtonPressEvent;
		keySetup.DashKey.OnRelease += OnDashButtonReleaseEvent;
		keys.Add(keySetup.DashKey);

		keySetup.DashKey.OnPress += OnDashButtonPressEvent;
		keySetup.DashKey.OnRelease += OnDashButtonReleaseEvent;
		keys.Add(keySetup.DashKey);

		keySetup.ReloadKey.OnPress += OnReloadButtonPressEvent;
		keys.Add(keySetup.ReloadKey);

		keySetup.KnifeKey.OnPress += OnKnifeKeyButtonPressEvent;
		keySetup.KnifeKey.OnRelease += OnKnifeKeyButtonReleaseEvent;
		keys.Add(keySetup.KnifeKey);

		keySetup.PauseKey.OnPress += OnPauseKeyButtonPressEvent;
		keySetup.PauseKey.OnRelease += OnPauseKeyButtonReleaseEvent;
		keys.Add(keySetup.KnifeKey);

	}

	private void OnKnifeKeyButtonReleaseEvent()
	{
		OnAttackRelease?.Invoke();
	}

	private void OnPauseKeyButtonPressEvent()
	{
	OnPauseButtonPress?.Invoke(owner);
	}

	private void OnPauseKeyButtonReleaseEvent()
	{
		OnPauseButtonRelease?.Invoke(owner);
	}

	private void PauseStop()
	{
		if (!owner.isDead)
		{
			isOn = true;
		}
	}

	private void PauseStart(Player obj)
	{
		isOn = false;
	}

	private void Dead()
	{
		isOn = false;
	}

	private void OnChargePressEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnChargePress?.Invoke();
	}

	private void OnChargeReleaseEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnChargeRelease?.Invoke();
	}

	private void OnKnifeKeyButtonPressEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnAttackPress?.Invoke(Vector2.zero);
	}

	private void OnReloadButtonPressEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnReloadPress?.Invoke();
	}

	private void OnDashButtonReleaseEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnDashButtonRelease?.Invoke();
	}

	private void OnDashButtonPressEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnDashButtonPress?.Invoke();
	}

	private void OnJumpReleaseEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnJumpRelease?.Invoke();
	}

	private void OnJumpPressEvent()
	{
		if (Menu_Pause.isPaused) return;
		OnJumpPress?.Invoke();
	}

	private void UpdateMovementDirection()
	{
		if (Menu_Pause.isPaused) return;
		if (!isMovingUp && !isMovingRight && !isMovingDown && !isMovingLeft)
		{
			moveDirection = Vector2.zero;
			if (isMoving)
			{
				isMoving = false;
				OnMoveRelease?.Invoke();
				return;
			}
		}
		var tempDirection = Vector2.zero;
		if (isMovingUp) tempDirection += Vector2.up;

		if (isMovingRight) tempDirection += Vector2.right;

		if (isMovingDown) tempDirection += Vector2.down;
		if (isMovingLeft) tempDirection += Vector2.left;

		moveDirection = tempDirection.normalized;
		if (moveDirection == Vector3.zero)
		{
			if (!isMoving) return;
			isMoving = false;
			OnMoveRelease?.Invoke();
		}
		else
		{
			isMoving = true;
			OnMovePress?.Invoke(moveDirection);
		}

	}

	private void OnMoveUpPress()
	{
		if (Menu_Pause.isPaused) return;
		isMovingUp = true;
		OnMovePress?.Invoke(moveDirection);
	}

	private void OnMoveRightPress()
	{
		if (Menu_Pause.isPaused) return;
		isMovingRight = true;
		OnMovePress?.Invoke(Vector3.right);
	}

	private void OnMoveDownPress()
	{
		if (Menu_Pause.isPaused) return;
		isMovingDown = true;

		OnMovePress?.Invoke(Vector3.down);
	}

	private void OnMoveLeftPress()
	{
		if (Menu_Pause.isPaused) return;
		isMovingLeft = true;
		OnMovePress?.Invoke(Vector3.left);
	}

	private void OnMoveUpPressRelease()
	{
		isMovingUp = false;
	}

	private void OnMoveRightRelease()
	{
		isMovingRight = false;
	}

	private void OnMoveDownRelease()
	{
		isMovingDown = false;
	}

	private void OnMoveLeftRelease()
	{
		isMovingLeft = false;
	}

	private void OnAttack1Press()
	{
		if (Menu_Pause.isPaused) return;
		OnRightTriggerPress?.Invoke(aimDirection);
	}

	private void OnAttack2Press()
	{
		if (Menu_Pause.isPaused) return;
		OnLeftTriggerPress?.Invoke(aimDirection);
	}

	private void OnAttack1Release()
	{
		if (Menu_Pause.isPaused) return;
		OnRightTriggerRelease?.Invoke();
	}

	private void OnAttack2Release()
	{
		if (Menu_Pause.isPaused) return;
		OnLeftTriggerRelease?.Invoke(aimDirection);
	}


	private void Update()
	{
		if (owner == null)
		{
			Debug.Log("no owner");
			return;
		}
		keySetup.PauseKey.update();
		if (!isOn) return;
		if (Menu_Pause.isPaused)
		{
			Debug.Log("paused");
			return;
		}
		foreach (var key in keys)
		{
			key.update();
		}

		UpdateMovementDirection();
		UpdateAimDirection();
	}

	private void UpdateAimDirection()
	{
		aimDirection = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition) - aimer.GetAimCenter() ;
		if (aimDirection.magnitude <= .1f)
		{
			OnAimStickInactive?.Invoke();
		}
		OnAim?.Invoke(aimDirection);
	}

	public void SetPlayer(Player player)
	{
		owner = player;

		if (player.data.isUsingKeyboard)
		{
			keySetup = player.data.keyboardControlSetup;
		}

		Init();

	}

	public Color GetPlayerColor()
	{
		return owner != null ? owner.data. playerColor : Color.white;
	}
}
