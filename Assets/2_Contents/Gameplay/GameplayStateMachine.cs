using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doinject;
using Kusoge;
using Kusoge.GameStates;
using Soar.Events;
using UnityEngine;

namespace Contents.Gameplay
{
    public class GameplayStateMachine : MonoBehaviour, IInjectableComponent
    {
        [SerializeField] private GameEvent<string> setNextStateEvent;
        
        [SerializeField] private GameStateEnum initialState = GameStateEnum.Intro;
        [SerializeField] private string gameEndSceneName = "MainMenu";
        
        private IReadOnlyDictionary<GameStateEnum, IGameState> gameStates;
        
        [Inject]
        public void Construct(IntroGameState introGameState, PlayGameState playGameState, GameOverGameState gameOverGameState)
        {
            gameStates = new Dictionary<GameStateEnum, IGameState>
            {
                { introGameState.Id, introGameState },
                { playGameState.Id, playGameState },
                { gameOverGameState.Id, gameOverGameState }
            };
        }
        
        private void Start() => Run().Forget();

        private async UniTaskVoid Run()
        {
            var activeState = initialState;
            while (activeState != GameStateEnum.None && !destroyCancellationToken.IsCancellationRequested)
            {
                Debug.Log($"Running {activeState}");
                activeState = await gameStates[activeState].Running(destroyCancellationToken);
            }
            
            setNextStateEvent.Raise(gameEndSceneName);
        }
    }
}