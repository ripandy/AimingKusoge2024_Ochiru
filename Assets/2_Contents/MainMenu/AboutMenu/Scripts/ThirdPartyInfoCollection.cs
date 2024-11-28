using System;
using Soar;
using Soar.Collections;
using UnityEngine;

namespace Feature.AboutMenu
{
    [CreateAssetMenu(fileName = "ThirdPartyInfoCollection", menuName = MenuHelper.DefaultCollectionMenu + "ThirdPartyInfoCollection")]
    public class ThirdPartyInfoCollection : Collection<ThirdPartyInfo>
    {
    }

    [Serializable]
    public struct ThirdPartyInfo
    {
        public string componentName;
        public string licenseType;
        [TextArea]
        public string licenseText;
    }
}