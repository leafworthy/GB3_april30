using System.Collections.Generic;
using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class SFX : Singleton<SFX>
	{
		private static List<AudioSource> AudioSources = new();
		public AudioSource ongoingAudioSource;
		public static AudioAssets sounds => _audio ? _audio : Resources.Load<AudioAssets>("Assets/Audio");
		private static AudioAssets _audio;
		public AudioSource UIaudioSource;
		public AudioSource SFXaudioSource;
		private static float maxDistance = 100;

		private void OnEnable()
		{
			ListExtensions.OnPlaySoundAt += ListExtensionsOnOnPlaySoundAt;
			ListExtensions.OnPlaySound += ListExtensionsOnOnPlaySound;
		}

		private void ListExtensionsOnOnPlaySound(AudioClip clip)
		{
			PlayUISound(clip);
		}

		private void ListExtensionsOnOnPlaySoundAt(AudioClip clip, Vector3 vector3)
		{
			PlaySFXAt(clip, vector3);
		}

		public  void StartOngoingSound()
		{
			if (ongoingAudioSource == null) return;
			ongoingAudioSource.Play();
		}
		public  void StopOngoingSound()
		{
			if (ongoingAudioSource == null) return;
			ongoingAudioSource.Stop();
		}

		public static void PlayUISound(AudioClip clip, float delay = 0)
		{
			I.UIaudioSource.clip = clip;
			I.UIaudioSource.PlayOneShot(clip);
		}

		private static void PlaySFXAt(AudioClip clip, Vector3 position, float delay = 0)
		{
			if(Vector2.Distance( CursorManager.GetCamera().transform.position, position) > maxDistance) return;
			//I.SFXaudioSource.gameObject.transform.position = position;
			I.SFXaudioSource.PlayOneShot(clip);
		}

	
	}
}