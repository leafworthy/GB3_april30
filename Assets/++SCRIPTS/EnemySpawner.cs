using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> spawnPoints;
    public int rate;
    private int counter;
    private List<GameObject> enemyPrefabs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        enemyPrefabs.Add(ASSETS.CharacterPrefabs.ConeEnemyPrefab);
        enemyPrefabs.Add(ASSETS.CharacterPrefabs.ToastEnemyPrefab);
        enemyPrefabs.Add(ASSETS.CharacterPrefabs.DonutEnemyPrefab);
    }

    private void SpawnEnemy(GameObject prefab)
    {
        var newEnemy = MAKER.Make(prefab, spawnPoints.GetRandom().transform.position);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        counter++;
        if (counter > rate)
        {
            counter = 0;
            SpawnEnemy(enemyPrefabs.GetRandom());
        }
    }
}
