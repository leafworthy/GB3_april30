using UnityEngine;

namespace GangstaBean.UI.RisingText
{
	public class RisingTextCreator : Singleton<RisingTextCreator>
	{
		public static void CreateRisingText(string textToRise, Vector2 position, Color textColor)
		{
			var risingText = ObjectMaker.I.Make(ASSETS.FX.risingTextPrefab, position);
			var risingTextScript = risingText.GetComponent<RisingText>();
			risingTextScript.RiseWithText(textToRise, textColor);
		}
	}
}