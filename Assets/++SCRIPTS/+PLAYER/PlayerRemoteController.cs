using System;
using UnityEngine;

public class PlayerRemoteController : MonoBehaviour, IMovementController, IPlayerController
{
	[SerializeField] private PlayerIndex playerIndex;

	private bool isOn = true;

	private bool attackPressed;
	private bool aimPressed;
	private bool movePressed;
	private bool chargePressed;
	private bool dashButtonPressed;
	private bool rightTriggerPressed;

	private float triggerTolerance = .6f;
	private float stickTolerance = .05f;
	private float aimTolerance = .2f;

	private float rAxis;
	private float lAxis;
	private bool startButtonPressed;
	private Player owner;
	private bool leftTriggerPressed;
	private bool reloadPressed;
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
	public event Action OnReloadRelease;

	public event Action OnAttackRelease;

	public event Action OnMoveRelease;
	public event Action<Vector3> OnMovePress;

	public event Action OnAimStickInactive;
	public event Action<Vector3> OnAim;

	public event Action OnChargePress;
	public event Action OnChargeRelease;


	private void Start()
	{
		isOn = true;
	}

	private void Update()
	{
		HandlePausing();
		if (!isOn) return;
		if(PAUSE.isPaused) return;
		HandleAiming();
		HandleReloading();
		HandleMovement();
		HandleAttacking();
		HandleJumping();
		HandleCharging();
		HandleDashing();
	}

	private void HandlePausing()
	{
		if (Is_Start_ButtonDown())
		{
			if (!startButtonPressed)
			{
				startButtonPressed = true;
				OnPauseButtonPress?.Invoke(owner);
			}
		}
		else
		{
			OnPauseButtonRelease?.Invoke(owner);
			startButtonPressed = false;
		}
	}

	private void HandleDashing()
	{

		if (Is_Y_ButtonDown())
		{
			if (!dashButtonPressed)
			{
				dashButtonPressed = true;
				OnDashButtonPress?.Invoke();
			}
		}
		else
		{
			OnDashButtonRelease?.Invoke();
			dashButtonPressed = false;
		}

	}

	private bool Is_Start_ButtonDown()
	{
		return GamePad.GetButton(CButton.Start, playerIndex);
	}

	private bool Is_Y_ButtonDown()
	{
		return GamePad.GetButton(CButton.Y, playerIndex);
	}

	private void HandleReloading()
	{
		if (Is_RightBumper_ButtonDown())
		{
			if (reloadPressed) return;
			OnReloadPress?.Invoke();
			reloadPressed = true;
		}
		else if (reloadPressed)
		{
			OnReloadRelease?.Invoke();
			reloadPressed = false;
		}
	}
	private void HandleCharging()
	{
		if (Is_B_ButtonDown())
		{
			OnChargePress?.Invoke();
			chargePressed = true;
		}
		else if (chargePressed)
		{
			OnChargeRelease?.Invoke();
			chargePressed = false;
		}
	}

	private bool Is_RightBumper_ButtonDown()
	{
		return GamePad.GetButton(CButton.RB, playerIndex);
	}
	private bool Is_B_ButtonDown()
	{
		return GamePad.GetButton(CButton.B, playerIndex);
	}

	private void HandleAttacking()
	{
		if (IsAttackButtonDown())
		{
			if (!attackPressed)
			{
				OnAttackPress?.Invoke(GetRightStickDir());
			}

			attackPressed = true;
		}
		else if (attackPressed)
		{
			attackPressed = false;
			OnAttackRelease?.Invoke();
		}

		if (IsLeftTriggerDown())
		{
			OnLeftTriggerPress?.Invoke(GetRightStickDirAmplitude());
			Debug.Log("LEFTTRIGGERPRESSED");
			leftTriggerPressed = true;
		}
		else if (leftTriggerPressed)
		{
			leftTriggerPressed = false;
			OnLeftTriggerRelease?.Invoke(GetRightStickDirAmplitude());
		}

		if (IsRightTriggerDown())
		{
			OnRightTriggerPress?.Invoke(GetRightStickDir());
			rightTriggerPressed = true;
		}
		else if (rightTriggerPressed)
		{
			rightTriggerPressed = false;
			OnRightTriggerRelease?.Invoke();
		}
	}

	private bool IsLeftTriggerDown()
	{
		lAxis = GamePad.GetAxis(CAxis.LT, playerIndex);
		return lAxis >= triggerTolerance;
	}

	private bool IsAttackButtonDown()
	{
		return GamePad.GetButton(CButton.X, playerIndex);
	}

	private void HandleJumping()
	{
		if (IsJumpButtonDown())
		{
			OnJumpPress?.Invoke();
		}
		else
		{
			OnJumpRelease?.Invoke();
		}
	}
	private bool IsJumpButtonDown()
	{
		return GamePad.GetButton(CButton.A, playerIndex);
	}

	private void HandleMovement()
	{
		if (IsMoveButtonDown())
		{
			movePressed = true;
			OnMovePress?.Invoke(GetMoveStickDir());
		}
		else if (movePressed)
		{
			movePressed = false;
			OnMoveRelease?.Invoke();
		}
	}
	private bool IsMoveButtonDown()
	{
		return GetMoveStickDir().magnitude >= stickTolerance;
	}

	private void HandleAiming()
	{
		if (IsAimStickActive())
		{
			OnAim?.Invoke(GetRightStickDir());
			aimPressed = true;
		}
		else if (aimPressed)
		{
			aimPressed = false;
			OnAimStickInactive?.Invoke();
		}
	}
	private bool IsAimStickActive()
	{
		return GetRightStickDirUnnormalized().magnitude >= aimTolerance;
	}

	private bool IsRightTriggerDown()
	{
		rAxis = GamePad.GetAxis(CAxis.RT, playerIndex);
		return rAxis >= triggerTolerance;
	}

	private Vector3 GetLeftStickDir()
	{
		return new Vector3(GamePad.GetAxis(CAxis.LX, playerIndex),
			-GamePad.GetAxis(CAxis.LY, playerIndex), 0).normalized;
	}

	private Vector3 GetRightStickDir()
	{
		return new Vector3(GamePad.GetAxis(CAxis.RX, playerIndex),
			-GamePad.GetAxis(CAxis.RY, playerIndex), 0).normalized;
	}

	private Vector3 GetRightStickDirUnnormalized()
	{
		return new Vector3(GamePad.GetAxis(CAxis.RX, playerIndex),
			-GamePad.GetAxis(CAxis.RY, playerIndex), 0);
	}

	private Vector3 GetRightStickDirAmplitude()
	{
		return new Vector3(GamePad.GetAxis(CAxis.RX, playerIndex),
			-GamePad.GetAxis(CAxis.RY, playerIndex), 0);
	}

	private Vector3 GetMoveStickDir()
	{
		return new Vector3(GamePad.GetAxis(CAxis.LX, playerIndex),
			GamePad.GetAxis(CAxis.LY, playerIndex), 0).normalized;
	}



	public void SetPlayer(Player player)
	{
		owner = player;
		playerIndex = player.playerIndex;

	}

	public Color GetPlayerColor()
	{
		if (owner != null)
		{
			return owner.playerColor;
		}
		else
		{
			return Color.white;
		}
	}
}
