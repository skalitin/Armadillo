function RenderNetwork(network) {
    // console.log(network);

    var nodes = new vis.DataSet(network.nodes);
    var edges = new vis.DataSet(network.edges);
    var data = {
        nodes: nodes,
        edges: edges
    };

    var options = {
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
              code: '\uf1ad',
              size: 30,
              color: 'lightskyblue'
            }
          },
          owners: {
            shape: 'box',
            font: { size: 20 },
            color: 'lightblue'
            // icon: {
            //   face: 'FontAwesome',
            //   code: '\uf007',
            //   size: 30,
            //   color: 'blue'
            // }
          }
        }
      };

    var container = document.getElementById('network');
    var network = new vis.Network(container, data, options);
}
