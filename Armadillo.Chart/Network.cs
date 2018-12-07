using System;

namespace Armadillo.Chart
{
    public class Node
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }

    public class Edge
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class Network
    {
        public Node[] Nodes { get; set; }
        public Edge[] Edges { get; set; }
    }
}