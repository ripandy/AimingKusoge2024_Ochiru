using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.Interfaces
{
    public interface IGameOverPresenter
    {
        /// <summary>
        /// Presents with Game Over screen and returns true if user wants to restart the game.
        /// </summary>
        /// <returns>true to replay or false to exit.</returns>
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