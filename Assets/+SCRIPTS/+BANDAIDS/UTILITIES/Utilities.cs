using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
	public static class Utilities
	{
		public static List<Vector2> GetPositionsWithinRadiusOfPoint(int numberOfPositions, Vector2 basePosition, float radius = 1)
		{
			var positions = new List<Vector2>();

			for (var i = 0; i < numberOfPositions; i++)
			{
				var angle = i * (360f / numberOfPositions) * Mathf.Deg2Rad;
				var offset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
				positions.Add(basePosition + offset);
			}

			return positions;
		}

		public static GameObject MakePrefab(GameObject gridPrefab, Vector3 position)
		{
			if (gridPrefab == null) return null;

			var assetPath = AssetDatabase.GetAssetPath(gridPrefab);
			if (string.IsNullOrEmpty(assetPath)) return null;

			var instance = (GameObject) PrefabUtility.InstantiatePrefab(gridPrefab);
			if (instance == null)
			{
				return null;
			}

			instance.transform.position = position;
			return instance;
		}
	}
}
