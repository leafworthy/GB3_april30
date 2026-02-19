using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class ObjectShaker : MonoBehaviour
	{
		float ShakeIntensity;
		float ShakeDecaySpeed;
		float ShakeDuration;
		Vector2 originalPosition;
		Vector2 shakeFactorPosition;
		bool hasCapturedOriginalPosition;

		void OnDisable()
		{
			transform.localPosition = originalPosition;

		}

		void OnEnable()
		{
			if (hasCapturedOriginalPosition) return;
			hasCapturedOriginalPosition = true;
			originalPosition = transform.localPosition;
		}

		// Update is called once per frame
		void Update()
		{
			// If shakeDuration is still running.
			if (ShakeDuration > 0)
			{
				shakeFactorPosition = (Vector2)Random.insideUnitCircle * ShakeIntensity * ShakeDuration;
				ShakeDuration -= ShakeDecaySpeed * Time.unscaledDeltaTime;
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
					Shake(5, .5f, .5f);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		void Shake(float intensity, float duration, float decay)
		{


				transform.localPosition = originalPosition;


			ShakeIntensity = intensity;
			ShakeDuration = duration;
			ShakeDecaySpeed = decay;
		}



	}
}
