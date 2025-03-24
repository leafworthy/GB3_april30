using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VInspector;

public class GridMaker : MonoBehaviour
{
	public GameObject gridPrefab;
	public int gridWidth = 10;
	public int gridHeight = 10;
	public float cellSizeX = 1f;
	public float cellSizeY = 1f;
	private List<GameObject> gridCells = new List<GameObject>();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	[Button()]
	public void MakeGrid()
	{
		ClearGrid();

		// Fill grid from left to right and bottom to top in isometric space
		for (int y = 0; y < gridHeight; y++)
		{
			for (int x = 0; x < gridWidth; x++)
			{
				// Calculate isometric position
				// This transforms grid coordinates to isometric screen coordinates
				// where x-axis runs diagonally from bottom-left to top-right
				// and y-axis runs diagonally from bottom-right to top-left
				float isoX = (x - y) * cellSizeX;
				float isoY = (x + y) * (cellSizeY * 0.5f);

				Vector3 position = new Vector3(isoX, isoY, 0);
				GameObject gridCell = MakePrefab(position);
				gridCell.transform.SetParent(transform);
				gridCells.Add(gridCell);
			}
		}
	}

	#if UNITY_EDITOR
	private GameObject MakePrefab(Vector3 position)
	{ // Get the Path to Prefab
		string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gridPrefab);

		// Load Prefab Asset as Object from path
		Object _newObject = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));

		//Instantiate the Prefab in the scene, as a child of the GO this script runs on
		GameObject _newPrefabInstance = PrefabUtility.InstantiatePrefab(_newObject, this.transform) as GameObject;
		_newPrefabInstance.transform.position = position;
	 return _newPrefabInstance;
	}	
	#endif

	private void ClearGrid()
	{
#if UNITY_EDITOR
		while (transform.childCount > 0)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}
#else
		foreach (GameObject cell in gridCells)
		{

            Destroy(cell);
#endif
		gridCells.Clear();
		}

		
	}

