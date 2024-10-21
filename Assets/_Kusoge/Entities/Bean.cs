using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.Entities
{
    public struct Bean
    {
        internal static int id;
        
        public int Id { get; }
        public Player.DirectionEnum ThrowDirection { get; }
        
        public Bean(Player.DirectionEnum throwDirection)
        {
            Id = id++;
            ThrowDirection = throwDirection;
        }
    }

    public interface IBeanPresenter
    {
        ValueTask<bool> Show(int id, Player.DirectionEnum throwDirection, CancellationToken cancellationToken = default);
        void Hide(int id);
    }
}