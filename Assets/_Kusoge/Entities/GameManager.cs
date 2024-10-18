using System;

namespace Kusoge
{
    [Serializable]
    public class GameManager
    {
        private const int MaxGoalSkip = 3;
        
        public int level;
        public int SkippedGoalCount { get; set; }
        
        public float GoalIntervalSecond { get; set; } = 5.0f;
        public float GoalAvailableSecond { get; set; } = 3.0f;

        public void InitializeLevel()
        {
            SkippedGoalCount = 0;
        }

        public bool IsAllGoalSkipped => SkippedGoalCount >= MaxGoalSkip;
    }
}