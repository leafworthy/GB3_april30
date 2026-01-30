using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public static class MyExtensions
	{
		public static List<GameObject> FindEnemyPlacersWithinCollider(Collider2D collider, List<PrefabPlacer> candidates = null)
		{
			var results = new List<GameObject>();

			if (collider == null)
				return results;

			foreach (var go in candidates)
			{
				if (!go.gameObject.activeInHierarchy)
					continue;

				Vector2 pos = go.transform.position;
				if (collider.OverlapPoint(pos)) results.Add(go.gameObject);
			}

			return results;
		}

		public static bool IsObjectWithinCollider(Collider2D collider, GameObject ObjectToFind)
		{
			Vector2 pos = ObjectToFind.transform.position;
			return collider.OverlapPoint(pos);
		}

		public static void RemoveChildren(this Transform transform)
		{
			for (var i = transform.childCount - 1; i >= 0; i--)
			{
				if (!Application.isPlaying) Object.DestroyImmediate(transform.GetChild(i).gameObject);
				else Object.Destroy(transform.GetChild(i).gameObject);
			}
		}

		public static void RemoveNulls<T>(this List<T> list) where T : Object
		{
			list.RemoveAll(item => item == null);
		}

		public static T GetRandom<T>(this List<T> list) where T : Object => list.Count == 0 ? null : list[Random.Range(0, list.Count)];

		public static void PlayRandomAt(this List<AudioClip> list, Vector3 position)
		{
			Services.sfx.PlayRandomAt(list, position);
		}

		public static void PlayRandom(this List<AudioClip> list)
		{
			Services.sfx.PlayRandom(list.GetRandom());
		}

		public static Vector2 GetRandomPointInside(this Collider2D col, int maxAttempts = 100)
		{
			if (col == null) return Vector2.zero;

			var bounds = col.bounds;

			for (var i = 0; i < maxAttempts; i++)
			{
				var randomPoint = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));

				if (col.OverlapPoint(randomPoint))
					return randomPoint;
			}

			return Vector2.zero;
		}
	}
}
