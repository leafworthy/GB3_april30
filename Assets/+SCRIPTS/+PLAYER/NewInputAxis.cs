using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __SCRIPTS
{
	public class NewInputAxis : IControlAxis
	{
		private InputAction action;
		private Vector2 currentDir;
		private bool IsActive;
		private Vector2 lastDir;
		private bool hasDirection;

		public bool isActive => IsActive;

		public NewInputAxis(InputAction _action, Player _owner)
		{
			action = _action;
			owner = _owner;
			owner.input.onActionTriggered += owner_onAction;
		}

		private void owner_onAction(InputAction.CallbackContext obj)
		{
			if (obj.action.name != action.name) return;
			if (obj.action.actionMap.name != action.actionMap.name) return;
			hasDirection = true;
			var dir = obj.action.ReadValue<Vector2>();
			UpdateDir(dir);
			lastDir = dir;
		}

		private void UpdateDir(Vector2 newDir)
		{
			currentDir = newDir;
			OnChange?.Invoke(this, currentDir);

			bool tooSmall = currentMagnitudeIsTooSmall();

			// Handle inactive state
			if (tooSmall)
			{
				if (IsActive)
				{
					IsActive = false;
					OnInactive?.Invoke(this);
					Debug.Log("[INPUT] On inactive, magnitude = " + currentDir.magnitude);
				}

				// Always reset directional state when inactive
				if (RightPressed || LeftPressed || UpPressed || DownPressed)
				{
					RightPressed = LeftPressed = UpPressed = DownPressed = false;
				}

				return;
			}

			// Handle transition to active state
			if (!IsActive)
			{
				IsActive = true;
				OnActive?.Invoke(this);
				Debug.Log("[INPUT] On active magnitude = " + currentDir.magnitude);

				// Reset directional states on activation to avoid stale triggers
				RightPressed = LeftPressed = UpPressed = DownPressed = false;
			}

			// Horizontal movement
			if (newDir.x > 0.01f)
			{
				if (!RightPressed)
				{
					RightPressed = true;
					LeftPressed = false;
					OnRight?.Invoke(this);
				}
			}
			else if (newDir.x < -0.01f)
			{
				if (!LeftPressed)
				{
					LeftPressed = true;
					RightPressed = false;
					OnLeft?.Invoke(this);
				}
			}
			else
			{
				RightPressed = LeftPressed = false;
			}

			// Vertical movement
			if (newDir.y > 0.01f)
			{
				if (!UpPressed)
				{
					UpPressed = true;
					DownPressed = false;
					OnUp?.Invoke(this);
				}
			}
			else if (newDir.y < -0.01f)
			{
				if (!DownPressed)
				{
					DownPressed = true;
					UpPressed = false;
					OnDown?.Invoke(this);
				}
			}
			else
			{
				UpPressed = DownPressed = false;
			}
		}
		public bool currentMagnitudeIsTooSmall() => (currentDir.magnitude < .3f);

		private bool UpPressed;
		private bool DownPressed;
		private bool LeftPressed;
		private bool RightPressed;
		public Player owner { get; set; }
		public event Action<NewInputAxis, Vector2> OnChange;
		public event Action<NewInputAxis> OnActive;
		public event Action<NewInputAxis> OnInactive;
		public event Action<NewInputAxis> OnRight;
		public event Action<NewInputAxis> OnLeft;
		public event Action<NewInputAxis> OnUp;
		public event Action<NewInputAxis> OnDown;
		public Vector3 GetCurrentAngle() => currentDir;

		public void update()
		{
			if (!hasDirection) return;
			UpdateDir(lastDir);
		}
	}
}
