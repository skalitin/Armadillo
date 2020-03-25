using System;
using Armadillo.Shared;

namespace Armadillo.Graph
{
    public class StatusStatisticsSlice
    {
        public StatusStatisticsSlice(string status, string color, int count = 0)
        {
            Status = status;
            Color = color;
            Count = count;
        }

        public string Status { get; set; }

        public string Color { get; set; }

        public int Count { get; set; }
    }
}

