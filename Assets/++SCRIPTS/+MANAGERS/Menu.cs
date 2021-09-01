using UnityEngine;

public abstract class Menu : MonoBehaviour
{
	protected bool isActive = false;
	public MENUS.Type menuType;


	public virtual void StartMenu()
	{
		ActivateMenu();
	}

	public virtual void StopMenu(MENUS.Type NextMenu)
	{
		DeactivateMenu();
	}

	private void ActivateMenu()
	{
		if (isActive) return;
		isActive = true;
		gameObject.SetActive(true);
	}
	public void DeactivateMenu()
	{
		isActive = false;
		gameObject.SetActive(false);
	}
}
