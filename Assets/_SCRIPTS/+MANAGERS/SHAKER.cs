using System;
using Cinemachine;
using UnityEngine;

namespace _SCRIPTS
{
	public class SHAKER : Singleton<SHAKER>
	{
		private CinemachineImpulseSource shaker;
		private Vector3 shakeVector = new Vector3(1, 1, 0);
		private static bool isOn = true;

		private void Start()
		{
			GAME.OnGameEnd += CleanUp;
			CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
		}

		private void CleanUp()
		{
			isOn = false;
		}

		public static void ShakeCamera( Vector3 shakePosition,float shakeMagnitude)
		{
			if (!isOn) return;
			if (I.shaker is null)
			{
				I.shaker = I.GetComponent<CinemachineImpulseSource>();
			}

			Debug.Log("trying to make shake");
			I.shaker.GenerateImpulseAt(shakePosition, I.shakeVector*shakeMagnitude);
		}
	}
}
