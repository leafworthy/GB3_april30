using UnityEngine;

 namespace RisingTextNamespace
{
	public class DestroyMeEvent : MonoBehaviour
	{
		public GameObject transformToDestroy;

		public void DestroyMe()
		{
			Maker.Unmake(transformToDestroy);
		}
	}
}