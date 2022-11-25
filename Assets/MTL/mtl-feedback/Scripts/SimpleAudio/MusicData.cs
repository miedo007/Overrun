using System;
using UnityEngine;

namespace MTLSimpleAudio
{
    [CreateAssetMenu(fileName = "music_", menuName = "SimpleAudio/MusicData", order = 0)]
    public class MusicData : ScriptableObject
    {
        public static event Action<MusicData> PlayRequested;
        public static event Action<MusicData> StopRequested;

        [SerializeField] private AudioClip clip = null;
        [SerializeField] private float volume = 1;
        [SerializeField] private float pitch = 1;

        public AudioClip Clip => clip;

        public float Volume => volume;

        public float Pitch => pitch;
        

        public void Play()
        {
            PlayRequested?.Invoke(this);
        }

        public void Stop()
        {
            StopRequested?.Invoke(this);
        }
    }
}