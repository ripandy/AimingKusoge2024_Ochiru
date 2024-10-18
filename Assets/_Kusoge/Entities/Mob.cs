using System;

namespace Kusoge
{
    [Serializable]
    public class Mob : Unit
    {
        public MoveStateEnum[] route;
        public MobSizeEnum size;

        private int routeIndex;
            
        public override Point PredictMovement()
        {
            routeIndex = routeIndex + 1 >= route.Length ? 0 : routeIndex + 1;
            var nextMove = route[routeIndex];
            CurrentMoveState = MoveStateEnum.Idle | route[routeIndex++];
            return base.PredictMovement();
        }
    }
    
    public enum MobSizeEnum
    {
        Solo = 0,
        Small,
        Medium,
        Large
    }
}