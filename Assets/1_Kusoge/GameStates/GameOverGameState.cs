using System.Threading;
using System.Threading.Tasks;
using Kusoge.DataTransferObjects;
using Kusoge.Interfaces;

namespace Kusoge.GameStates
{
    public class GameOverGameState : IGameState
    {
        private readonly Player player;
        private readonly IGameOverPresenter gameOverPresenter;
        
        public GameStateEnum Id => GameStateEnum.GameOver;
        
        public GameOverGameState(Player player, IGameOverPresenter gameOverPresenter)
        {
            this.player = player;
            this.gameOverPresenter = gameOverPresenter;
        }

        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            var statistics = new GameStatsDto(player.BeanEatenCount, player.MaxComboCount);
            var restart = await gameOverPresenter.Show(statistics, cancellationToken);
            return restart ? GameStateEnum.Intro : GameStateEnum.None;
        }
    }
}