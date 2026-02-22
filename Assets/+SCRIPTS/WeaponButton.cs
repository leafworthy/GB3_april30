using __SCRIPTS;
using UnityEngine;

public class WeaponButton : MonoBehaviour
{
	HideRevealObjects buttonObjects => _buttonObjects ??= GetComponentInChildren<HideRevealObjects>();
	HideRevealObjects _buttonObjects;

	public enum buttons
	{
		R1,
		R2,
		L1,
		L2,
		Circle,
		Square
	}

	public void Set(buttons index)
	{
		switch (index)
		{
			case buttons.R1:
				buttonObjects.Set((int) buttons.R1);
				break;
			case buttons.R2:
				buttonObjects.Set((int) buttons.R2);
				break;
			case buttons.L1:
				buttonObjects.Set((int) buttons.L1);
				break;
			case buttons.L2:
				buttonObjects.Set((int) buttons.L2);
				break;
			case buttons.Circle:
				buttonObjects.Set((int) buttons.Circle);
				break;
			case buttons.Square:
				buttonObjects.Set((int) buttons.Square);
				break;
		}
	}
}
