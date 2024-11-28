using System;
using UnityEngine;

namespace Feature.AudioControl
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxHandler : MonoBehaviour
    {
        [SerializeField] private SoundEnumVariable soundEnumVariable;
        [SerializeField] private SoundCollection soundCollection;

        private AudioSource audioSource;
        private IDisposable subscription;

        private void Start()
        {
            if (!TryGetComponent(out audioSource)) return;
            subscription = soundEnumVariable.Subscribe(PlaySound);
        }

        private void PlaySound(SoundEnum soundEnum)
        {
            if (!soundCollection.TryGetValue(soundEnum, out var clip)) return;
            
            audioSource.PlayOneShot(clip);
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}