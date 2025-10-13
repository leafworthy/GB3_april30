using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlaySoundFromTimeToTime : MonoBehaviour
	{
		[SerializeField] private float roarRateMin = 15;
		[SerializeField] private float roarRateMax = 25;
		[SerializeField] private float currentRoarTime;
		public List<AudioClip> soundsToPlay = new();
		private float roarRate;

		private void OnEnable()
		{
			RandomizeRoarRate();
		}

		private void RandomizeRoarRate()
		{
			roarRate = Random.Range(roarRateMin, roarRateMax);
		}

		private void Update()
		{
			currentRoarTime += Time.deltaTime;
			if (!(currentRoarTime >= roarRate)) return;
			currentRoarTime = 0;
			RandomizeRoarRate();
			soundsToPlay.PlayRandomAt(transform.position);
		}
	}
}