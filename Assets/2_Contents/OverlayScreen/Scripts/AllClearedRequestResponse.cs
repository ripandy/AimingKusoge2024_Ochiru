using System.Threading;
using System.Threading.Tasks;
using Kusoge.DataTransferObjects;
using Kusoge.Interfaces;
using Soar.Transactions;

namespace Contents.LevelIntro
{
    public class GameOverTransaction : Transaction<GameStatsDto, bool>, IGameOverPresenter
    {
        public async ValueTask<bool> Show(GameStatsDto statsDto, CancellationToken cancellationToken = default)
        {
            return await RequestAsync(statsDto, cancellationToken);
        }
    }
}