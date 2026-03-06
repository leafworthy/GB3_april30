using __SCRIPTS;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : Singleton<CameraSwitcher>
{
	public CinemachineCamera MainFollowCamera;
	public CinemachineCamera currentArenaCamera;


	[RuntimeInitializeOnLoadMethod]
	static void ResetStatics()
	{
		_instance = null;
	}

	public void SoloCamera(CinemachineCamera _cam)
	{
		if (_cam == null) return;
		Debug.Log("camera soloed: " + _cam.name);
		MainFollowCamera.Priority = 0;
		currentArenaCamera = _cam;
		currentArenaCamera.Priority = 10;
	}


	public void UnSoloCamera()
	{
		MainFollowCamera.Priority = 10;
		if (currentArenaCamera == null) return;
		currentArenaCamera.Priority = 0;
	}
}
