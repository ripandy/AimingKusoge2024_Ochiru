using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.GameStates
{
    public class GameOverState
    {
        public GameStateEnum StateId => GameStateEnum.GameOver;
        
        public GameOverState()
        {
        }

        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            return GameStateEnum.None;
        }
    }
}