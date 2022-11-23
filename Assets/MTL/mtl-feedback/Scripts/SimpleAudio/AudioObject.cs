using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace MTLSimpleAudio
{
    public class AudioObject : MonoBehaviour
    {
        private const float FadeTime = 1;
        
        [SerializeField] private AudioSource audioSource = null;
        public bool IsPlaying => audioSource.isPlaying;
        public AudioClip Clip => audioSource.clip;
        
        public void Play(AudioInstance audioInstance, bool despawnOnCompletion = true)
        {
            audioSource.clip = audioInstance.Clip;
            audioSource.volume = audioInstance.Volume;
            audioSource.pitch = audioInstance.Pitch;
            audioSource.Play();
            
            if (despawnOnCompletion)
            {
                LeanPool.Despawn(this, audioInstance.Clip.length / audioSource.pitch);
            }
        }

        public void PlayMusic(MusicData musicData)
        {
            audioSource.clip = musicData.Clip;
            audioSource.loop = true;

            audioSource.volume = 0;
            audioSource.Play();
            audioSource.DOKill();
            audioSource.DOFade(musicData.Volume, FadeTime).SetEase(Ease.InQuad);
        }

        public void StopMusic()
        {
            audioSource.DOKill();
            audioSource.DOFade(0, FadeTime)
                .OnComplete(() =>
                {
                    LeanPool.Despawn(this);
                });
        }
    }
    
}