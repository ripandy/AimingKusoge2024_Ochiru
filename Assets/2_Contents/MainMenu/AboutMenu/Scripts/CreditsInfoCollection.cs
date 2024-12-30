using System;
using Soar.Collections;
using UnityEngine;

namespace Feature.AboutMenu
{
    [CreateAssetMenu(fileName = "CreditsInfoCollection", menuName = "Kusoge/CreditsInfoCollection")]
    public class CreditsInfoCollection : Collection<CreditsInfo>
    {
    }

    [Serializable]
    public struct CreditsInfo
    {
        public string role;
        public string[] names;
    }
}