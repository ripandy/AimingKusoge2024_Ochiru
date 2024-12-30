using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.Interfaces
{
    public interface IIntroPresenter
    {
        ValueTask Show(CancellationToken cancellationToken = default);
    }
}