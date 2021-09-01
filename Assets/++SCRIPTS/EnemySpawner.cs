using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> spawnPoints;
    public int rate;
    private int counter;
    private List<GameObject> enemyPrefabs = new List<GameObject>();

    bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        isOn = true;
        enemyPrefabs.Add(ASSETS.Players.ConeEnemyPrefab);
        enemyPrefabs.Add(ASSETS.Players.ToastEnemyPrefab);
        enemyPrefabs.Add(ASSETS.Players.DonutEnemyPrefab);
        LEVELS.OnLevelStop += Cleanup;
        LEVELS.OnLevelStart += Init;
    }

    private void Init(List<Player> obj)
    {
        isOn = true;
    }

    private void Cleanup()
    {
        isOn = false;
    }

    private void SpawnEnemy(GameObject prefab)
    {
        var newEnemy = MAKER.Make(prefab, spawnPoints.GetRandom().transform.position);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isOn) return;
        counter++;
        if (counter > rate)
        {
            counter = 0;
            SpawnEnemy(enemyPrefabs.GetRandom());
        }
    }
}
