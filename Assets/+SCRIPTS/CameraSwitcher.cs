using __SCRIPTS;
using Unity.Cinemachine;

public class CameraSwitcher : Singleton<CameraSwitcher>
{
	public CinemachineCamera MainFollowCamera;
	public CinemachineCamera currentArenaCamera;


	public void SoloCamera(CinemachineCamera _cam)
	{
		if (_cam == null) return;
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
