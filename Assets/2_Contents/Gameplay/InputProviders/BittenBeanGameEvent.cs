using System.Threading;
using System.Threading.Tasks;
using Kusoge.Interfaces;
using Soar.Events;

namespace Contents.Gameplay
{
    public class BittenBeanGameEvent : GameEvent<int>, IPlayerBiteInputProvider
    {
        public ValueTask<int> WaitForBite(CancellationToken cancellationToken = default)
        {
            return EventAsync(cancellationToken);
        }
    }
}