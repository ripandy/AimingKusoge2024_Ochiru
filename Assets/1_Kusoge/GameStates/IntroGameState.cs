using System.Threading;
using System.Threading.Tasks;
using Kusoge.Entities;
using Kusoge.Interfaces;

namespace Kusoge.GameStates
{
    public class IntroGameState : IGameState
    {
        private readonly Player player;
        private readonly BeanLauncher beanLauncher;
        private readonly IIntroPresenter introPresenter;
        
        public GameStateEnum Id => GameStateEnum.Intro;
        
        public IntroGameState(
            Player player,
            BeanLauncher beanLauncher,
            IIntroPresenter introPresenter)
        {
            this.player = player;
            this.beanLauncher = beanLauncher;
            this.introPresenter = introPresenter;
        }
        
        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            var showIntroTask = introPresenter.Show(cancellationToken);
            
            player.Initialize();
            beanLauncher.Initialize();
            
            await showIntroTask;
            return GameStateEnum.GamePlay;
        }
    }

    
}