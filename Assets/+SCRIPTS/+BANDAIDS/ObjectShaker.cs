using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class ObjectShaker : MonoBehaviour
	{
		private float ShakeIntensity;
		private float ShakeDecaySpeed;
		private float ShakeDuration;
		private Vector2 originalPosition;
		private Vector2 shakeFactorPosition;
		private bool hasCapturedOriginalPosition;
		private void OnDisable()
		{
			transform.localPosition = originalPosition;
			
		}

		private void OnEnable()
		{
			if (hasCapturedOriginalPosition) return;
			hasCapturedOriginalPosition = true;
			originalPosition = transform.localPosition;
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			// If shakeDuration is still running.
			if (ShakeDuration > 0)
			{
				shakeFactorPosition = (Vector2)Random.insideUnitCircle * ShakeIntensity * ShakeDuration;
				ShakeDuration -= ShakeDecaySpeed * Time.deltaTime;
				transform.localPosition = originalPosition + shakeFactorPosition;
			}
			else
			{
				transform.localPosition = originalPosition;
			}

		}

	

		public enum ShakeIntensityType
		{
			low,
			medium,
			high
		}

		public void Shake(ShakeIntensityType type)
		{

			switch (type)
			{
				case ShakeIntensityType.low:
					Shake(3f, .2f, .5f);
					break;
				case ShakeIntensityType.medium:
					Shake(4f, .3f, .5f);
					break;
				case ShakeIntensityType.high:
					Shake(5f, .4f, .5f);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		private void Shake(float intensity, float duration, float decay)
		{

		
				transform.localPosition = originalPosition;
			

			ShakeIntensity = intensity;
			ShakeDuration = duration;
			ShakeDecaySpeed = decay;
		}



	}
}