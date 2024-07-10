using UnityEngine;

public class DestroyMeEvent : MonoBehaviour
{
	public GameObject transformToDestroy;
	public void DestroyMe()
	{
		if (transformToDestroy == null) return;
		Maker.Unmake(transformToDestroy.gameObject);
	}
}