using System;
using Armadillo.Shared;

namespace Armadillo.Graph
{
    public class LevelStatisticsSlice
    {
        public LevelStatisticsSlice(int level, int count = 0)
        {
            Level = level;
            Count = count;
        }

        public int Level { get; set; }

        public int Count { get; set; }

        public string Color
        {
            get
            {
                return new NodeColor(new Subcase { Level = Level.ToString(), Status = "" }).Background;
            }
        }
    }
}

