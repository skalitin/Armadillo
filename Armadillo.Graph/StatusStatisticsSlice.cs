using System;
using Armadillo.Shared;

namespace Armadillo.Graph
{
    public class StatusStatisticsSlice
    {
        public StatusStatisticsSlice(string status, int count = 0)
        {
            Status = status;
            Count = count;
        }

        public string Status { get; set; }

        public int Count { get; set; }

        public string Color
        {
            get
            {
                return new NodeColor(new Subcase
                {
                    Level = "3",
                    Created = DateTime.UtcNow,
                    Status = Status 
                }).Border;
            }
        }
    }
}

