using Soar;
using Soar.Collections;
using UnityEngine;

namespace Feature.Extension
{
    [CreateAssetMenu(fileName = "SpriteCollection", menuName = MenuHelper.DefaultCollectionMenu + "SpriteCollection")]
    public class SpriteCollection : Collection<Sprite>
    {
    }
}