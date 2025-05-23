using UnityEngine;

[ExecuteAlways]
public class Rotator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
	    var x = this.transform.localEulerAngles.x;
       transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, x);
    }
}
