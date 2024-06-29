using System;
using UnityEngine;

public class CameraStunner : MonoBehaviour
{
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
			       StunLength.Short => .01f,
			       StunLength.Normal => .0175f,
			       StunLength.Long => .025f,
			       StunLength.Special => .5f,
			       StunLength.None =>0,
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
