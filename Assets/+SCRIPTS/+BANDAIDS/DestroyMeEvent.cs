using UnityEngine;

namespace __SCRIPTS
{
	public class DestroyMeEvent : MonoBehaviour
	{
		public GameObject transformToDestroy;

		public void DestroyMe()
		{
			var objectMaker = ServiceLocator.Get<ObjectMaker>();
			if (transformToDestroy == null) return;
			objectMaker.Unmake(transformToDestroy.gameObject);
		}
	}
}