using UnityEngine;

namespace __SCRIPTS.RisingText
{
	public class RisingTextCreator : MonoBehaviour, IService
	{
		public  void CreateRisingText(string textToRise, Vector2 position, Color textColor, float fontSize = 3)
		{
			Debug.Log("Rising text: " + textToRise);
			var risingTextGameObject = Services.objectMaker.Make(Services.assetManager.FX.risingTextPrefab, position);
			var risingTextScript = risingTextGameObject.GetComponent<RisingText>();
			risingTextScript.RiseWithText(textToRise, textColor, fontSize);
		}

		//REQUIRES ObjectMaker
		public void StartService()
		{
		}
	}
}
