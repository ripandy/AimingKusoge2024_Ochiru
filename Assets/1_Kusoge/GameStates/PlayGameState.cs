using System;
using System.Threading;
using System.Threading.Tasks;
using Kusoge.Interfaces;
using UnityEngine;

namespace Kusoge.GameStates
{
    public class PlayGameState : IGameState, IDisposable
    {
        private readonly Player player;
        private readonly BeanLauncher beanLauncher;
        private readonly IPlayerHealthPresenter playerHealthPresenter;
        private readonly IPlayerStatsPresenter playerStatsPresenter;
        private readonly IPlayerDirectionPresenter playerDirectionPresenter;
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
            IPlayerHealthPresenter playerHealthPresenter,
            IPlayerStatsPresenter playerStatsPresenter,
            IPlayerDirectionPresenter playerDirectionPresenter,
            IPlayerDirectionInputProvider playerDirectionInputProvider,
            IPlayerBiteInputProvider playerBiteInputProvider,
            IBeanPresenter beanPresenter)
        {
            this.player = player;
            this.beanLauncher = beanLauncher;
            this.playerHealthPresenter = playerHealthPresenter;
            this.playerStatsPresenter = playerStatsPresenter;
            this.playerDirectionPresenter = playerDirectionPresenter;
            this.playerDirectionInputProvider = playerDirectionInputProvider;
            this.playerBiteInputProvider = playerBiteInputProvider;
            this.beanPresenter = beanPresenter;
        }
        
        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            Debug.Log($"[{GetType().Name}] Initializing cancellation token source and task completion source.");
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            tcs = new TaskCompletionSource<bool>();
            
            Debug.Log($"[{GetType().Name}] Calling HandlePlayerDirectionInput.");
            HandlePlayerDirectionInput();
            
            Debug.Log($"[{GetType().Name}] Calling ExecuteBeanLauncher.");
            ExecuteBeanLauncher();
            
            Debug.Log($"[{GetType().Name}] Calling HandlePlayerBiteInput.");
            HandlePlayerBiteInput();

            Debug.Log($"[{GetType().Name}] Waiting for task completion source to finish.");
            await tcs.Task;
            await Task.Yield();
            
            Debug.Log($"[{GetType().Name}] Returns GameOver state.");
            return GameStateEnum.GameOver;
        }

        private async void HandlePlayerDirectionInput()
        {
            try
            {
                Debug.Log($"[{GetType().Name}] Waiting for player direction input.");
                var direction = await playerDirectionInputProvider.WaitForDirectionInput(Token);
                
                Debug.Log($"[{GetType().Name}] Player direction input received: {direction}");
                player.Direction = direction;
                
                Debug.Log($"[{GetType().Name}] Showing player direction.");
                playerDirectionPresenter.Show(player.Direction);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[{GetType().Name}] HandlePlayerDirectionInput OperationCanceledException caught, meaning state is over. Showing player default direction forward.");
                // state's over
                playerDirectionPresenter.Show(DirectionEnum.Forward);
            }
            
            Debug.Log($"[{GetType().Name}] HandlePlayerDirectionInput Checking if cancellation token source is null or token cancellation is requested.");
            if (cts == null || Token.IsCancellationRequested)
            {
                Debug.Log($"[{GetType().Name}] HandlePlayerDirectionInput Cancellation token source is null or token cancellation is requested. Exiting.");
                return;
            }
            
            Debug.Log($"[{GetType().Name}] Recursive call to HandlePlayerDirectionInput.");
            HandlePlayerDirectionInput(); // Recursive Call
        }

        private async void ExecuteBeanLauncher()
        {
            try
            {
                Debug.Log($"[{GetType().Name}] Launching bean.");
                LaunchBean();
                
                Debug.Log($"[{GetType().Name}] Waiting for bean launcher delay {beanLauncher.LaunchDelay}.");
                await Task.Delay(beanLauncher.LaunchDelay, Token);
            }
            catch (OperationCanceledException)
            {
                // ignore
                Debug.Log($"[{GetType().Name}] ExecuteBeanLauncher OperationCanceledException caught, meaning state is over.");
            }
            
            Debug.Log($"[{GetType().Name}] ExecuteBeanLauncher Checking if cancellation token source is null or token cancellation is requested.");
            if (cts == null || Token.IsCancellationRequested)
            {
                Debug.Log($"[{GetType().Name}] ExecuteBeanLauncher Cancellation token source is null or token cancellation is requested. Exiting.");
                return;
            }
            
            Debug.Log($"[{GetType().Name}] Recursive call to ExecuteBeanLauncher.");
            ExecuteBeanLauncher(); // Recursive Call
        }

        private async void LaunchBean()
        {
            try
            {
                Debug.Log($"[{GetType().Name}] Launching bean...");
                var bean = beanLauncher.LaunchBean();
                Debug.Log($"[{GetType().Name}] Showing bean {bean.Id}, {bean.ThrowDirection}. Waiting for bean to drop.");
                var dropped = await beanPresenter.Show(bean.Id, bean.ThrowDirection, Token);
                await Task.Yield();
                Debug.Log($"[{GetType().Name}] Bean has finished launched. Removing bean-{bean.Id} dropped: {dropped}");
                beanLauncher.RemoveBean(bean.Id);
                if (!dropped) return;
                
                Debug.Log($"[{GetType().Name}] Bean was dropped, hide it. Eaten beans are hidden by the player bite input.");
                // Bean was dropped, hide it. Eaten beans are hidden by the player bite input.
                beanPresenter.Hide(bean.Id);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[{GetType().Name}] LaunchBean OperationCanceledException caught, meaning state is over. Set task completion source result to false.");
                tcs.TrySetResult(false);
                return;
            }
            
            Debug.Log($"[{GetType().Name}] Player was not able to eat the bean. Damaging player.");
            player.Damaged();
            playerHealthPresenter.Show(player.HealthPercentage);
            
            Debug.Log($"[{GetType().Name}] Checking if player is alive {player.IsAlive}.");
            if (player.IsAlive) return;

            Debug.Log($"[{GetType().Name}] Player is dead. Cancelling cancellation token source and setting task completion source result to true.");
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            tcs.TrySetResult(true);
        }

        private async void HandlePlayerBiteInput()
        {
            try
            {
                Debug.Log($"[{GetType().Name}] Waiting for player bite input.");
                var bittenId = await playerBiteInputProvider.WaitForBite(Token);
                Debug.Log($"[{GetType().Name}] Player bite input received: {bittenId}");
                if (beanLauncher.TryGetBean(bittenId, out var bittenBean))
                {
                    Debug.Log($"[{GetType().Name}] Player was able to eat the bean. EatBean, Show player health and stats, Update bean launcher.");
                    player.EatBean();
                    playerHealthPresenter.Show(player.HealthPercentage);
                    playerStatsPresenter.Show(player.GameStats);
                    beanLauncher.UpdateLaunchRate(player.ComboCount);
                    
                    Debug.Log($"[{GetType().Name}] Hide bean-{bittenBean.Id}.");
                    beanPresenter.Hide(bittenBean.Id);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
                Debug.Log($"[{GetType().Name}] HandlePlayerBiteInput OperationCanceledException caught, meaning state is over.");
            }
            
            Debug.Log($"[{GetType().Name}] HandlePlayerBiteInput Checking if cancellation token source is null or token cancellation is requested.");
            if (cts == null || Token.IsCancellationRequested)
            {
                Debug.Log($"[{GetType().Name}] HandlePlayerBiteInput Cancellation token source is null or token cancellation is requested. Exiting.");
                return;
            }
            
            Debug.Log($"[{GetType().Name}] Recursive call to HandlePlayerBiteInput.");
            HandlePlayerBiteInput(); // Recursive Call
        }

        public void Dispose()
        {
            Debug.Log($"[{GetType().Name}] Dispose. Disposing cancellation token source and task completion source.");
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            tcs = null;
        }
    }
}