function AddCenterLabel(chart, slices) {
    let subcaseCount = slices.reduce((total, value) => { return total + value.count }, 0);
    let label = anychart.standalones.label();
    label.text(subcaseCount);
    label.fontColor("black");
    label.fontSize(40);
    label.width("100%");
    label.height("100%");
    label.hAlign("center");
    label.vAlign("middle");
    chart.center().content(label);
}

function RenderStatusStatistics(slices) {
    
  let data = slices.map(each => {
      return {
          x: each.status,
          value: each.count,
          fill: each.color,
          label: {enabled:true, fontColor: "black", fontSize: 15, format:"{%value}"}
      };
    });

    let chart = window.status_statistics_chart;
    if(!chart) {
        chart = anychart.pie();
        window.status_statistics_chart = chart;
    }

    chart.title("Subcases by status");

    chart.data(data);
    chart.innerRadius("60%");
    AddCenterLabel(chart, slices);

    let interactivity = chart.interactivity();
    interactivity.selectionMode('none');
    interactivity.hoverMode("byspot");

    chart.legend().itemsLayout("horizontal-expandable");

    chart.container("status-statistics-container");
    chart.draw();  
}

function RenderLevelStatistics(slices) {
  
   let data = slices.map(each => {
        return {
            x: "Level " + each.level,
            value: each.count,
            fill: each.color,
            label: {enabled:true, fontColor: "black", fontSize: 15, format:"{%value}"}
        };
   });
  
   let chart = window.level_statistics_chart;
   if(!chart) {
      chart = anychart.pie();
      window.level_statistics_chart = chart;
   }
  
  chart.title("Subcases by level");

  chart.data(data);
  chart.innerRadius("60%");
  AddCenterLabel(chart, slices);
  
  let interactivity = chart.interactivity();
  interactivity.selectionMode('none');
  interactivity.hoverMode("byspot");

  chart.legend().itemsLayout("horizontal-expandable");
  
  chart.container("level-statistics-container");
  chart.draw();
}

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
          shadow: true,
          // shapeProperties:{borderDashes:[10, 10]}
          // borderWidth: 2,
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
