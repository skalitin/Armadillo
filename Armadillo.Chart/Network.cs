using System;

namespace Armadillo.Chart
{
    public class Node
    {
        public Node(string id, string label, string group = "subcases")
        {
            Id = id;
            Label = label;
            Group = group;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public string Color { get; set; }
        public int? Size { get; set; }
    }

    public class Edge
    {
        public Edge(string from = null, string to = null)
        {
            From = from;
            To = to;
        }

        public string From { get; set; }
        public string To { get; set; }
    }

    public class Network
    {
        public Node[] Nodes { get; set; } = new Node[] {};
        public Edge[] Edges { get; set; } = new Edge[] {};
    }
}