using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class CameraStunner_FX : MonoBehaviour
	{
		private const float stunMultiplier = 2;//1.5f

		public enum StunLength
		{
			Short,
			Normal,
			Long,
			Special,
			None
		}
		private static bool isStunned;
		private static float currentStunDuration;

		 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ResetStatics()
		{
			isStunned = false;
			currentStunDuration = 0;
			Time.timeScale = 1;
		}
		public static void StartStun(StunLength length)
		{
			var duration = GetDurationFromLength(length);
			if (isStunned)
			{
				if (currentStunDuration < duration)
				{
					currentStunDuration = duration;
				}
			}
			else
			{
				Time.timeScale = 0;
				currentStunDuration = duration;
				isStunned = true;
			}
		}

		private static float GetDurationFromLength(StunLength length)
		{
			return length switch
			       {
				       StunLength.Short => .01f*stunMultiplier,
				       StunLength.Normal => .0175f * stunMultiplier,
				       StunLength.Long => .025f * stunMultiplier,
				       StunLength.Special => .2f * stunMultiplier,
				       StunLength.None =>0 * stunMultiplier,
				       _ => throw new ArgumentOutOfRangeException(nameof(length), length, null)
			       };
		}

		public void Update()
		{

			if (!isStunned) return;
			currentStunDuration -= Time.unscaledDeltaTime;
			if (!(currentStunDuration <= 0)) return;
			currentStunDuration = 0;
			isStunned = false;
			Time.timeScale = 1;
		}
	}
}
