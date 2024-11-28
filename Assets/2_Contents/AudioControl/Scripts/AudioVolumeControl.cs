using Soar.Variables;
using UnityEngine;

namespace Feature.AudioControl
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioVolumeControl : MonoBehaviour
    {
        [SerializeField] private Variable<float> volumeVariable;

        private void Start()
        {
            if (!TryGetComponent<AudioSource>(out var audioSource)) return;
            volumeVariable.Subscribe(value => audioSource.volume = value);
        }
    }
}