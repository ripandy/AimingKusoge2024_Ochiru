using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kusoge.GameStates;
using NUnit.Framework;
using UnityEngine;

namespace Kusoge.Tests
{
    public class PlayGameStateTests
    {
        [TestCaseSource(nameof(CreatePlayGameStateTestCaseData))]
        public async Task PlayGameState_ShouldReturnNextLevel_OnGoalReached(
            Player player,
            DeliverTarget deliverTarget,
            Item item,
            IReadOnlyCollection<Unit> units,
            IReadOnlyCollection<IUnitMovePresenter> unitMovePresenters,
            MapData mapData)
        {
            using var cts = new CancellationTokenSource();
            
            // Test dummy player move input handler
            var dummyPlayerMoveInputHandler = new DummyPlayerMoveInputHandler();

            var i = 1;
            while (!cts.IsCancellationRequested && i < 5)
            {
                var direction = await dummyPlayerMoveInputHandler.WaitInput(cts.Token);
                var expected = i % 3 - 1;
                Assert.AreEqual((int)direction, expected);
                i++;
            }
            
            // Test dummy player collision handler
            var dummyPlayerCollisionHandler = new DummyPlayerCollisionHandler(player, item, units.OfType<Mob>().ToArray(), mapData);
            
            var collidedId = await dummyPlayerCollisionHandler.WaitForCollision(cts.Token);
            Assert.AreEqual(-1, collidedId);

            player.Position = item.Position;
            collidedId = await dummyPlayerCollisionHandler.WaitForCollision(cts.Token);
            Assert.AreEqual(item.id, collidedId);
            
            player.Position = deliverTarget.Position;
            collidedId = await dummyPlayerCollisionHandler.WaitForCollision(cts.Token);
            Assert.AreEqual(deliverTarget.id, collidedId);

            var mob = units.First(u => u.id != player.id);
            player.Position = mob.Position;
            collidedId = await dummyPlayerCollisionHandler.WaitForCollision(cts.Token);
            Assert.AreEqual(mob.id, collidedId);
            
            player.Position = new Point(1, 0);
            i = 0;
            while (!cts.IsCancellationRequested && i < 5)
            {
                collidedId = await dummyPlayerCollisionHandler.WaitForCollision(cts.Token);
                Assert.AreEqual(-1, collidedId);
                i++;
            }
            
            var playGameState = new PlayGameState(
                new GameManager { level = 1, GoalIntervalSecond = 3f, GoalAvailableSecond = 5f },
                units,
                player,
                deliverTarget,
                item,
                mapData,
                dummyPlayerMoveInputHandler,
                dummyPlayerCollisionHandler,
                unitMovePresenters,
                new DummyMapPresenter(player, deliverTarget, item, units.OfType<Mob>().ToArray()));

            var nextState = await playGameState.Running(cts.Token);
            
            Assert.AreEqual(GameStateEnum.LevelClear, nextState);
        }

        private static IEnumerable<TestCaseData> CreatePlayGameStateTestCaseData()
        {
            var player = new Player { id = 99, startingPosition = new Point(1, 0), Position = new Point(1, 0) };
            var deliverTarget = new DeliverTarget
            {
                id = 98,
                route = new []
                {
                    MoveStateEnum.MoveDown
                },
                startingPosition = new Point(0, 4),
                Position = new Point(0, 4)
            };
            var item = new Item { id = 1000, Position = new Point(0, 2) };
            var units = Enumerable.Range(0, 2).Select(i =>
                {
                    var pos = new Point(3, 1 + (i % 2 == 0 ? 0 : 4));
                    return new Mob
                    {
                        id = i + 100,
                        route = new[]
                        {
                            MoveStateEnum.MoveLeft,
                            MoveStateEnum.MoveLeft,
                            MoveStateEnum.MoveUp,
                            MoveStateEnum.MoveUp,
                            MoveStateEnum.MoveUp
                        },
                        size = MobSizeEnum.Small,
                        startingPosition = pos,
                        Position = pos
                    } as Unit;
                })
                .ToList();
            units.Insert(0, player);
            units.Insert(1, deliverTarget);
            
            var unitMovePresenters = units.Select(_ => new DummyUnitMovePresenter()).ToList();
            
            var tileData = new[] {
                0, 0, 0, 1, -1,
                0, 0, 0, 2, -1,
                0, 0, 0, 1, -1,
                0, 0, 0, 1, -1,
                0, 0, 0, 1, -1,
                0, 0, 0, 2, -1,
                0, 2, 0, 1, -1
            }.Cast<TileEnum>().ToArray();
            
            var mapData = new MapData { tiles = tileData, width = 5, height = 7};

            yield return new TestCaseData(player, deliverTarget, item, units, unitMovePresenters, mapData);
        }
        
        private class DummyPlayerMoveInputHandler : IPlayerMoveInputHandler
        {
            private int count;
            public async ValueTask<MoveDirection> WaitInput(CancellationToken cancellationToken = default)
            {
                await Task.Delay(1000, cancellationToken);
                count++;
                var direction = count % 3 - 1;
                return (MoveDirection)direction;
            }
        }
        
        private class DummyPlayerCollisionHandler : IPlayerCollisionHandler
        {
            private readonly Player player;
            private readonly Item item;
            private readonly IReadOnlyCollection<Mob> mobs;
            private readonly MapData mapData;

            public DummyPlayerCollisionHandler(Player player, Item item, IReadOnlyCollection<Mob> mobs, MapData mapData)
            {
                this.player = player;
                this.item = item;
                this.mobs = mobs;
                this.mapData = mapData;
            }
            
            public async ValueTask<int> WaitForCollision(CancellationToken cancellationToken = default)
            {
                await Task.Yield();

                if (mapData.GetGrid(player.Position) == TileEnum.Goal)
                    return (int)TileEnum.Goal;
                
                if (player.Position == item.Position)
                    return item.id;
                
                var collidedMob = mobs.FirstOrDefault(mob => mob.Position == player.Position);
                return collidedMob?.id ?? -1;
            }
        }
        
        private class DummyMapPresenter : IMapPresenter
        {
            private readonly Player player;
            private readonly DeliverTarget deliverTarget;
            private readonly Item item;
            private readonly IReadOnlyCollection<Mob> mobs;
            
            public DummyMapPresenter(Player player, DeliverTarget deliverTarget, Item item, IReadOnlyCollection<Mob> mobs)
            {
                this.player = player;
                this.deliverTarget = deliverTarget;
                this.item = item;
                this.mobs = mobs;
            }
            
            public async ValueTask Show(TileEnum[] gridData, int width, bool goalEnabled, CancellationToken cancellationToken = default)
            {
                await Task.Delay(100, cancellationToken: cancellationToken);
                var sb = new StringBuilder();
                sb.AppendLine("P: Player, D: DeliverTarget, I: Item, M: Mob, 2: Goal");
                for (var i = 0; i < gridData.Length; i++)
                {
                    var position = new Point(i % width, i / width);
                    
                    if (player.Position == position)
                        sb.Append("P");
                    else if (deliverTarget.Position == position)
                        sb.Append("D");
                    else if (item.Position == position)
                        sb.Append("I");
                    else if (mobs.Any(mob => mob.Position == position))
                        sb.Append("M");
                    else if (gridData[i] == TileEnum.Goal && goalEnabled)
                        sb.Append("=");
                    else
                        sb.Append((int)gridData[i]);
                    
                    if ((i + 1) % width == 0)
                        sb.AppendLine();
                    else
                        sb.Append(", ");
                }
                Debug.Log(sb.ToString());
            }
        }
        
        private class DummyUnitMovePresenter : IUnitMovePresenter
        {
            public async ValueTask Show(int id, Point target, MoveStateEnum moveState)
            {
                await Task.Delay(1000);
            }
        }
    }
}