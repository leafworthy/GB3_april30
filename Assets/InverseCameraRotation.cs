using UnityEngine;
using VInspector;

public class InverseCameraRotation : MonoBehaviour
{
	public Vector3 cameraEuler = new Vector3(30f, 45f, 0f);

	[Button()]
	void Start()
	{
		Quaternion inverse = Quaternion.Inverse(Quaternion.Euler(cameraEuler));
		transform.rotation = inverse;
	}
}
