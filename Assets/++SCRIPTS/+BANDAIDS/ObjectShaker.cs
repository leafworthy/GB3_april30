using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS._BANDAIDS
{
	public class ObjectShaker : MonoBehaviour
	{
		protected float ShakeIntensity;
		protected float ShakeDecaySpeed;
		protected float ShakeDuration;
		private Vector2 originalPosition;
		private Vector2 shakeFactorPosition;

		void OnDisable()
		{
			Destroy(this);
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
				Destroy(this);
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

			if (ShakeDuration <= 0)
			{
				originalPosition = transform.localPosition;
			}
			else
			{
				transform.localPosition = originalPosition;
			}

			ShakeIntensity = intensity;
			ShakeDuration = duration;
			ShakeDecaySpeed = decay;
		}


		public void ResetShake()
		{
			ShakeIntensity = 0f;
			ShakeDuration = 0f;
			ShakeDecaySpeed = 0f;
		}


	}
}
