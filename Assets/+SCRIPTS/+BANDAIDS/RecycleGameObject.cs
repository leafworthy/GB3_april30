using UnityEngine;
using GangstaBean.Core;

namespace __SCRIPTS
{
	public class RecycleGameObject : MonoBehaviour
	{
		public void ActivateGameObject()
		{
			// Reset all poolable components when activating
			var poolables = GetComponentsInChildren<IPoolable>();
			foreach (var poolable in poolables)
			{
				poolable.OnPoolSpawn();
			}
			
			gameObject.SetActive(true);
		}

		public void DeactivateGameObject()
		{
			// Notify poolable components when deactivating
			var poolables = GetComponentsInChildren<IPoolable>();
			foreach (var poolable in poolables)
			{
				poolable.OnPoolDespawn();
			}
			
			gameObject.SetActive(false);
		}
	}
}



