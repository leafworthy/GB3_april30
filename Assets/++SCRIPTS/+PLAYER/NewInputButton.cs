using System;
using UnityEngine.InputSystem;

public class NewInputButton : NewControlButton
{
	private InputAction action;
	public Player owner { get; set; }
	public event Action<NewControlButton> OnPress;
	public event Action<NewControlButton> OnHold;
	public event Action<NewControlButton> OnRelease;
	public bool IsPressed { get; set; }

	public NewInputButton(InputAction _action, Player _owner)
	{
		action = _action;
		owner = _owner;
		owner.input.onActionTriggered += owner_onAction;
	}

	private void owner_onAction(InputAction.CallbackContext obj)
	{
		if (obj.action.name != action.name) return;
		if (obj.phase == InputActionPhase.Performed)
		{

			if (!IsPressed)
			{
				IsPressed = true;
				OnPress?.Invoke(this);
			}
			else
			{
				OnHold?.Invoke(this);
			}
		}
		else if (obj.phase == InputActionPhase.Canceled)
		{
			if (!IsPressed) return;
			IsPressed = false;
			OnRelease?.Invoke(this);
		}
	}


	public void update()
	{

	}
}