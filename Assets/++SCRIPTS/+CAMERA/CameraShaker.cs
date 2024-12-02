using System;
using Cinemachine;
using UnityEngine;
using Random = System.Random;

public class CameraShaker: Singleton<CameraShaker>
{
	private static CinemachineImpulseSource cinemachineImpulseSource;
	public float shakeMultiplier = 1;


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
		
		var shakeVector = new Vector3(UnityEngine.Random.Range(-1,1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
		cinemachineImpulseSource.GenerateImpulseAtPositionWithVelocity(shakePosition, shakeVector* I.shakeMultiplier);
	}
}