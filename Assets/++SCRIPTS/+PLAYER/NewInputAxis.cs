using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputAxis : IControlAxis
{
	private InputAction action;
	private Vector2 currentDir;
	private bool IsActive;
	private Vector2 lastDir;
	private bool hasDirection;

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

		if (currentMagnitudeIsTooSmall())
		{
			if (IsActive)
			{
				IsActive = false;
				OnInactive?.Invoke(this);
			}

			RightPressed = false;
			LeftPressed = false;
			DownPressed = false;
			UpPressed = false;
			return;
		}

		if (!IsActive)
		{
			IsActive = true;
			OnActive?.Invoke(this);
			return;
		}

		if (currentDir.x > 0)
		{
			LeftPressed = false;
			if (RightPressed) return;
			RightPressed = true;
			OnRight?.Invoke(this);
		}
		else if (currentDir.x < 0)
		{
			RightPressed = false;
			if (LeftPressed) return;
			LeftPressed = true;
			OnLeft?.Invoke(this);
		}

		if (currentDir.y > 0)
		{
			DownPressed = false;
			if (UpPressed) return;
			UpPressed = true;
			OnUp?.Invoke(this);
		}
		else if (currentDir.y < 0)
		{
			UpPressed = false;
			if (DownPressed) return;
			DownPressed = true;
			OnDown?.Invoke(this);
		}
	}

	private bool currentMagnitudeIsTooSmall() => !(currentDir.magnitude > .5f);

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