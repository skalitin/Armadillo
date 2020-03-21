using System;

namespace Armadillo.Graph
{
    public class NodeColor
    {
        public NodeColor(string background, string border = "")
        {
            Background = background;
            Border = border;
        }

        public string Background { get; set; }
        public string Border { get; set; }
    }

    public class Node
    {
        public Node(string id, string label, string group = "subcases")
        {
            Id = id;
            Label = label;
            Group = group;
            BorderWidth = 0;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public NodeColor Color { get; set; }
        public int Value { get; set; }
        public int BorderWidth { get; set; }
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