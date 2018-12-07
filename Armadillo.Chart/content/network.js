function RenderNetwork(network) {
    console.log(network);

    var nodes = new vis.DataSet(network.nodes);
    var edges = new vis.DataSet(network.edges);
    var data = {
        nodes: nodes,
        edges: edges
    };

    var options = {};

    var container = document.getElementById('network');
    var network = new vis.Network(container, data, options);
}
