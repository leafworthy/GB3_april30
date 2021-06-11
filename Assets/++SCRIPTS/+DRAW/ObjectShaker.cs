using UnityEngine;

namespace _SCRIPTS
{
	public class ObjectShaker : MonoBehaviour
	{
		protected float ShakeIntensity;
		protected float ShakeDecay;
		protected float ShakeDuration;
		private Vector3 originalPosition;
		private Vector3 shakeFactorPosition;

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
				shakeFactorPosition = Random.insideUnitCircle * ShakeIntensity * ShakeDuration;
				ShakeDuration -= ShakeDecay * Time.deltaTime;
				transform.position = originalPosition + shakeFactorPosition;
			}
			else
			{
				Destroy(this);
			}

		}

		public void Shake(float intensity, float duration, float decay)
		{
			originalPosition = transform.position;
			ShakeIntensity = intensity;
			ShakeDuration = duration;
			ShakeDecay = duration;
		}

		public void ResetShake()
		{
			ShakeIntensity = 0f;
			ShakeDuration = 0f;
			ShakeDecay = 0f;
		}


	}
}
