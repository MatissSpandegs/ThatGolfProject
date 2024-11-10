using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Manager
{
    public class GameplaySoundManager : MonoBehaviour
    {
        [SerializeField] private List<SoundData> sounds;
        [SerializeField] private List<SoundData> winSounds;
        [SerializeField] private List<SoundData> loseSounds;
        
        private List<AudioSource> sources = new();

        public void PlaySound(Sounds sound)
        {
            SoundData soundData;
            switch (sound)
            {
                case Sounds.ShotIn:
                    soundData = GetRandomSound(winSounds);
                    break;
                case Sounds.ShotMissed:
                    soundData = GetRandomSound(loseSounds);
                    break;
                default:
                    if (!FindSound(sound, out soundData))
                    {
                        Debug.LogError($"Unable to play sound {sound}, no clip found");
                        return;
                    }
                    break;
            }

            for (var i = 0; i < sources.Count; i++)
            {
                var audioSource = sources[i];
                if (audioSource.isPlaying)
                {
                    continue;
                }
                PlaySound(audioSource, soundData);
                return;
            }

            var newSource = gameObject.AddComponent<AudioSource>();
            sources.Add(newSource);
            PlaySound(newSource, soundData);
        }

        private SoundData GetRandomSound(List<SoundData> randomSounds)
        {
            var randomIndex = Random.Range(0, randomSounds.Count);
            return randomSounds[randomIndex];
        }

        private bool FindSound(Sounds sound, out SoundData data)
        {
            data = null;
            for (var i = 0; i < sounds.Count; i++)
            {
                var soundData = sounds[i];
                if (soundData.Type != sound) continue;
                data = soundData;
                return true;
            }
            return false;
        }

        private void PlaySound(AudioSource source, SoundData data)
        {
            source.clip = data.Sound;
            source.volume = data.Volume;
            source.loop = data.Looping;
            source.Play();
        }



        [Serializable]
        public enum Sounds
        {
            Cheer,
            Shoot,
            ShotIn,
            ShotMissed,
            Ambience
        }

        [Serializable]
        public class SoundData
        {
            public Sounds Type;
            public AudioClip Sound;
            public float Volume;
            public bool Looping;
        }
    }
}
