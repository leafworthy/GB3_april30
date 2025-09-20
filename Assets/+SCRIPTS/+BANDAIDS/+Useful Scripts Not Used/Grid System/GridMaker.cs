using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace __SCRIPTS
{
	public class GridMaker : MonoBehaviour
	{
		public GameObject gridPrefab;
		public int gridWidth = 10;
		public int gridHeight = 10;
		public float cellSizeX = 1f;
		public float cellSizeY = 1f;
		private List<GameObject> gridCells = new();
		public Vector2 offset;

#if UNITY_EDITOR
		[Button("Make Grid")]
		public void MakeGrid()
		{
			if (gridPrefab == null) return;

			ClearGrid();

			for (var y = 0; y < gridHeight; y++)
			{
				for (var x = 0; x < gridWidth; x++)
				{
					var isoX = (x - y) * cellSizeX;
					var isoY = (x + y) * (cellSizeY * 0.5f);

					var position = new Vector3(isoX + offset.x, isoY + offset.y, 0);
					var gridCell = MakePrefab(position);
					if (gridCell != null)
					{
						Undo.RegisterCreatedObjectUndo(gridCell, "Create Grid Cell");
						gridCell.transform.SetParent(transform);
						gridCells.Add(gridCell);
					}
				}
			}
		}

		private GameObject MakePrefab(Vector3 position)
		{
			if (gridPrefab == null) return null;

			// Check if the prefab is an asset
			var assetPath = AssetDatabase.GetAssetPath(gridPrefab);
			if (string.IsNullOrEmpty(assetPath)) return null;

			var instance = (GameObject) PrefabUtility.InstantiatePrefab(gridPrefab);
			if (instance == null) return null;

			instance.transform.position = position;
			return instance;
		}

#endif

		private void ClearGrid()
		{
#if UNITY_EDITOR
			var attempts = 0;
			var maxAttempts = 1000;

			while (transform.childCount > 0 && attempts < maxAttempts)
			{
				var child = transform.GetChild(0).gameObject;
				if (child != null) Undo.DestroyObjectImmediate(child);
				attempts++;
			}

			#else
            foreach (GameObject cell in gridCells)
            {
                if (cell != null)
                {
                    Destroy(cell);
                }
            }
#endif
			gridCells.Clear();
		}
	}
}
