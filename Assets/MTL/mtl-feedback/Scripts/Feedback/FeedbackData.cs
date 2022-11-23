using System;
using Lofelt.NiceVibrations;
using MTLSimpleAudio;
using UnityEngine;

namespace Project.Feedback
{
    [CreateAssetMenu(fileName = "feedback_", menuName = "Data/FeedbackData", order = 0)]
    public class FeedbackData : ScriptableObject
    {
        public static event Action<FeedbackInfo> PlayRequested;
        
        [SerializeField] private bool playHaptics;
        
        [SerializeField] private HapticPatterns.PresetType hapticPreset = HapticPatterns.PresetType.None;

        [SerializeField] private bool playFx;
        [SerializeField] private GameObject fxPrefab;
        [SerializeField] private float fxLifetime = 1;
        
        [SerializeField] private bool playAudio;
        [SerializeField] private AudioData audioData;

        [SerializeField] private bool playScreenShake;
        [SerializeField] private float amplitude;

        [SerializeField] private bool playHitStop;
        [SerializeField] private float hitStopTime = 0.05f;
        
        public bool PlayHaptics => playHaptics;
        public HapticPatterns.PresetType HapticPreset => hapticPreset;

        public bool PlayFx => playFx;
        public GameObject FxPrefab => fxPrefab;
        public float FxLifetime => fxLifetime;
        public bool PlayAudio => playAudio;

        public bool PlayHitStop => playHitStop;

        public float HitStopTime => hitStopTime;

        public bool PlayScreenShake => playScreenShake;

        public float Amplitude => amplitude;

        public void Play()
        {
            Play(Vector3.zero, Quaternion.identity, -1f);
        }

        public void Play(Vector3 position, Quaternion rotation, float delay = -1f)
        {
            if (playAudio)
            {
                audioData.Play();
            }
            
            PlayRequested?.Invoke(new FeedbackInfo
            {
                Data = this,
                Position = position,
                Rotation = rotation,
                Delay = delay
            });
        }
    }

    public struct FeedbackInfo
    {
        public FeedbackData Data;
        public Vector3 Position;
        public Quaternion Rotation;
        public float Delay;
    }
}