using Kusoge;
using Soar.Variables;
using UnityEngine;

namespace Contents.Gameplay
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Kusoge/PlayerData")]
    public class PlayerData : JsonableVariable<Player>
    {
    }
}