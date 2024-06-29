using UnityEngine;
using UnityEngine.Serialization;

public class Cameras : Singleton<Cameras>
{


	[SerializeField] private  Camera currentCamera;

	public static void SetMenuCameraActive(bool on)
	{
	}



	public static Camera GetCamera()
	{
		if(I.currentCamera == null)
			I.currentCamera = Camera.main;
		return I.currentCamera;
	}

}