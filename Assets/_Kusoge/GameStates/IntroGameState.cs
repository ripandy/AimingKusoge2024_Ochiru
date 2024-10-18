using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.GameStates
{
    public class IntroGameState : IGameState
    {
        private readonly GameManager gameManager;
        private readonly IIntroPresenter introPresenter;
        
        public GameStateEnum Id => GameStateEnum.Intro;
        
        public IntroGameState(
            GameManager gameManager,
            IIntroPresenter introPresenter)
        {
            this.gameManager = gameManager;
            this.introPresenter = introPresenter;
        }
        
        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            var showIntroTask = introPresenter.Show();
            
            gameManager.InitializeLevel();
            
            await showIntroTask;
            return GameStateEnum.GamePlay;
        }
    }

    public interface IIntroPresenter
    {
        ValueTask Show();
    }
}