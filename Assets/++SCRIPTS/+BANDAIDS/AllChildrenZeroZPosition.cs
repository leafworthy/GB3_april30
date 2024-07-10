using UnityEngine;

[ExecuteInEditMode]
public class AllChildrenZeroZPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnEnable()
    {
        foreach (Transform t in transform)
        {
            t.position = new Vector3(t.position.x, t.position.y, 0);
        }
    }
}