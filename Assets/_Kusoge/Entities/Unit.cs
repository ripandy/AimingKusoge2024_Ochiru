using System;

namespace Kusoge
{
    [Serializable]
    public abstract class Unit
    {
        public int id;
        public Point startingPosition;
        
        public MoveStateEnum CurrentMoveState { get; set; }
        
        public Point Position { get; set; }
        
        public virtual Point PredictMovement()
        {
            if (CurrentMoveState is MoveStateEnum.None or MoveStateEnum.Idle) return Position;
            
            var nextPosition = Position;
            
            if (CurrentMoveState.HasFlag(MoveStateEnum.MoveUp))
            {
                nextPosition.y++;
            }
            else if (CurrentMoveState.HasFlag(MoveStateEnum.MoveDown))
            {
                nextPosition.y--;
            }
            
            if (CurrentMoveState.HasFlag(MoveStateEnum.MoveLeft))
            {
                nextPosition.x--;
            }
            else if (CurrentMoveState.HasFlag(MoveStateEnum.MoveRight))
            {
                nextPosition.x++;
            }

            return nextPosition;
        }

        public void Move(Point newPosition)
        {
            Position = newPosition;
        }

        public void ResetPosition()
        {
            CurrentMoveState = MoveStateEnum.Idle;
            Position = startingPosition;
        }
    }
    
    [Flags]
    public enum MoveStateEnum
    {
        None = -1,
        Idle = 0,
        MoveUp = 1 << 2,
        MoveLeft = 2 << 2,
        MoveRight = 3 << 2,
        MoveDown = 4 << 2
    }
}