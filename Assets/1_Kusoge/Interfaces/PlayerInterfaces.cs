using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.Interfaces
{
    public enum DirectionEnum
    {
        Forward = 0,
        Left = -1,
        Right = 1
    }
    
    public interface IPlayerDirectionPresenter
    {
        void Show(DirectionEnum direction);
    }
    
    public interface IPlayerDirectionInputProvider
    {
        ValueTask<DirectionEnum> WaitForDirectionInput(CancellationToken cancellationToken = default);
    }
    
    public interface IPlayerBiteInputProvider
    {
        ValueTask<int> WaitForBite(CancellationToken cancellationToken = default);
    }
}