using System.Threading;
using System.Threading.Tasks;
using Kusoge.Entities;
using Kusoge.GameStates;
using NUnit.Framework;

namespace Kusoge.Tests
{
    public class GameStateTests
    {
        [TestCase(1)]
        [TestCase(3)]
        public async Task GameStates_ShouldRunAllStatesProperly(int playCount)
        {
            using var cts = new CancellationTokenSource();
            var token = cts.Token;
            
            // Entities
            var player = new Player { hp = 100 };
            var beanLauncher = new BeanLauncher { launchRate = 10 };
            
            // Game States
            var introGameState = new IntroGameState(player, beanLauncher, new DummyIntroPresenter());

            var inputProvider = new DummyPlayerInputProvider(player, beanLauncher);
            
            using var playGameState = new PlayGameState(
                player,
                beanLauncher,
                new DummyPlayerPresenter(),
                inputProvider,
                inputProvider,
                new DummyBeanPresenter());
            
            var gameOverGameState = new GameOverGameState(player, new DummyGameOverPresenter());

            // Run game states
            var count = 0;
            var nextState = GameStateEnum.Intro;

            while (count < playCount && nextState == GameStateEnum.Intro)
            {
                count++;
                
                nextState = await introGameState.Running(token);
                Assert.AreEqual(GameStateEnum.GamePlay, nextState);
                
                nextState = await playGameState.Running(token);
                Assert.AreEqual(GameStateEnum.GameOver, nextState);

                // For test purpose, set combo to 99 to simulate exit game instead of restarting.
                if (count == 2)
                {
                    player.ComboCount = 99;
                }
                
                nextState = await gameOverGameState.Running(token);
                Assert.AreEqual(count == 2 ? GameStateEnum.None : GameStateEnum.Intro, nextState);
            }
            
            if (playCount == 1)
            {
                Assert.AreEqual(count, playCount);
            }
            else
            {
                Assert.AreNotEqual(count, playCount);
            }
        }
    }
}