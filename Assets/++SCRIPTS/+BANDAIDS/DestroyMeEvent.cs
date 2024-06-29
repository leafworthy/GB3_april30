using ToolBox.Pools;
using UnityEngine;

public class DestroyMeEvent : MonoBehaviour
{
	public GameObject transformToDestroy;
	public void DestroyMe()
	{
		if (transformToDestroy == null) return;
		transformToDestroy.gameObject.Release();
	}
}
