using System;
using UnityEngine;

[Serializable]
public class KeyboardKey
{
	public KeyCode key;
	public event Action OnPress;
	public event Action OnRelease;
	private bool isPressed;
	public bool isAutomatic;

	public void update()
	{
		if (Input.GetKey(key))
		{

			if(isPressed && !isAutomatic) return;
			OnPress?.Invoke();
			Debug.Log("keypress " + key.ToString());
			isPressed = true;
		}
		else
		{
			if(!isPressed) return;
			OnRelease?.Invoke();
			isPressed = false;
		}
	}
}
