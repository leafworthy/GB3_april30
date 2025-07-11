using UnityEngine;

namespace __SCRIPTS.RisingText
{
	public class RisingTextCreator : ServiceUser, IService
	{
		public  void CreateRisingText(string textToRise, Vector2 position, Color textColor)
		{
			var risingTextGameObject = objectMaker.Make( assets.FX.risingTextPrefab, position);
			var risingTextScript = risingTextGameObject.GetComponent<RisingText>();
			risingTextScript.RiseWithText(textToRise, textColor);
		}

		//REQUIRES ObjectMaker
		public void StartService()
		{
		}
	}
}
