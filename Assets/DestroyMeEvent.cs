using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMeEvent : MonoBehaviour
{
	public GameObject transformToDestroy;
	public void DestroyMe()
	{
		MAKER.Unmake(transformToDestroy);
	}
}
