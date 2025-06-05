using UnityEngine;

namespace __SCRIPTS
{
	public class RecycleGameObject : MonoBehaviour
	{
		public void ActivateGameObject()
		{
			gameObject.SetActive(true);
		}

		public void DeactivateGameObject()
		{
			gameObject.SetActive(false);
		}


	}
}



