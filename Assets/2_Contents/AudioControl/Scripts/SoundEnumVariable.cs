using Soar;
using Soar.Variables;
using UnityEngine;

namespace Feature.AudioControl
{
    [CreateAssetMenu(fileName = "SoundEnumVariable", menuName = MenuHelper.DefaultVariableMenu + "SoundEnum")]
    public class SoundEnumVariable : Variable<SoundEnum>
    {
    }

    public enum SoundEnum
    {
        Button,
    }
}