using System.Collections.Generic;
using __SCRIPTS._COMMON;
using UnityEngine;

namespace __SCRIPTS._SFX
{
    public class SFX : MonoBehaviour
    {
        private static List<AudioSource> AudioSources = new List<AudioSource>();
        private static AudioSource specialAudioSource;
        public static AudioAssets sounds => _audio ? _audio : Resources.Load<AudioAssets>("Assets/Audio");
        private static AudioAssets _audio;

        private void OnEnable()
        {
            ListExtensions.OnPlaySound += ListExtensions_OnPlaySound;
            
        }

        private void ListExtensions_OnPlaySound(AudioClip clip)
        {
            PlaySound(clip);
        }

        private static AudioSource AddAudioSource()
        {
            var audioSourceParent = new GameObject("AudioSourceParent");
            var newAudioSource = audioSourceParent.AddComponent<AudioSource>();
            AudioSources.Add(newAudioSource);
            return newAudioSource;
        }

        public void StartSpecialSound(AudioClip clip)
        {
            specialAudioSource = GetNextAudioSource();
            specialAudioSource.clip = clip;
            specialAudioSource.Play();
        }

        public static void StopSpecialSound()
        {
            if (specialAudioSource == null) return;
            specialAudioSource.Stop();
        }

        public static void PlaySound(AudioClip clip, float delay = 0)
        {
            var source = GetNextAudioSource();
            source.clip = clip;
            source.PlayOneShot(clip);

        }


        private static AudioSource GetNextAudioSource()
        {
            foreach (var source in AudioSources)
            {
                if (source.isPlaying) continue;
                return source;
            }

            return AddAudioSource();
        }
    }
}
