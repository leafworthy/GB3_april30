using UnityEngine;

namespace GangstaBean.Audio
{
	public class RoarFromTimeToTime : MonoBehaviour
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
				SFX.I.sounds.cone_roar_sounds.PlayRandomAt(transform.position);
			}
		}
	}
}