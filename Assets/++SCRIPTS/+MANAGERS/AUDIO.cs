using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AUDIO : Singleton<AUDIO>
{
    private List<AudioSource> AudioSources = new List<AudioSource>();

    public static void PlaySound(AudioClip clip, float delay = 0)
    {
        var source = I.GetAudioSource();
        source.clip = clip;

        source.PlayDelayed(delay);

    }

    private AudioSource GetAudioSource()
    {
        foreach (var t in AudioSources.Where(t => !t.isPlaying))
        {
            return t;
        }

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioSources.Add(audioSource);
        return audioSource;
    }
}
