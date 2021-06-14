using System;
using Cinemachine;
using UnityEngine;

namespace _SCRIPTS
{
	public class SHAKER : Singleton<SHAKER>
	{
		private CinemachineImpulseSource shaker;
		private Vector3 shakeVector = new Vector3(1, 1, 0);
		private static float MaxShakeMagnitude = 10;
		private static bool isOn = true;

		public enum ShakeIntensityType
		{
			low,
			medium,
			high
		}
		private void Start()
		{
			GAME.OnGameEnd += CleanUp;
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
				case ShakeIntensityType.medium:
					ShakeCamera(shakePosition, 2);
					break;
				case ShakeIntensityType.high:
					ShakeCamera(shakePosition, 3);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		private static void ShakeCamera( Vector3 shakePosition,float shakeMagnitude)
		{
			shakeMagnitude = Mathf.Min(shakeMagnitude, MaxShakeMagnitude);

			if (!isOn) return;
			if (I.shaker is null)
			{
				I.shaker = I.GetComponent<CinemachineImpulseSource>();
			}

			I.shaker.GenerateImpulseAt(shakePosition, I.shakeVector*shakeMagnitude);
		}
	}
}
