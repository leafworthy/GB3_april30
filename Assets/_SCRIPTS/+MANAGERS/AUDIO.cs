using System.Collections.Generic;
using UnityEngine;
using AudioSource = UnityEngine.AudioSource;



namespace _SCRIPTS
{
    public class AUDIO : Singleton<AUDIO>
    {
        public List<AudioSource> AudioSources = new List<AudioSource>();

        private void Start()
        {
            GetAudioSource();
        }

        public void PlaySound(string soundName, float delay = 0)
        {
            var source = GetAudioSource();
            source.clip = Resources.Load("SFX/" + soundName) as AudioClip;

            source.PlayDelayed(delay);

        }

        public void PlaySound(AudioClip clip, float delay = 0)
        {
            var source = GetAudioSource();
            source.clip = clip;

            source.PlayDelayed(delay);

        }

        private AudioSource GetAudioSource()
        {
            for (int i = 0; i < AudioSources.Count; i++)
            {
                if (!AudioSources[i].isPlaying)
                {
                    return AudioSources[i];
                }
            }

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            AudioSources.Add(audioSource);
            return audioSource;
        }
    }
}
