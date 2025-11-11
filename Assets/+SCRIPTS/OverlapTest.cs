using Sirenix.OdinInspector;
using UnityEngine;

public class OverlapTest : MonoBehaviour
{
	[SerializeField] private Collider2D otherA;
	[SerializeField] private Collider2D otherB;

	private Collider2D myCollider => mainPlayerGO.GetComponent<Collider2D>();


	public GameObject mainPlayerGO;
	private float lastCheckTime;
	public bool checkContinuously = true;
	private float checkInterval = .5f;

	private void Update()
	{
		if (!checkContinuously) return;

		// Check at specified intervals to optimize performance
		if (Time.time - lastCheckTime >= checkInterval)
		{
			CheckOverlap();
			lastCheckTime = Time.time;
		}
	}


	private void CheckOverlap()
	{
		mainPlayerGO = Services.playerManager?.mainPlayer?.SpawnedPlayerGO;
		if (mainPlayerGO == null)
		{
			Debug.Log("can't find");
			return;
		}
		ContactFilter2D filter = new ContactFilter2D().NoFilter();
		//filter.SetLayerMask(layer);
		//filter.useTriggers = true;
		Collider2D[] results = new Collider2D[25];
		int count = myCollider.Overlap(filter, results);

		bool foundA = false, foundB = false;
		for (int i = 0; i < count; i++)
		{
			if (results[i] == otherA) foundA = true;
			if (results[i] == otherB) foundB = true;
		}

		Debug.Log("foundA: " + foundA + ", foundB: " + foundB);

		if (foundA && foundB)
		{
			Debug.Log("Overlapping both colliders!");
		}
	}
}
