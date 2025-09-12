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
			if (gridPrefab == null)
			{
				Debug.LogError("GridMaker: gridPrefab is null.");
				return null;
			}

			var assetPath = AssetDatabase.GetAssetPath(gridPrefab);
			if (string.IsNullOrEmpty(assetPath))
			{
				Debug.LogError("GridMaker: Assigned prefab is not a valid asset. Make sure you're assigning a prefab from the Project window, not the scene.");
				return null;
			}

			var instance = (GameObject) PrefabUtility.InstantiatePrefab(gridPrefab);
			if (instance == null)
			{
				Debug.LogError("GridMaker: Failed to instantiate prefab.");
				return null;
			}

			instance.transform.position = position;
			return instance;
		}
	}


}
