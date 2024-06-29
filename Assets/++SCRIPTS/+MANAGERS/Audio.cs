using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Audio : MonoBehaviour
{
    private List<AudioSource> AudioSources = new List<AudioSource>();
    private AudioSource specialAudioSource;
    private AudioSource audioSource;
    
    private static Audio I;

    private void Awake()
    {
        I = this;
        audioSource = GetComponent<AudioSource>();
    }

    private AudioSource AddAudioSource()
    {
        var newAudioSource = gameObject.AddComponent<AudioSource>();
        AudioSources.Add(newAudioSource);
        return newAudioSource;
    }

    public void StartLongSound(AudioClip clip)
    {
        specialAudioSource = GetAudioSource();
        specialAudioSource.clip = clip;
        specialAudioSource.Play();
    }

    public static void StopSpecialSound()
    {
        if (I.specialAudioSource == null) return;
        I.specialAudioSource.Stop();
    }

    public static void PlaySound(AudioClip clip, float delay = 0)
    {
        var source = GetAudioSource();
        source.clip = clip;

        source.PlayOneShot(clip);

    }

    private static AudioSource GetAudioSource()
    {
        var source = GetNextAudioSource();
        //if (I.audioSource == null) I.audioSource = I.AddAudioSource();
        return source;
    }

    private static AudioSource GetNextAudioSource()
    {
        foreach (var source in I.AudioSources)
        {
            if (source.isPlaying) continue;
            return source;
        }

        return I.AddAudioSource();
    }
}
