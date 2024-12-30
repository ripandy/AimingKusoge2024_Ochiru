using Soar.Collections;
using UnityEngine;

namespace Feature.AudioControl
{
    [CreateAssetMenu(fileName = "SoundCollection", menuName = "Kusoge/SoundCollection")]
    public class SoundCollection : SoarDictionary<SoundEnum, AudioClip>
    {
    }
}