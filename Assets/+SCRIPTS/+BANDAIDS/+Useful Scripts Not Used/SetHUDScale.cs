using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class SetHUDScale : MonoBehaviour
{
   public List<Transform> hudElements; // List of HUD elements to scale
   public float hudScale;
    // Update is called once per frame

    [Button]
    void SetScale()
    {
		foreach (Transform element in hudElements)
		{
			if (element != null)
			{
				element.localScale = new Vector3(hudScale, hudScale, hudScale);
			}
		}
    }
}
