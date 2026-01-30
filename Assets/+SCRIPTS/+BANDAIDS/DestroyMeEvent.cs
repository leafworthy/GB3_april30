using UnityEngine;

namespace __SCRIPTS
{
	public class DestroyMeEvent : MonoBehaviour
	{
		public GameObject transformToDestroy;
		public bool disableDestroy = false;

		public void DestroyMe()
		{
			if (disableDestroy) return;
			var objectMaker = ServiceLocator.Get<ObjectMaker>();
			if (transformToDestroy == null) return;
			objectMaker.Unmake(transformToDestroy.gameObject);
		}
	}
}
