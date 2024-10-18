using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.GameStates
{
    public interface IGameState
    {
        GameStateEnum Id { get; }
        ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default);
    }
}