using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public static class MyExtensions
	{

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
			if (col == null)
			{
				Debug.LogWarning("[Collider2DExtensions] Null collider passed to GetRandomPointInside.");
				return Vector2.zero;
			}

			Bounds bounds = col.bounds;

			for (int i = 0; i < maxAttempts; i++)
			{
				Vector2 randomPoint = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));

				if (col.OverlapPoint(randomPoint))
					return randomPoint;
			}

			Debug.LogWarning($"[Collider2DExtensions] Failed to find a point inside {col.name} after {maxAttempts} attempts.");
			return Vector2.zero;
		}


	}
}
