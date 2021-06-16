using System;
using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
	public class PlayerKeyboardMouseController : MonoBehaviour, IMovementController, IPlayerController
	{
		[SerializeField] private PlayerIndex playerIndex;

		public KeyboardControlSetup keySetup;

		private bool isOn = true;
		public GameObject aimCenter;
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
		public event Action<Vector3> OnMoveDirectionRelease;
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

		private void UpdateAimDirection()
		{
			var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
			var mouseDir = (mousePos - aimCenter.transform.position).normalized;
			OnAim?.Invoke(mouseDir);
		}

		private void UpdateMovementDirection()
		{
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
				if (isMoving)
				{
					isMoving = false;
					OnMoveRelease?.Invoke();
				}
			}
			else
			{
				isMoving = true;
				OnMovePress?.Invoke(moveDirection);
			}

		}

		private void OnMoveUpPress()
		{
			isMovingUp = true;
			OnMovePress?.Invoke(moveDirection);
		}

		private void OnMoveRightPress()
		{
			isMovingRight = true;
			OnMovePress?.Invoke(Vector3.right);
		}

		private void OnMoveDownPress()
		{
			isMovingDown = true;

			OnMovePress?.Invoke(Vector3.down);
		}

		private void OnMoveLeftPress()
		{
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

		private List<KeyboardKey> keys = new List<KeyboardKey>();
		private bool isMoving;

		private void Start()
		{
			cam = Camera.main;
			isOn = true;

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

			keySetup.JumpKey.OnPress += OnJumpPress;
			keySetup.JumpKey.OnRelease += OnJumpRelease;
			keys.Add(keySetup.JumpKey);

			keySetup.PauseKey.OnPress += OnPausePress;
			keySetup.PauseKey.OnRelease += OnPauseRelease;
			//keys.Add(PauseKey);

			keySetup.DashKey.OnPress += OnDashButtonPress;
			keySetup.DashKey.OnRelease += OnDashButtonRelease;
			keys.Add(keySetup.DashKey);

		}

		private void OnPauseRelease()
		{
			OnPauseButtonRelease?.Invoke(owner);
		}

		private void OnPausePress()
		{
			OnPauseButtonPress?.Invoke(owner);
		}

		private void OnAttack1Press()
		{
			OnRightTriggerPress?.Invoke(aimDirection);
		}

		private void OnAttack2Press()
		{
			OnLeftTriggerPress?.Invoke(aimDirection);
		}

		private void OnAttack1Release()
		{
			OnRightTriggerRelease?.Invoke();
		}

		private void OnAttack2Release()
		{
			OnLeftTriggerRelease?.Invoke(aimDirection);
		}


		private void Update()
		{
			keySetup.PauseKey.update();
			if (!isOn) return;
			if (PAUSE.isPaused) return;
			foreach (var key in keys)
			{
				key.update();
			}

			UpdateAimDirection();
			UpdateMovementDirection();
		}

		public void SetPlayer(Player player)
		{
			owner = player;
			playerIndex = player.playerIndex;
		}

		public Color GetPlayerColor()
		{
			if (owner != null)
				return owner.playerColor;
			else
				return Color.white;
		}
	}
}
