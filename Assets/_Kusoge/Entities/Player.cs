using System;

namespace Kusoge
{
    [Serializable]
    public class Player : Unit
    {
        public int PossessedItem { get; set; }
        
        public void SetMoveStateFlag(MoveStateEnum flag)
        {
            switch (flag)
            {
                case MoveStateEnum.None:
                case MoveStateEnum.Idle:
                    CurrentMoveState = flag;
                    return;
                case MoveStateEnum.MoveUp:
                    CurrentMoveState &= ~MoveStateEnum.MoveDown;
                    break;
                case MoveStateEnum.MoveLeft:
                    CurrentMoveState &= ~MoveStateEnum.MoveRight;
                    break;
                case MoveStateEnum.MoveRight:
                    CurrentMoveState &= ~MoveStateEnum.MoveLeft;
                    break;
                case MoveStateEnum.MoveDown:
                    CurrentMoveState &= ~MoveStateEnum.MoveUp;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flag), flag, null);
            }
            CurrentMoveState |= flag;
        }

        public void ResetLeftRightMoveFlag()
        {
            CurrentMoveState &= ~MoveStateEnum.MoveLeft & ~MoveStateEnum.MoveRight;
        }

        public void ResetUpDownMoveFlag()
        {
            CurrentMoveState &= ~MoveStateEnum.MoveUp & ~MoveStateEnum.MoveDown;
        }
    }
}