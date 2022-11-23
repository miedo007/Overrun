using System;
using UnityEngine;

namespace MTLSimpleAudio
{
    [CreateAssetMenu(fileName = "music_", menuName = "SimpleAudio/MusicData", order = 0)]
    public class MusicData : ScriptableObject
    {
        public static event Action<MusicData> PlayRequested;

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
    }
}