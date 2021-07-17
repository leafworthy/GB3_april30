using UnityEngine;

public class DestroyMeEvent : MonoBehaviour
{
	public GameObject transformToDestroy;
	public void DestroyMe()
	{
		MAKER.Unmake(transformToDestroy);
	}
}
