using Kusoge.DataTransferObjects;
using Kusoge.Interfaces;

namespace Kusoge.Tests
{
    public class DummyPlayerPresenter : IPlayerDirectionPresenter, IPlayerHealthPresenter, IPlayerStatsPresenter
    {
        public void Show(DirectionEnum direction)
        {
        }

        public void Show(float healthPercentage)
        {
        }

        public void Show(GameStatsDto statsDto)
        {
        }
    }
}