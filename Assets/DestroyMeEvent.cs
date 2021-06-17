using System.Collections;
using System.Collections.Generic;
using _SCRIPTS;
using UnityEngine;

public class DestroyMeEvent : MonoBehaviour
{
	public GameObject transform;
	public void DestroyMe()
	{
		MAKER.Unmake(transform);
	}
}
