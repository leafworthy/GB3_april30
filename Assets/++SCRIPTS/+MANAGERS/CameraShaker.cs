using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShaker:Singleton<CameraShaker>
{
	private static CinemachineImpulseSource cinemachineImpulseSource;
	private static Vector3 shakeVector = new Vector3(3, 3, 0);
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
		cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
		CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
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
		cinemachineImpulseSource.GenerateImpulseWithForce(10);
		if (!isOn) return;
	}
}
