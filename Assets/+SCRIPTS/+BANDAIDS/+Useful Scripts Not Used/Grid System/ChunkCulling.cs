using System.Collections.Generic;
using UnityEngine;

public class ChunkCulling : MonoBehaviour
{

    public List<GameObject> chunks;
    public Camera cam;
    public Vector2 camSize;
    private void Update()
    {
        Vector3 camPos = cam.transform.position;
        foreach (GameObject chunk in chunks)
        {
            if (chunk != null)
            {
                Vector3 chunkPos = chunk.transform.position;
                float distance = Vector3.Distance(camPos, chunkPos);

                if (distance > camSize.magnitude)
                {
                    chunk.SetActive(false);
                }
                else
                {
                    chunk.SetActive(true);
                }
            }
        }
    }
}
