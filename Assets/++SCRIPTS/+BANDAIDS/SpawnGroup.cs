using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnGroup : MonoBehaviour
{
	public List<GameObject> spawnPoints;

	void OnValidate()
	{
		spawnPoints.Clear();
		foreach (Transform child in transform)
		{
			if (child != transform)
			{
				spawnPoints.Add(child.gameObject);
			}
		}
	}
}
