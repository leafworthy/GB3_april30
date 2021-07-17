using System;

public interface IPlayerMenuControl
{
	event Action<Player> MenuMoveRight;
	event Action<Player> MenuMoveLeft;
	event Action<Player> MenuMoveUp;
	event Action<Player> MenuMoveDown;
	event Action<Player> MenuPressA;
	event Action<Player> MenuPressB;
	event Action<Player> MenuPressPause;
	void UpdateButtons();
}
