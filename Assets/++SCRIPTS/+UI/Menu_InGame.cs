public class Menu_InGame : Menu
{

	public override void StopMenu(MENUS.Type nextMenu)
	{
		if (nextMenu == MENUS.Type.Pause)
		{
			isActive = false;
		}
		else
		{
			base.StopMenu(nextMenu);
		}
	}
}
