using System;

namespace Kusoge
{
    public enum TileEnum
    {
        Void = -1,
        Movable = 0,
        Wall,
        Goal
    }
    
    [Serializable]
    public class MapData
    {
        public TileEnum[] tiles = Array.Empty<TileEnum>();

        public int width;
        public int height;
        
        public bool GoalEnabled { get; set; }
        
        public TileEnum this[int x, int y]
        {
            get => GetValueAsGrid(x, y);
            set => SetValueAsGrid(x, y, value);
        }
        
        public bool IsMovable(Point gridPoint)
        {
            if (gridPoint.x < 0 || gridPoint.x >= width ||
                gridPoint.y < 0 || gridPoint.y >= height) return false;
            return tiles[gridPoint.y * width + gridPoint.x] is TileEnum.Movable or TileEnum.Goal;
        }

        public TileEnum GetGrid(Point gridPoint)
        {
            return GetValueAsGrid(gridPoint.x, gridPoint.y);
        }

        public void SetGrid(Point gridPoint, TileEnum value)
        {
            SetValueAsGrid(gridPoint.x, gridPoint.y, value);
        }
        
        private TileEnum GetValueAsGrid(int x, int y)
        {
            var idx = y * width + x;
            return tiles[idx];
        }
        
        private void SetValueAsGrid(int x, int y, TileEnum value)
        {
            var idx = y * width + x;
            tiles[idx] = value;
        }
    }
}