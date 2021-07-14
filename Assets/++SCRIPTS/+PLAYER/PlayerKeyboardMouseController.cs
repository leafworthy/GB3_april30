using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardMouseController : MonoBehaviour, IMovementController, IPlayerController
{

	private KeyboardControlSetup keySetup;

	private bool isOn = true;
	private Vector3 aimDirection;
	private Vector3 moveDirection;
	private Vector3 aimPosition;

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

	private bool isMovingUp;
	private bool isMovingRight;
	private bool isMovingLeft;
	private bool isMovingDown;
	private Camera cam;
	private List<KeyboardKey> keys = new List<KeyboardKey>();
	private bool isMoving;

	private IAimHandler aimer;

	private void Init()
	{
		cam = Camera.main;
		isOn = true;
		aimer = GetComponent<IAimHandler>();
		if (aimer == null)
		{
			Debug.Log("aimer is null");
			Debug.Break();
		}
		CURSOR.ShowCursor();

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
		keys.Add(keySetup.KnifeKey);

	}
	private void OnChargePressEvent()
	{
		if (PAUSE.isPaused) return;
		OnChargePress?.Invoke();
	}

	private void OnChargeReleaseEvent()
	{
		if (PAUSE.isPaused) return;
		OnChargeRelease?.Invoke();
	}

	private void OnKnifeKeyButtonPressEvent()
	{
		if (PAUSE.isPaused) return;
		OnAttackPress?.Invoke(Vector2.zero);
	}

	private void OnReloadButtonPressEvent()
	{
		if (PAUSE.isPaused) return;
		OnReloadPress?.Invoke();
	}

	private void OnDashButtonReleaseEvent()
	{
		if (PAUSE.isPaused) return;
		OnDashButtonRelease?.Invoke();
	}

	private void OnDashButtonPressEvent()
	{
		if (PAUSE.isPaused) return;
		OnDashButtonPress?.Invoke();
	}

	private void OnJumpReleaseEvent()
	{
		if (PAUSE.isPaused) return;
		OnJumpRelease?.Invoke();
	}

	private void OnJumpPressEvent()
	{
		if (PAUSE.isPaused) return;
		OnJumpPress?.Invoke();
	}

	private void UpdateMovementDirection()
	{
		if (PAUSE.isPaused) return;
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
		if (PAUSE.isPaused) return;
		isMovingUp = true;
		OnMovePress?.Invoke(moveDirection);
	}

	private void OnMoveRightPress()
	{
		if (PAUSE.isPaused) return;
		isMovingRight = true;
		OnMovePress?.Invoke(Vector3.right);
	}

	private void OnMoveDownPress()
	{
		if (PAUSE.isPaused) return;
		isMovingDown = true;

		OnMovePress?.Invoke(Vector3.down);
	}

	private void OnMoveLeftPress()
	{
		if (PAUSE.isPaused) return;
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
		if (PAUSE.isPaused) return;
		OnRightTriggerPress?.Invoke(aimDirection);
	}

	private void OnAttack2Press()
	{
		if (PAUSE.isPaused) return;
		OnLeftTriggerPress?.Invoke(aimDirection);
	}

	private void OnAttack1Release()
	{
		if (PAUSE.isPaused) return;
		OnRightTriggerRelease?.Invoke();
	}

	private void OnAttack2Release()
	{
		if (PAUSE.isPaused) return;
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
		if (PAUSE.isPaused)
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
		Debug.Log("update aim direction " + aimDirection);
		OnAim?.Invoke(aimDirection);
	}

	public void SetPlayer(Player player)
	{
		owner = player;

		if (player.isUsingKeyboard)
		{
			keySetup = player.keyboardControlSetup;
		}

		Init();

	}

	public Color GetPlayerColor()
	{
		return owner != null ? owner.playerColor : Color.white;
	}
}
