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
			if (transformToDestroy == null) return;
			Services.objectMaker.Unmake(transformToDestroy.gameObject);
		}
	}
}
