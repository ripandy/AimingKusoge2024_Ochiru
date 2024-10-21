using System.Threading;
using System.Threading.Tasks;
using Kusoge.Entities;

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
            var statistics = new GameStatistics(player.BeanEatenCount, player.ComboCount);
            var restart = await gameOverPresenter.Show(statistics, cancellationToken);
            return restart ? GameStateEnum.Intro : GameStateEnum.None;
        }
    }
    
    public interface IGameOverPresenter
    {
        /// <summary>
        /// Presents with Game Over screen and returns true if user wants to restart the game.
        /// </summary>
        /// <returns>true to restart or false to exit.</returns>
        ValueTask<bool> Show(GameStatistics statistics, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// DTO for game-end statistics.
    /// </summary>
    public struct GameStatistics
    {
        public int Score { get; }
        public int Combo { get; }
        
        public GameStatistics(int score, int combo)
        {
            Score = score;
            Combo = combo;
        }
    }
}