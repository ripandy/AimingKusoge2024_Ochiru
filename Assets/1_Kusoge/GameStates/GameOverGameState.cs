using System.Threading;
using System.Threading.Tasks;
using Kusoge.DataTransferObjects;
using Kusoge.Interfaces;
using UnityEngine;

namespace Kusoge.GameStates
{
    public class GameOverGameState : IGameState
    {
        private readonly Player player;
        private readonly IGameOverPresenter gameOverPresenter;
        
        public GameStateEnum Id => GameStateEnum.GameOver;
        
        public GameOverGameState(Player player, IGameOverPresenter gameOverPresenter)
        {
            this.player = player;
            this.gameOverPresenter = gameOverPresenter;
        }

        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            Debug.Log($"[{GetType().Name}] Initializing statistics.");
            var stats = new GameStatsDto(player.BeanEatenCount, player.ComboCount);
            
            Debug.Log($"[{GetType().Name}] Showing game over presenter. statistics: {stats.Score}, {stats.Combo}");
            var restart = await gameOverPresenter.Show(stats, cancellationToken);
            
            Debug.Log($"[{GetType().Name}] Game over presenter finished. Restart: {restart}");
            return restart ? GameStateEnum.Intro : GameStateEnum.None;
        }
    }
}