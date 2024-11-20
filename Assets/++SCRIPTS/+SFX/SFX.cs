using System.Collections.Generic;
using UnityEngine;

public class SFX : Singleton<SFX>
{
	private static List<AudioSource> AudioSources = new();
	private static AudioSource specialAudioSource;
	public static AudioAssets sounds => _audio ? _audio : Resources.Load<AudioAssets>("Assets/Audio");
	private static AudioAssets _audio;
	public AudioSource UIaudioSource;
	public AudioSource SFXaudioSource;

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


	public static void StopSpecialSound()
	{
		if (specialAudioSource == null) return;
		specialAudioSource.Stop();
	}

	public static void PlayUISound(AudioClip clip, float delay = 0)
	{
		I.UIaudioSource.clip = clip;
		I.UIaudioSource.PlayOneShot(clip);
	}

	private static void PlaySFXAt(AudioClip clip, Vector3 position, float delay = 0)
	{
		I.SFXaudioSource.transform.position = position;
		I.SFXaudioSource.PlayOneShot(clip);
	}

	
}