using UnityEngine;

public class Menu_Main : Menu
{

	public override void StartMenu()
	{
		base.StartMenu();
		PLAYERS.OnPlayerJoin += Player_OnJoin;
	}

	private void Player_OnJoin(Player player)
	{
		PLAYERS.OnPlayerJoin -= Player_OnJoin;
		player.isJoining = true;
		Debug.Log("on player join");
		MENUS.ChangeMenu(MENUS.Type.CharacterSelection);
	}
}
