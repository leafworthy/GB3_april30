using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuKeyboardControl : IPlayerMenuControl
{
	private Player owner;
	private List<KeyboardKey> keys = new List<KeyboardKey>();
	public event Action<Player> MenuPressPause;
	public event Action<Player> MenuMoveRight;
	public event Action<Player> MenuMoveLeft;
	public event Action<Player> MenuMoveUp;
	public event Action<Player> MenuMoveDown;
	public event Action<Player> MenuPressA;
	public event Action<Player> MenuPressB;

	public void UpdateButtons()
	{
		foreach (var keyboardKey in keys)
		{
			keyboardKey.update();
		}
	}

	public PlayerMenuKeyboardControl(Player player)
	{
		owner = player;
		var keySetup = player.keyboardControlSetup;

		keySetup.MoveUpKey.OnPress += MoveUpPress;
		keys.Add(keySetup.MoveUpKey);

		keySetup.MoveRightKey.OnPress += MoveRightPress;
		keys.Add(keySetup.MoveRightKey);

		keySetup.MoveDownKey.OnPress += MoveDownPress;
		keys.Add(keySetup.MoveDownKey);

		keySetup.MoveLeftKey.OnPress += MoveLeftPress;
		keys.Add(keySetup.MoveLeftKey);

		keySetup.Attack1Key.OnPress += Attack1Press;
		keys.Add(keySetup.Attack1Key);


		keySetup.JumpKey.OnPress += Jump_Press;
		keys.Add(keySetup.JumpKey);

		keySetup.PauseKey.OnPress += Pause_Press;
		keys.Add(keySetup.PauseKey);
	}

	private void Jump_Press()
	{
		MenuPressA?.Invoke(owner);
	}

	private void Pause_Press()
	{
		MenuPressPause?.Invoke(owner);
	}

	private void B_Press()
	{
		MenuPressB?.Invoke(owner);
	}

	private void MoveRightPress()
	{
		MenuMoveRight?.Invoke(owner);
	}

	private void MoveDownPress()
	{
		MenuMoveDown?.Invoke(owner);
	}

	private void MoveLeftPress()
	{
		MenuMoveLeft?.Invoke(owner);
	}

	private void Attack1Press()
	{
		MenuPressA?.Invoke(owner);
	}

	private void MoveUpPress()
	{
		MenuMoveUp?.Invoke(owner);
	}
}
