using System.Threading;
using System.Threading.Tasks;
using Kusoge.DataTransferObjects;
using Kusoge.Interfaces;
using UnityEngine;

namespace Kusoge.Tests
{
    public class DummyGameOverPresenter : IGameOverPresenter
    {
        public async ValueTask<bool> Show(GameStatsDto stats, CancellationToken cancellationToken = default)
        {
            Debug.Log("Game Over!");
            await Task.Delay(1000, cancellationToken);
            Debug.Log($"Beans Eaten: {stats.Score}");
            await Task.Delay(500, cancellationToken);
            Debug.Log($"Last Combo: {stats.Combo}");
            await Task.Delay(500, cancellationToken);
            
            return stats.Combo != 99; // For test purpose, return false (exit game) if combo is 99.
        }
    }
}