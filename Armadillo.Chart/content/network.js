function RenderNetwork(network) {
    console.log(network);

    var nodes = new vis.DataSet(network.nodes);
    var edges = new vis.DataSet(network.edges);
    var data = {
        nodes: nodes,
        edges: edges
    };

    var options = {
        groups: {
          customers: {
            shape: 'icon',
            icon: {
              face: 'FontAwesome',
              code: '\uf1ad',
              size: 50,
              color: '#f0a30a'
            }
          },
          owners: {
            shape: 'icon',
            icon: {
              face: 'FontAwesome',
              code: '\uf007',
              size: 50,
              color: '#aa00ff'
            }
          }
        }
      };

    var container = document.getElementById('network');
    var network = new vis.Network(container, data, options);
}
