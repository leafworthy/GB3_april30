using __SCRIPTS;
using Unity.Cinemachine;

public class CameraSwitcher : Singleton<CameraSwitcher>
{
	public CinemachineCamera MainFollowCamera;
	public CinemachineCamera currentArenaCamera;
	bool isInArena;


	public CinemachineCamera GetCurrentCamera() => isInArena? currentArenaCamera : MainFollowCamera;

	public void SoloCamera(CinemachineCamera _cam)
	{
		if (_cam == null) return;
		isInArena = true;
		MainFollowCamera.Priority = 0;
		currentArenaCamera = _cam;
		currentArenaCamera.Priority = 10;
	}


	public void UnSoloCamera()
	{
		isInArena = false;
		MainFollowCamera.Priority = 10;
		if (currentArenaCamera == null) return;
		currentArenaCamera.Priority = 0;
	}
}
