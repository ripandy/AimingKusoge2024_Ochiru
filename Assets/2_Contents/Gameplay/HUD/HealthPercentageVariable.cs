using Kusoge.Interfaces;
using Soar.Variables;

namespace Contents.Gameplay.HUD
{
    public class HealthPercentageVariable : Variable<float>, IPlayerHealthPresenter
    {
        public void Show(float healthPercentage)
        {
            Value = healthPercentage;
        }
    }
}