using System;
using Cinemachine;
using UnityEngine;

public class SHAKER:Singleton<SHAKER>
{
	private static CinemachineImpulseSource cinemachineImpulseSource;
	private static Vector3 shakeVector = new Vector3(1, 1, 0);
	private static float MaxShakeMagnitude = 10;
	private static bool isOn = true;

	public enum ShakeIntensityType
	{
		low,
		normal,
		high
	}
	private void Start()
	{
		LEVELS.OnLevelStop += CleanUp;
		cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
		CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
	}

	private void CleanUp()
	{
		isOn = false;
	}

	public static void ShakeCamera(Vector3 shakePosition,ShakeIntensityType type)
	{
		switch (type)
		{
			case ShakeIntensityType.low:
				ShakeCamera(shakePosition, 1);
				break;
			case ShakeIntensityType.normal:
				ShakeCamera(shakePosition, 2);
				break;
			case ShakeIntensityType.high:
				ShakeCamera(shakePosition, 4);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	private static void ShakeCamera( Vector3 shakePosition,float shakeMagnitude)
	{
		shakeMagnitude = Mathf.Min(shakeMagnitude, MaxShakeMagnitude);
		if (!isOn) return;
		cinemachineImpulseSource.GenerateImpulseAt(shakePosition, shakeVector*shakeMagnitude);
	}
}
