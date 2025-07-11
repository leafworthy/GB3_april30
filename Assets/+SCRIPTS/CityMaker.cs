using System.Collections.Generic;
using __SCRIPTS;
using __SCRIPTS.Plugins._ISOSORT;
using UnityEngine;
using VInspector;
using UnityEditor;

public class CityMaker : MonoBehaviour
{
    private GridMaker gridMaker;
    private GridCulling gridCulling;

    public GameObject GB_House;
    public GameObject GasStation;
    public GameObject Graveyard;
    public GameObject RichHouse;

    public List<GameObject> houses;

    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSizeX = 336;
    public float cellSizeY = 335.75f;
    public Vector2 offset = new Vector2(465,-24.1f);

    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
    public List<GameObject> placedHouses = new List<GameObject>();

#if UNITY_EDITOR
    [Button("Generate City")]
    public void GenerateCity()
    {
        if (!ValidateHousePrefabs())
        {
            Debug.LogError("CityMaker: Missing required house prefabs!");
            return;
        }

        EnsureIsoSpriteSortingManager();
        ClearCity();
        PlaceSpecialHouses();
        FillWithRandomHouses();

        Debug.Log($"City generated with {placedHouses.Count} houses!");
    }

    [Button("Clear City")]
    public void ClearCity()
    {
        foreach (GameObject house in placedHouses)
        {
            if (house != null)
            {
                Undo.DestroyObjectImmediate(house);
            }
        }

        placedHouses.Clear();
        occupiedPositions.Clear();
    }
#endif

    private bool ValidateHousePrefabs()
    {
        return GB_House != null && GasStation != null && Graveyard != null && RichHouse != null;
    }

    private void PlaceSpecialHouses()
    {
        List<GameObject> specialHouses = new List<GameObject> { GB_House, GasStation, Graveyard, RichHouse };

        foreach (GameObject specialHouse in specialHouses)
        {
            Vector2Int gridPos = GetRandomEmptyPosition();
            if (gridPos != Vector2Int.one * -1)
            {
                PlaceHouseAtGridPosition(specialHouse, gridPos);
            }
        }
    }

    private void FillWithRandomHouses()
    {
        if (houses == null || houses.Count == 0)
        {
            Debug.LogWarning("CityMaker: No random houses available!");
            return;
        }

        int maxHouses = Mathf.Min(gridWidth * gridHeight, 50);
        int housesToPlace = maxHouses - placedHouses.Count;

        Debug.Log($"CityMaker: Placing {housesToPlace} random houses...");

        for (int i = 0; i < housesToPlace; i++)
        {
            Vector2Int gridPos = GetRandomEmptyPosition();
            if (gridPos == Vector2Int.one * -1)
            {
                Debug.Log($"CityMaker: No more empty positions available after placing {i} houses");
                break;
            }

            GameObject randomHouse = houses[Random.Range(0, houses.Count)];
            PlaceHouseAtGridPosition(randomHouse, gridPos);

            if (i % 10 == 0)
            {
                Debug.Log($"CityMaker: Placed {i + 1} houses so far...");
            }
        }
    }

    private Vector2Int GetRandomEmptyPosition()
    {
        if (occupiedPositions.Count >= gridWidth * gridHeight)
        {
            return Vector2Int.one * -1;
        }

        int attempts = 0;
        int maxAttempts = 100;

        while (attempts < maxAttempts)
        {
            int x = Random.Range(0, gridWidth);
            int y = Random.Range(0, gridHeight);
            Vector2Int position = new Vector2Int(x, y);

            if (!occupiedPositions.Contains(position))
            {
                return position;
            }

            attempts++;
        }

        return Vector2Int.one * -1;
    }

    private void PlaceHouseAtGridPosition(GameObject housePrefab, Vector2Int gridPosition)
    {
        float isoX = (gridPosition.x - gridPosition.y) * cellSizeX;
        float isoY = (gridPosition.x + gridPosition.y) * (cellSizeY * 0.5f);
        Vector3 worldPosition = new Vector3(isoX + offset.x, isoY + offset.y, 0);

        GameObject house = InstantiateHouse(housePrefab, worldPosition);
        if (house != null)
        {
            occupiedPositions.Add(gridPosition);
            placedHouses.Add(house);
            house.transform.SetParent(transform);
        }
    }

    private GameObject InstantiateHouse(GameObject prefab, Vector3 position)
    {
#if UNITY_EDITOR
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("CityMaker: Assigned prefab is not a valid asset.");
            return null;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance != null)
        {
            instance.transform.position = position;
            Undo.RegisterCreatedObjectUndo(instance, "Create House");
        }
        return instance;
#else
        return Instantiate(prefab, position, Quaternion.identity);
#endif
    }

    private void EnsureIsoSpriteSortingManager()
    {
        // Check if IsoSpriteSortingManager already exists in the scene
        if (IsoSpriteSortingManager.I == null)
        {
            // Create a new GameObject for the IsoSpriteSortingManager
            GameObject managerGO = new GameObject("IsoSpriteSortingManager");
            managerGO.AddComponent<IsoSpriteSortingManager>();
            
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(managerGO, "Create IsoSpriteSortingManager");
#endif
            Debug.Log("CityMaker: Created IsoSpriteSortingManager for city generation");
        }
        else
        {
            Debug.Log("CityMaker: IsoSpriteSortingManager already exists");
        }
    }
}
