using System.Threading;
using System.Threading.Tasks;
using Kusoge.Entities;
using Kusoge.Interfaces;
using UnityEngine;

namespace Kusoge.Tests
{
    internal class DummyPlayerInputProvider : IPlayerDirectionInputProvider, IPlayerBiteInputProvider
    {
        private readonly Player player;
        private readonly BeanLauncher beanLauncher;
        
        public DummyPlayerInputProvider(Player player, BeanLauncher beanLauncher)
        {
            this.player = player;
            this.beanLauncher = beanLauncher;
        }
        
        public async ValueTask<DirectionEnum> WaitForDirectionInput(CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Delay(beanLauncher.LaunchDelay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            
            var direction = player.BeanEatenCount < 3 && beanLauncher.TryGetBean(player.BeanEatenCount, out var bean)
                ? bean.ThrowDirection
                : (DirectionEnum)(Random.Range(0, 3) - 1);
            Debug.Log($"Player Direction update: {direction}");
            return  direction;
        }

        public async ValueTask<int> WaitForBite(CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Delay(beanLauncher.LaunchDelay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            
            var beanId = player.BeanEatenCount < 3
                ? player.BeanEatenCount
                : Random.Range(0, beanLauncher.LaunchedBeanCount * 2);
            if (beanLauncher.TryGetBean(beanId, out var bean) && bean.ThrowDirection == player.Direction)
            {
                Debug.Log($"Player Bite beanId: {beanId}");
                return beanId;
            }
            
            Debug.Log("Player Bite Nothing..!");
            return -1;
        }
    }
}