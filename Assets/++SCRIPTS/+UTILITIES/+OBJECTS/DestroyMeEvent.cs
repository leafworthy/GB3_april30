using UnityEngine;

namespace GangstaBean.Utilities.Objects
{
	public class DestroyMeEvent : MonoBehaviour
	{
		public GameObject transformToDestroy;
		public void DestroyMe()
		{
			if (transformToDestroy == null) return;
			ObjectMaker.I.Unmake(transformToDestroy.gameObject);
		}
	}
}