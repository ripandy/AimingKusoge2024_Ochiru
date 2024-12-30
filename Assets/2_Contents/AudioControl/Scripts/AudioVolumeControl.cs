using System;
using Soar.Variables;
using UnityEngine;

namespace Feature.AudioControl
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioVolumeControl : MonoBehaviour
    {
        [SerializeField] private Variable<float> volumeVariable;
        
        private IDisposable subscription;

        private void Start()
        {
            if (!TryGetComponent<AudioSource>(out var audioSource)) return;
            subscription = volumeVariable.Subscribe(value => audioSource.volume = value);
        }
        
        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}