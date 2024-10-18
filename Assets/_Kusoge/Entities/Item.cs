using System;

namespace Kusoge
{
    [Serializable]
    public class Item
    {
        public int id;
        public Point Position { get; set; }
    }
}