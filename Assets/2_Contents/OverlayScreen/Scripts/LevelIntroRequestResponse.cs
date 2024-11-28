using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Kusoge.Interfaces;
using Soar.Transactions;

namespace Contents.LevelIntro
{
    public class LevelIntroRequestResponse : Transaction, IIntroPresenter
    {
        public async ValueTask Show(CancellationToken cancellationToken = default)
        {
            await RequestAsync(cancellationToken);
        }
    }
}