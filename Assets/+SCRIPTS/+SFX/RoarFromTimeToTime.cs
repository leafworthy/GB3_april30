using UnityEngine;

namespace __SCRIPTS
{
	public class RoarFromTimeToTime : ServiceUser
	{
		private float roarRate;
		private float currentRoarTime;

		private void OnEnable()
		{
			roarRate = Random.Range(15, 25);
		}

		private void Update()
		{
			currentRoarTime += Time.deltaTime;
			if (currentRoarTime >= roarRate)
			{
				currentRoarTime = 0;
				roarRate = Random.Range(15, 25);
				sfx.sounds.cone_roar_sounds.PlayRandomAt(transform.position);
			}
		}
	}
}
