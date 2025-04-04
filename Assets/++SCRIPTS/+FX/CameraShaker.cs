using System;
using __SCRIPTS.Cursor;
using Unity.Cinemachine;
using UnityEngine;

namespace __SCRIPTS
{
	public class CameraShaker: MonoBehaviour
	{
		private static CinemachineImpulseSource cinemachineImpulseSource;
		private static float shakeMultiplier = 0.3f;
		private static float maxDistanceFromCamera = 40;

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
			var distanceFromCamera =  Vector3.Distance(shakePosition, CursorManager.GetCamera().transform.position);
			if(distanceFromCamera >= maxDistanceFromCamera) return;
			var distanceRatio = distanceFromCamera / maxDistanceFromCamera;
		
			var shakeVector = new Vector3(UnityEngine.Random.Range(-1,1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
			cinemachineImpulseSource.GenerateImpulseWithVelocity(shakeVector * shakeMultiplier* shakeMagnitude*
			                                                     distanceRatio);
		}
	}
}