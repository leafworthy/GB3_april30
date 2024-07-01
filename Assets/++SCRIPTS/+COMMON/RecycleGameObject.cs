using System;
using UnityEngine;

namespace __SCRIPTS._COMMON
{
	public class RecycleGameObject : MonoBehaviour
	{
		public event Action OnActivate;
		public event Action OnCleanup; 
		public void ActivateGameObject()
		{
			gameObject.SetActive(true);
			OnActivate?.Invoke();
		}

		public void DeactivateGameObject()
		{
			OnCleanup?.Invoke();
			gameObject.SetActive(false);
		}


	}
}


