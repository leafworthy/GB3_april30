using UnityEngine;

namespace __SCRIPTS
{
	public class ColorSchemeHandler: MonoBehaviour
	{
		public HouseColorScheme houseColors;


		public Color GetColor(HouseColorScheme.ColorType type)
		{
			return houseColors.GetColor(type);
		}
	}
}