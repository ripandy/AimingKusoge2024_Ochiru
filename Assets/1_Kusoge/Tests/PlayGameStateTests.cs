using System.Threading;
using System.Threading.Tasks;
using Kusoge.Entities;
using Kusoge.GameStates;
using NUnit.Framework;
using UnityEngine;

namespace Kusoge.Tests
{
    public class PlayGameStateTests
    {
        [Test]
        public async Task PlayGameState_ShouldReturnGameOver_OnPlayerDeath()
        {
            using var cts = new CancellationTokenSource();
            var token = cts.Token;
            
            var player = new Player { hp = 100 };
            
            var beanLauncher = new BeanLauncher { launchRate = 10 };
            
            var inputProvider = new DummyPlayerInputProvider(player, beanLauncher);
            
            using var playGameState = new PlayGameState(
                player,
                beanLauncher,
                new DummyPlayerPresenter(),
                inputProvider,
                inputProvider,
                new DummyBeanPresenter());

            beanLauncher.Initialize();
            var result = await playGameState.Running(token);
            
            Assert.GreaterOrEqual(player.BeanEatenCount, 3, "Player should eat at least 3 beans.");
            
            Debug.Log($"Final Score: {player.BeanEatenCount} beans eaten, with {player.ComboCount} combo.");
            
            Assert.AreEqual(GameStateEnum.GameOver, result);
        }
    }
}