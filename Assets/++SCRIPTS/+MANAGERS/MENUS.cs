using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MENUS : Singleton<MENUS>
{
	private static Menu currentMenu;
	private static List<Menu> menus = new List<Menu>();
	private static GameObject MenuContainer;
	private static Camera menuCamera;

	private void Start()
	{
		menuCamera = GetComponentInChildren<Camera>();
		InitMenus();
		ChangeMenu(Type.Main);
	}

	private static void InitMenus()
	{
		MenuContainer = new GameObject("Menus");
		menus.Add(MAKER.Make(ASSETS.ui.Menu_Main).GetComponent<Menu>());
		menus.Add(MAKER.Make(ASSETS.ui.Menu_CharacterSelection).GetComponent<Menu>());
		menus.Add(MAKER.Make(ASSETS.ui.Menu_InGame).GetComponent<Menu>());
		menus.Add(MAKER.Make(ASSETS.ui.Menu_PauseMenu).GetComponent<Menu>());
		foreach (var menu in menus)
		{
			menu.gameObject.transform.SetParent(MenuContainer.transform);
			menu.DeactivateMenu();
		}
	}

	private static Menu GetMenu(Type menuType)
	{
		return menus.FirstOrDefault(t => t.menuType == menuType);
	}

	public static void ChangeMenu(Type menuType)
	{
		Debug.Log("Changing menu to " + menuType.ToString());
		if (currentMenu != null)
		{
			Debug.Log("Closing menu to " + currentMenu.menuType.ToString());
			currentMenu.StopMenu(menuType);
		}
		currentMenu = GetMenu(menuType);
		Debug.Log(currentMenu.menuType.ToString());
		if (currentMenu.menuType == Type.InGame || currentMenu.menuType == Type.Pause)
		{
			menuCamera.gameObject.SetActive(false);
		}else
		{
			menuCamera.gameObject.SetActive(true);
		}
		currentMenu.StartMenu();
	}

	public enum Type
	{
		Main,
		CharacterSelection,
		InGame,
		Pause,
		Endscreen
	}
}
