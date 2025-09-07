using UnityEngine;
using VInspector;

namespace __SCRIPTS
{

[ExecuteAlways]
	public class ColorSchemeHandler: MonoBehaviour
	{
		private void Awake()
		{
			RandomPalette();
		}

		public HouseColorScheme houseColors;

		[Button]
		public void RandomPalette()
		{
			HouseColorScheme[] allSchemes = Resources.LoadAll<HouseColorScheme>("ColorPalettes");


			int randomIndex = Random.Range(0, allSchemes.Length);
			houseColors = allSchemes[randomIndex];
			Debug.Log($"Randomly selected color scheme: {houseColors.name}");
		}
	}
}
