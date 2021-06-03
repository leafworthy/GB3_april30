using _SCRIPTS;
using UnityEngine;

public class HITSTUN : Singleton<HITSTUN>
{
	private static bool isStunned;
	private static float currentStunDuration;
	public static void StartStun(float duration)
	{
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
