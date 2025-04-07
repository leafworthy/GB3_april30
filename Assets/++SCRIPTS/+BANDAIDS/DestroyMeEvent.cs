using UnityEngine;

namespace __SCRIPTS
{
	public class DestroyMeEvent : MonoBehaviour
	{
		public GameObject transformToDestroy;
		public void DestroyMe()
		{
			if (transformToDestroy == null) return;
			ObjectMaker.I.Unmake(transformToDestroy.gameObject);
			Debug.Log("destroyed");
		}
	}
}