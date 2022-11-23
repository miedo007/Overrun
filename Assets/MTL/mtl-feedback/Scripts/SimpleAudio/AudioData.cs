using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MTLSimpleAudio
{
    [CreateAssetMenu(fileName = "audio_", menuName = "SimpleAudio/AudioData", order = 0)]
    public class AudioData : ScriptableObject
    {
        public static event Action<AudioInstance> PlayRequested;

        [SerializeField] private AudioClip[] clips = null;
        [SerializeField] private Vector2 volumeRange = Vector2.one;
        [SerializeField] private Vector2 pitchRange = Vector2.one;
        [SerializeField] private Vector2 delayRange = Vector2.zero;

        [NonSerialized] private float _nextPlayTime = 0;
        
        public int LastIndexPlayed { get; set; }

        public void Play()
        {
            Play(false);
        }

        public void Play(AudioObject source)
        {
            Play(true, source);
        }

        private void Play(bool useSpecifiedSource, AudioObject source = null)
        {
            if (Time.realtimeSinceStartup < _nextPlayTime)
            {
                return;
            }

            var clipIndex = 0;
            if (clips.Length > 1)
            {
                do
                {
                    clipIndex = Random.Range(0, clips.Length);
                } while (clipIndex == LastIndexPlayed);
            }

            LastIndexPlayed = clipIndex;
            
            PlayRequested?.Invoke(new AudioInstance()
            {
                Clip = clips[clipIndex],
                Volume = Random.Range(volumeRange.x, volumeRange.y),
                Pitch = Random.Range(pitchRange.x, pitchRange.y),
                UseSpecifiedSource = useSpecifiedSource,
                Source = source
            });

            _nextPlayTime = Time.realtimeSinceStartup + Random.Range(delayRange.x, delayRange.y);
        }
        
        public void Play(AudioClip audioClip, float volume = 1, float pitch = 1)
        {
            if (Time.realtimeSinceStartup < _nextPlayTime)
            {
                return;
            }

            PlayRequested?.Invoke(new AudioInstance()
            {
                Clip = audioClip,
                Volume = volume,
                Pitch = pitch
            });
            

            _nextPlayTime = Time.realtimeSinceStartup + Random.Range(delayRange.x, delayRange.y);
        }
    }
    
    

    public struct AudioInstance
    {
        public AudioClip Clip;
        public float Volume;
        public float Pitch;
        public bool UseSpecifiedSource;
        public AudioObject Source;
    }
}

