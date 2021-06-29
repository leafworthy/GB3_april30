using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float globalRate = 200;
    public float radius = 1;
    public List<GameObject> prefabs;
    private float counter = 0;
    private bool on = false;
    private float totalLength;
    public List<GameObject> spawns;
    public int preSpawns = 10;
    private BoxCollider2D col;
    public float areaRandom;

    // The amount of time before spawning starts.
    public GameObject[] things;

    // Array of enemy prefabs.
    public GameObject[] places;
    void Awake()
    {
        col = GetComponent<BoxCollider2D>();

    }

    void Start()
    {
        for (int i = 0; i < preSpawns; i++)
        {

            Spawn();
        }
    }

    public void Off()
    {
        on = false;
    }

    public void On()
    {
        on = true;
    }

    void Spawn()
    {
        Vector2 randomLocation = new Vector2(Random.Range(col.bounds.min.x, col.bounds.max.x), Random.Range(col.bounds.min.y, col.bounds.max.y));

        MAKER.Make(prefabs[0], randomLocation);
    }


    public void SpawnThingsInPlaces()
    {
        int thingIndex = Random.Range(0, things.Length);
        int placeIndex = Random.Range(0, places.Length);
        MAKER.Make(things[thingIndex],
            new Vector3(places[placeIndex].transform.position.x + Random.Range(-areaRandom, areaRandom),
                places[placeIndex].transform.position.y + Random.Range(-areaRandom, areaRandom)));
    }

    void FixedUpdate()
    {
        if (on)
        {
            counter += 1 * Time.timeScale;
            if (counter >= globalRate)
            {
                Spawn();
                counter = 0;
            }
        }

    }
}
