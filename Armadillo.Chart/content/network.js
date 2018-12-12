function RenderNetwork(network) {
    var nodes = new vis.DataSet(network.nodes);
    var edges = new vis.DataSet(network.edges);
    var data = {
        nodes: nodes,
        edges: edges
    };

    var options = {
        physics: {
          stabilization: false
        },      
        nodes: {
          borderWidth: 2,
          shadow: true,
          // scaling: {
          //   customScalingFunction: function (min, max, total, value) {
          //     return value/total;
          //   },
          //   min: 5,
          //   max: 150
          // }
        },
        edges: {
          width: 2,
          shadow: true,
          color: {
            color: 'gray'
          }
        },
        groups: {
          subcases: {
            shape: 'dot',
            font: { size: 20 },
          },
          customers: {
            shape: 'icon',
            font: { size: 20 },
            //color: 'lightskyblue',
            icon: {
              face: 'FontAwesome',
              code: '\uf0c0',
              size: 30,
              color: 'blue'
            }
          },
          owners: {
            shape: 'icon',
            //shape: 'box',
            font: { size: 20 },
            //color: 'lightblue',
            icon: {
              face: 'FontAwesome',
              code: '\uf007',
              size: 30,
              color: 'lightblue'
            }
          }
        }
      };
    
    var container = document.getElementById('network-wrapper');
    window.network = new vis.Network(container, data, options);
}
