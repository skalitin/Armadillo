using System;
using Armadillo.Shared;
using System.Collections.Generic;

namespace Armadillo.Graph
{
    public class NodeColor
    {
        public NodeColor(Subcase subcase)
        {
            // Source https://www.w3schools.com/colors/colors_trends.asp
            var colors = new Dictionary<string, string>
            {
                {"1", "#9B2335"},
                {"2", "#EFC050"},
                {"3", "#5B5EA6"},
                {"4", "#DFCFBE"},
            };

            Background = colors[subcase.Level];

            var borders = new Dictionary<string, string>
            {
                {"Update from Support", "#98B4D4"},
                {"Fix Provided", "#009B77"},
                {"Waiting Support Response", "#E15D44"},
                {"Assigned", "#F7CAC9"},
                {"Investigating", "#7FCDCD"},
            };

            string border = null;
            if(borders.TryGetValue(subcase.Status, out border))
            {
                Border = border;
            }
            else
            {
                Border = "#98B4D4";
            }

            // if(DateTime.UtcNow - subcase.Created > TimeSpan.FromDays(30))
        }

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