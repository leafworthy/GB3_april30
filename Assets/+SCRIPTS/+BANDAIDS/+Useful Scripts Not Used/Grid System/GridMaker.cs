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
        private List<GameObject> gridCells = new List<GameObject>();
        public Vector2 offset;

#if UNITY_EDITOR
        [Button("Make Grid")]
        public void MakeGrid()
        {
            if (gridPrefab == null)
            {
                Debug.LogError("GridMaker: gridPrefab is null.");
                return;
            }

            ClearGrid();

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    float isoX = (x - y) * cellSizeX;
                    float isoY = (x + y) * (cellSizeY * 0.5f);

                    Vector3 position = new Vector3(isoX + offset.x, isoY + offset.y, 0);
                    GameObject gridCell = MakePrefab(position);
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
            if (gridPrefab == null)
            {
                Debug.LogError("GridMaker: gridPrefab is null.");
                return null;
            }

            // Check if the prefab is an asset
            string assetPath = AssetDatabase.GetAssetPath(gridPrefab);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError(
                    "GridMaker: Assigned prefab is not a valid asset. Make sure you're assigning a prefab from the Project window, not the scene.");
                return null;
            }

            GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(gridPrefab);
            if (instance == null)
            {
                Debug.LogError("GridMaker: Failed to instantiate prefab.");
                return null;
            }

            instance.transform.position = position;
            return instance;
        }

#endif

        private void ClearGrid()
        {
#if UNITY_EDITOR
            int attempts = 0;
            int maxAttempts = 1000;
            
            while (transform.childCount > 0 && attempts < maxAttempts)
            {
                GameObject child = transform.GetChild(0).gameObject;
                if (child != null)
                {
                    Undo.DestroyObjectImmediate(child);
                }
                attempts++;
            }
            
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("GridMaker: Maximum attempts reached while clearing grid. Some children may remain.");
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
