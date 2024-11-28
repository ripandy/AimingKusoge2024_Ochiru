using System;
using UnityEngine;

namespace Feature.AudioControl
{
    [RequireComponent(typeof(AudioSource))]
    public class BgmHandler : MonoBehaviour
    {
        [SerializeField] private AudioClipVariable audioClipVariable;

        private AudioSource audioSource;
        
        private IDisposable subscription;

        private void Start()
        {
            if (!TryGetComponent(out audioSource)) return;

            audioSource.loop = true;
            subscription = audioClipVariable.Subscribe(PlayClip);
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip == null) return;
            
            audioSource.clip = clip;
            audioSource.Play();
        }
        
        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}