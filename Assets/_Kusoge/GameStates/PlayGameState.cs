using System;
using System.Threading;
using System.Threading.Tasks;
using Kusoge.Entities;

namespace Kusoge.GameStates
{
    public class PlayGameState : IGameState, IDisposable
    {
        private readonly Player player;
        private readonly BeanLauncher beanLauncher;
        private readonly IPlayerDirectionInputProvider playerDirectionInputProvider;
        private readonly IPlayerBiteInputProvider playerBiteInputProvider;
        private readonly IBeanPresenter beanPresenter;

        public GameStateEnum Id => GameStateEnum.GamePlay;

        private CancellationTokenSource cts;
        private CancellationToken Token => cts.Token;

        private TaskCompletionSource<bool> tcs;

        public PlayGameState(
            Player player,
            BeanLauncher beanLauncher,
            IPlayerDirectionInputProvider playerDirectionInputProvider,
            IPlayerBiteInputProvider playerBiteInputProvider,
            IBeanPresenter beanPresenter)
        {
            this.player = player;
            this.beanLauncher = beanLauncher;
            this.playerDirectionInputProvider = playerDirectionInputProvider;
            this.playerBiteInputProvider = playerBiteInputProvider;
            this.beanPresenter = beanPresenter;
        }
        
        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            tcs = new TaskCompletionSource<bool>();
            
            HandlePlayerDirectionInput();
            InitializeBeanLauncher();
            HandlePlayerBiteInput();

            await tcs.Task;
            await Task.Yield();
            
            return GameStateEnum.GameOver;
        }

        private async void HandlePlayerDirectionInput()
        {
            try
            {
                var direction = await playerDirectionInputProvider.WaitForDirectionInput(Token);
                player.Direction = direction;
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            
            if (Token.IsCancellationRequested) return;
            HandlePlayerDirectionInput(); // Recursive Call
        }

        private async void InitializeBeanLauncher()
        {
            try
            {
                // TODO: control launch timer
                LaunchBeans();
                await Task.Delay(beanLauncher.LaunchDelay, Token);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            
            if (Token.IsCancellationRequested) return;
            InitializeBeanLauncher(); // Recursive Call
        }

        private async void LaunchBeans()
        {
            try
            {
                var bean = beanLauncher.LaunchBean();
                var dropped = await beanPresenter.Show(bean.Id, bean.ThrowDirection, Token);
                beanLauncher.RemoveBean(bean.Id);
                if (!dropped) return;
            }
            catch (TaskCanceledException)
            {
                tcs.TrySetResult(false);
                return;
            }
            
            player.Damaged();
            if (player.IsAlive) return;

            cts?.Cancel();
            tcs.TrySetResult(true);
        }

        private async void HandlePlayerBiteInput()
        {
            try
            {
                var bittenId = await playerBiteInputProvider.WaitForBite(Token);
                if (beanLauncher.TryGetBean(bittenId, out var bittenBean))
                {
                    player.EatBean();
                    beanPresenter.Hide(bittenBean.Id);
                }
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            
            if (Token.IsCancellationRequested) return;
            HandlePlayerBiteInput(); // Recursive Call
        }

        public void Dispose()
        {
            cts?.Dispose();
            cts = null;
        }
    }
    
    public interface IPlayerDirectionInputProvider
    {
        ValueTask<Player.DirectionEnum> WaitForDirectionInput(CancellationToken cancellationToken = default);
    }
    
    public interface IPlayerBiteInputProvider
    {
        ValueTask<int> WaitForBite(CancellationToken cancellationToken = default);
    }
}