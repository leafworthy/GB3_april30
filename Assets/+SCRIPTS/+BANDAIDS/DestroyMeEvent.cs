using UnityEngine;

namespace __SCRIPTS
{
	public class DestroyMeEvent : ServiceUser
	{
		public GameObject transformToDestroy;
		public void DestroyMe()
		{
			if (transformToDestroy == null) return;
			objectMaker.Unmake(transformToDestroy.gameObject);
		}
	}
}
