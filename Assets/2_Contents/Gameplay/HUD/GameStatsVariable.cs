using Kusoge.DataTransferObjects;
using Kusoge.Interfaces;
using Soar.Variables;

namespace Contents.Gameplay.HUD
{
    public class GameStatsVariable : Variable<GameStatsDto>, IPlayerStatsPresenter
    {
        public void Show(GameStatsDto statsDto)
        {
            Value = statsDto;
        }
    }
}