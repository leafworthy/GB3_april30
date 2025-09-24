using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Collider2DExtensions
{
	/// <summary>
	/// Finds all GameObjects whose positions are within the given Collider2D.
	/// </summary>
	public static List<GameObject> FindEnemyPlacersWithinCollider(Collider2D collider, List<EnemyPlacer> candidates = null)
	{
		var results = new List<GameObject>();

		if (collider == null)
			return results;


		foreach (var go in candidates)
		{
			if (!go.gameObject.activeInHierarchy)
				continue;

			Vector2 pos = go.transform.position;
			if (collider.OverlapPoint(pos))
			{
				results.Add(go.gameObject);
			}
		}

		return results;
	}

	public static bool IsObjectWithinCollider(Collider2D collider, GameObject ObjectToFind)
	{
		Vector2 pos = ObjectToFind.transform.position;
		return collider.OverlapPoint(pos);
	}
}
