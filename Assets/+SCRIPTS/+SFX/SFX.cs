using System.Collections.Generic;
using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class SFX : MonoBehaviour, IService
	{
		public AudioSource ongoingAudioSource;
		public AudioAssets sounds => _audio ?? Resources.Load<AudioAssets>("Assets/Audio");
		private AudioAssets _audio;
		public AudioSource UIaudioSource;
		public AudioSource SFXaudioSource;
		private const float maxDistance = 100;
		public void StartService()
		{
		}

		public  void PlayRandomAt(List<AudioClip> list, Vector3 position)
		{

			PlaySFXAt(list.GetRandom(), position);

		}

		public  void PlayRandom(AudioClip clip)
		{
			PlayUISound(clip);

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

		public void PlayUISound(AudioClip clip, float delay = 0)
		{
			UIaudioSource.clip = clip;
			UIaudioSource.PlayOneShot(clip);
		}

		private void PlaySFXAt(AudioClip clip, Vector3 position)
		{
			if (CursorManager.GetCamera() == null) return;
			if(Vector2.Distance( CursorManager.GetCamera().transform.position, position) > maxDistance) return;
			//I.SFXaudioSource.gameObject.transform.position = position;
			if (SFXaudioSource == null) return;
			SFXaudioSource.PlayOneShot(clip);
		}


	}
}
