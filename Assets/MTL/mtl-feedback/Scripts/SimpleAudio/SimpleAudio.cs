using System.Collections;
using System.Collections.Generic;
using Project.Application;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Audio;

namespace MTLSimpleAudio
{
    public class SimpleAudio : MonoBehaviour
    {
        [SerializeField] private AudioObject audioObjectPrefab = null;
        [SerializeField] private AudioObject musicObjectPrefab = null;
        [SerializeField] private AudioMixerGroup soundFxGroup = null;
        [SerializeField] private AudioMixerGroup musicGroup = null;
        
        private float _defaultMusicVolume;
        private float _defaultSoundVolume;
        private AudioObject _currentMusicObject;
        private readonly List<AudioData> _registeredData = new();
        private float _unduckMusicTime;

        private const string MusicVolume = "volume_music";
        private const string SoundVolume = "volume_fx";
        
        public bool GetSoundEffectsActive()
        {
            return PlayerPrefs.GetInt(PrefKeys.SoundEnabled, 1) == 1;
        }

        public void SetSoundEffectsActive(bool value)
        {
            PlayerPrefs.SetInt(PrefKeys.SoundEnabled, value ? 1 : 0);
            PlayerPrefs.Save();
            soundFxGroup.audioMixer.SetFloat(SoundVolume, value ? _defaultSoundVolume : -100);
        }

        public bool GetMusicActive()
        {
            return PlayerPrefs.GetInt(PrefKeys.MusicEnabled, 1) == 1;
        }
        
        public void SetMusicActive(bool value)
        {
            PlayerPrefs.SetInt(PrefKeys.MusicEnabled, value ? 1 : 0);
            PlayerPrefs.Save();
            musicGroup.audioMixer.SetFloat(MusicVolume, value ? _defaultMusicVolume : -100);
        }

        private void OnEnable()
        {
            musicGroup.audioMixer.GetFloat(MusicVolume, out _defaultMusicVolume);
            soundFxGroup.audioMixer.GetFloat(SoundVolume, out _defaultSoundVolume);

            AudioData.Register += AudioDataOnRegister;
            AudioData.PlayRequested += AudioDataOnPlayRequested;
            MusicData.PlayRequested += MusicDataOnPlayRequested;
            MusicData.StopRequested += MusicDataOnStopRequested;
        }

        private void OnDisable()
        {
            AudioData.Register -= AudioDataOnRegister;
            AudioData.PlayRequested -= AudioDataOnPlayRequested;
            MusicData.PlayRequested -= MusicDataOnPlayRequested;
            MusicData.StopRequested -= MusicDataOnStopRequested;
        }

        private void OnDestroy()
        {
            foreach (var data in _registeredData)
            {
                data.Cleanup();
            }
        }

        private void AudioDataOnRegister(AudioData obj)
        {
            obj.Initialize();
            _registeredData.Add(obj);
        }

        private void AudioDataOnPlayRequested(AudioInstance audioInstance)
        {
            if (audioInstance.DuckMusic)
            {
                var unduckTime = Time.time + ((audioInstance.Clip.length * audioInstance.Pitch) * 0.5f);
                if (unduckTime > _unduckMusicTime)
                {
                    musicGroup.audioMixer.SetFloat(MusicVolume, -35f);
                    _unduckMusicTime = unduckTime;
                }
            }
            if (audioInstance.Source)
            {
                audioInstance.Source.Play(audioInstance, false);
            }
            else
            {
                var audioObject = LeanPool.Spawn(audioObjectPrefab, transform);
                audioObject.Play(audioInstance);
            }
        }

        private IEnumerator Start()
        {
            // Waiting a frame is required before accessing the audio mixer
            yield return null;
            yield return null;
            SetSoundEffectsActive(GetSoundEffectsActive());
            SetMusicActive(GetMusicActive());
        }

        private void Update()
        {
            if (GetMusicActive())
            {
                if (Time.time >= _unduckMusicTime)
                {
                    musicGroup.audioMixer.SetFloat(MusicVolume, _defaultMusicVolume);
                }
            }
        }

        private void MusicDataOnPlayRequested(MusicData musicData)
        {
            if (_currentMusicObject != null)
            {
                if (_currentMusicObject.Clip == musicData.Clip)
                {
                    return;
                }
                
                _currentMusicObject.StopMusic();
            }

            _currentMusicObject = LeanPool.Spawn(musicObjectPrefab, transform);
            _currentMusicObject.PlayMusic(musicData);
        }

        private void MusicDataOnStopRequested(MusicData musicData)
        {
            if (_currentMusicObject != null)
            {
                if (_currentMusicObject.Clip != musicData.Clip)
                {
                    return;
                }
                
                _currentMusicObject.StopMusic();
                _currentMusicObject = null;
            }
        }
    }
}