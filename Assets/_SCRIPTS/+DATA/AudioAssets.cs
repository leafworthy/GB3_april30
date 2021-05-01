using System;
using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/AudioAssets")]
	public class AudioAssets : ScriptableObject
	{
		public enum SoundType
		{
			shoot,
			knifehit
		}

		public List<AudioClip> shoot_sounds = new List<AudioClip>();
		public List<AudioClip> knifehit_sounds = new List<AudioClip>();

		public AudioClip PlayRandom(SoundType soundType)
		{
			switch (soundType)
			{
				case SoundType.shoot:
					return shoot_sounds.GetRandom();
					break;
				case SoundType.knifehit:
					return knifehit_sounds.GetRandom();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
			}
		}
	}
}
