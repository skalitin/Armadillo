function RenderNetwork(network) {
    console.log(network);

    var nodes = new vis.DataSet(network.nodes);
    var edges = new vis.DataSet(network.edges);
    var data = {
        nodes: nodes,
        edges: edges
    };

    var options = {
        nodes: {
          borderWidth: 1,
        },
        edges: {
          width: 2,
          // color: {
          //   inherit: 'both'
          // }
        },
        groups: {
          subcases: {
            shape: 'dot',
            font: { size: 20 },
          },
          customers: {
            shape: 'icon',
            font: { size: 15 },
            icon: {
              face: 'FontAwesome',
              code: '\uf1ad',
              size: 30,
              color: '#f0a30a'
            }
          },
          owners: {
            shape: 'icon',
            font: { size: 15 },
            icon: {
              face: 'FontAwesome',
              code: '\uf007',
              size: 30,
              color: '#aa00ff'
            }
          }
        }
      };

    var container = document.getElementById('network');
    var network = new vis.Network(container, data, options);
}
