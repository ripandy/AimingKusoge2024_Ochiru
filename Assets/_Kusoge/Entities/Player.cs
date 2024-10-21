using System;

namespace Kusoge.Entities
{
    [Serializable]
    public class Player
    {
        public int hp = 100;
        
        public int CurrentHp { get; set; }
        public int BeanEatenCount { get; set; }
        public int ComboCount { get; set; }
        
        public DirectionEnum Direction { get; set; }

        public bool IsAlive => CurrentHp > 0;

        private const int BeanHeal = 1;
        private const int BeanDamage = BeanHeal * 20;
        
        public void Initialize()
        {
            CurrentHp = hp;
            BeanEatenCount = 0;
            ComboCount = 0;
            Direction = DirectionEnum.Forward;
        }

        public void EatBean()
        {
            BeanEatenCount++;
            ComboCount++;
            CurrentHp = Math.Min(CurrentHp + BeanHeal, hp);
        }

        public void Damaged()
        {
            ComboCount = 0;
            CurrentHp = Math.Max(CurrentHp - BeanDamage, 0);
        }
        
        public enum DirectionEnum
        {
            Forward = 0,
            Left = -1,
            Right = 1
        }
    }
}