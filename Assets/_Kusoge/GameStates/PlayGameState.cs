using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kusoge.GameStates
{
    public class PlayGameState : IGameState, IDisposable
    {
        // Entities
        private readonly GameManager gameManager;
        private readonly IReadOnlyCollection<Unit> units;
        private readonly IReadOnlyCollection<Mob> mobs;
        private readonly Player player;
        private readonly DeliverTarget deliverTarget;
        private readonly Item droppedItem;
        private readonly MapData mapData;

        // Handlers
        private readonly IPlayerMoveInputHandler playerMoveInputHandler;
        private readonly IPlayerCollisionHandler playerCollisionHandler;

        // Presenters
        private readonly IReadOnlyCollection<IUnitMovePresenter> unitMovePresenters;
        private readonly IMapPresenter mapPresenter;
        
        public GameStateEnum Id => GameStateEnum.GamePlay;
        
        private CancellationTokenSource cts;
        private CancellationToken Token => cts.Token;
        private bool reachedGoal;
        
        public PlayGameState(
            GameManager gameManager,
            IReadOnlyCollection<Unit> units,
            Player player, 
            DeliverTarget deliverTarget,
            Item droppedItem,
            MapData mapData,
            IPlayerMoveInputHandler playerMoveInputHandler,
            IPlayerCollisionHandler playerCollisionHandler,
            IReadOnlyCollection<IUnitMovePresenter> unitMovePresenters,
            IMapPresenter mapPresenter)
        {
            this.gameManager = gameManager;
            this.units = units;
            this.player = player;
            this.deliverTarget = deliverTarget;
            this.droppedItem = droppedItem;
            this.mapData = mapData;
            this.unitMovePresenters = unitMovePresenters;
            this.mapPresenter = mapPresenter;
            this.playerMoveInputHandler = playerMoveInputHandler;
            this.playerCollisionHandler = playerCollisionHandler;
            
            mobs = units.OfType<Mob>().ToArray();
        }
        
        public async ValueTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            HandlePlayerInput();
            HandlePlayerCollision();
            HandleGoalLifetime();

            var nextState = Id;

            while (!Token.IsCancellationRequested && nextState == Id)
            {
                HandleUnitMovement();
                await PresentMovement();
                nextState = EvaluateNextState();
                await Task.Yield();
            }
            
            // TODO: Cleanup state

            return nextState;
        }

        /// <summary>
        /// Player input changes the move state of the player.
        /// Then, we'd handle its movement along with other units.
        /// </summary>
        private async void HandlePlayerInput()
        {
            while (!Token.IsCancellationRequested)
            {
                var inputReceived = await playerMoveInputHandler.WaitInput(Token);
                switch (inputReceived)
                {
                    case MoveDirection.Left:
                        player.SetMoveStateFlag(MoveStateEnum.MoveLeft);
                        break;
                    case MoveDirection.Right:
                        player.SetMoveStateFlag(MoveStateEnum.MoveRight);
                        break;
                    case MoveDirection.None:
                        player.ResetLeftRightMoveFlag();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Player moves and collides with their surroundings.
        /// We'd handle collisions here and change states accordingly.
        /// </summary>
        private async void HandlePlayerCollision()
        {
            while (!Token.IsCancellationRequested)
            {
                var collisionId = await playerCollisionHandler.WaitForCollision(Token);
                reachedGoal = collisionId == (int)TileEnum.Goal && mapData.GoalEnabled;
                if (reachedGoal)
                {
                    cts.Cancel();
                    return;
                }

                player.SetMoveStateFlag(MoveStateEnum.MoveUp); // default auto move direction

                // check collisions with items
                var gotItem = collisionId == droppedItem.id;
                if (gotItem)
                {
                    player.PossessedItem = droppedItem.id;
                }

                // check collisions with mobs
                var collidedMob = mobs.FirstOrDefault(mob => mob.id == collisionId);
                if (collidedMob == null) continue;
                if (collidedMob.id == deliverTarget.id)
                {
                    if (player.PossessedItem != deliverTarget.requestedItem) continue;
                    player.PossessedItem = 0;
                }
                else
                {
                    player.SetMoveStateFlag(MoveStateEnum.MoveDown); // pushed-away
                }
            }
        }

        private async void HandleGoalLifetime()
        {
            mapData.GoalEnabled = false;
            while (!Token.IsCancellationRequested && !gameManager.IsAllGoalSkipped)
            {
                await Task.Delay(TimeSpan.FromSeconds(gameManager.GoalIntervalSecond), Token);
                mapData.GoalEnabled = true;
                await Task.Delay(TimeSpan.FromSeconds(gameManager.GoalAvailableSecond), Token);
                mapData.GoalEnabled = false;
                gameManager.SkippedGoalCount++;
            }
            cts.Cancel();
        }

        /// <summary>
        /// Validate move against maps and handle all unit movements.
        /// </summary>
        private void HandleUnitMovement()
        {
            foreach (var unit in units)
            {
                var predictedMove = unit.PredictMovement();
                var isValid = mapData.IsMovable(predictedMove);
                if (!isValid) continue;
                unit.Move(predictedMove);
            }
        }

        /// <summary>
        /// Handle the presentation of movement for all units.
        /// </summary>
        private async ValueTask PresentMovement()
        {
            var showMapTask = mapPresenter.Show(mapData.tiles, mapData.width, mapData.GoalEnabled, Token).AsTask();
            var moveTasks = units.SelectMany(unit =>
                unitMovePresenters.Select(
                    presenter => presenter.Show(unit.id, unit.Position, unit.CurrentMoveState)), (_, task) => task.AsTask()).ToArray();
            var tasks = moveTasks.Append(showMapTask);
            await Task.WhenAll(tasks);
        }

        private GameStateEnum EvaluateNextState()
        {
            if (reachedGoal)
                return GameStateEnum.LevelClear;
            if (gameManager.IsAllGoalSkipped)
                return GameStateEnum.GameOver;
            return Id;
        }

        public void Dispose()
        {
            cts.Dispose();
            cts = null;
        }
    }

    public interface IUnitMovePresenter
    {
        ValueTask Show(int id, Point target, MoveStateEnum moveState);
    }

    public interface IMapPresenter
    {
        ValueTask Show(TileEnum[] gridData, int width, bool goalEnabled, CancellationToken cancellationToken = default);
    }

    public interface IPlayerCollisionHandler
    {
        ValueTask<int> WaitForCollision(CancellationToken cancellationToken = default);
    }
    
    public interface IPlayerMoveInputHandler
    {
        ValueTask<MoveDirection> WaitInput(CancellationToken cancellationToken = default);
    }
    
    public enum MoveDirection
    {
        Left = -1,
        None,
        Right = 1
    }
}