using Unity.Cinemachine;
using UnityEngine;

public class CrimsonCameraPusher : MonoBehaviour
{
	public CinemachineCamera cam;
	public Vector3 speed;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    cam.transform.position += speed;
    }
}
