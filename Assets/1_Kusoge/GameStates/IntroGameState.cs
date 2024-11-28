using System.Threading;
using System.Threading.Tasks;
using Kusoge.Interfaces;
using UnityEngine;

namespace Kusoge.GameStates
{
    public class IntroGameState : IGameState
    {
        private readonly Player player;
        private readonly BeanLauncher beanLauncher;
        private readonly IPlayerHealthPresenter playerHealthPresenter;
        private readonly IPlayerStatsPresenter playerStatsPresenter;
        private readonly IIntroPresenter introPresenter;
        
        public GameStateEnum Id => GameStateEnum.Intro;
        
        public IntroGameState(
            Player player,
            BeanLauncher beanLauncher,
            IPlayerHealthPresenter playerHealthPresenter,
            IPlayerStatsPresenter playerStatsPresenter,
            IIntroPresenter introPresenter)
        {
            this.player = player;
            this.beanLauncher = beanLauncher;
            this.playerHealthPresenter = playerHealthPresenter;
            this.playerStatsPresenter = playerStatsPresenter;
            this.introPresenter = introPresenter;
        }
        
        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            Debug.Log($"[{GetType().Name}] Showing intro Presenter");
            var showIntroTask = introPresenter.Show(cancellationToken);
            
            Debug.Log($"[{GetType().Name}] Initialize Player and BeanLauncher.");
            player.Initialize();
            beanLauncher.Initialize();
            
            Debug.Log($"[{GetType().Name}] Showing player health and stats.");
            playerHealthPresenter.Show(player.HealthPercentage);
            playerStatsPresenter.Show(player.GameStats);
            
            Debug.Log($"[{GetType().Name}] Waiting for intro to finish.");
            await showIntroTask;
            
            Debug.Log($"[{GetType().Name}] Intro finished. Returns GamePlay state.");
            return GameStateEnum.GamePlay;
        }
    }

    
}