using System;

namespace Armadillo.Graph
{
    public class Slice
    {
        public Slice(int level, int count, string color = null)
        {
            Level = level;
            Count = count;
            Color = color;
        }

        public int Level { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }
    }
}

