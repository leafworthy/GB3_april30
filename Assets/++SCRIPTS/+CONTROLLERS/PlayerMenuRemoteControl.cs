using System;
using _PLUGINS._INPUT.Scripts;

public class PlayerMenuRemoteControl : IPlayerMenuControl
{
	public PlayerMenuRemoteControl(Player player)
	{
		owner = player;
		playerIndex = player.data.playerIndex;
	}

	private bool A_Pressed;
	private bool B_Pressed;
	private bool Left_Pressed;
	private bool Right_Pressed;
	private bool Up_Pressed;
	private bool Down_Pressed;
	private bool Start_Pressed;

	public event Action<Player> MenuMoveRight;
	public event Action<Player> MenuMoveLeft;
	public event Action<Player> MenuMoveUp;
	public event Action<Player> MenuMoveDown;
	public event Action<Player> MenuPressA;
	public event Action<Player> MenuPressB;
	public event Action<Player> MenuPressPause;
	private PlayerIndex playerIndex;
	private Player owner;


	public void UpdateButtons()
	{
		if (GamePad.GetButton(CButton.Start, playerIndex))
		{
			if (!Start_Pressed)
			{
				Start_Pressed = true;
				MenuPressPause?.Invoke(owner);
			}
		}
		else
			B_Pressed = false;
		if (GamePad.GetButton(CButton.B, playerIndex))
		{
			if (!B_Pressed)
			{
				B_Pressed = true;
				MenuPressB?.Invoke(owner);
			}
		}
		else
			B_Pressed = false;

		if (GamePad.GetButton(CButton.A, playerIndex))
		{
			if (!A_Pressed)
			{
				A_Pressed = true;
				MenuPressA?.Invoke(owner);
			}
		}
		else
			A_Pressed = false;

		HandleLeftRightMoves();
		HandleUpDownMoves();
	}


	private void HandleLeftRightMoves()
	{
		var dpadX = GamePad.GetAxis(CAxis.LX, playerIndex);
		if (dpadX > .5)
		{
			if (!Right_Pressed)
			{
				MenuMoveRight?.Invoke(owner);
				Right_Pressed = true;
			}
		}
		else
			Right_Pressed = false;

		if (dpadX < -.5)
		{
			if (!Left_Pressed)
			{
				MenuMoveLeft?.Invoke(owner);
				Left_Pressed = true;
			}
		}
		else
			Left_Pressed = false;
	}

	private void HandleUpDownMoves()
	{
		var dpadY = GamePad.GetAxis(CAxis.LY, playerIndex);
		if (dpadY > .5)
		{
			if (!Up_Pressed)
			{
				MenuMoveUp?.Invoke(owner);
				Up_Pressed = true;
			}
		}
		else
			Up_Pressed = false;

		if (dpadY < -.5)
		{
			if (!Down_Pressed)
			{
				MenuMoveDown?.Invoke(owner);
				Down_Pressed = true;
			}
		}
		else
			Down_Pressed = false;
	}


	public void CleanUp()
	{
		A_Pressed = false;
		B_Pressed = false;
		Left_Pressed = false;
		Right_Pressed = false;
	}
}
