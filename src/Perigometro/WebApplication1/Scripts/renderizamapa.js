var heatmapData = [];
var mapOptions;
var map;
var array;
var heatmap;

function initialize(){
    mapOptions = {
        center: new google.maps.LatLng(-30.023994, -51.183378),
        zoom: 14,
        mapTypeId: google.maps.MapTypeId.MAP
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    buscarDados();
};

function adicionaHeatmap() {
    heatmap = new google.maps.visualization.HeatmapLayer({
        data: heatmapData
    });
    heatmap.setMap(map); 
};

/*Modificação do heatmap*/
function toggleHeatmap() {
  heatmap.setMap(heatmap.getMap() ? null : map);
};

/*Muda a cor*/
function changeGradient() {
  var gradient = [
    'rgba(0, 255, 255, 0)',
    'rgba(0, 255, 255, 1)',
    'rgba(0, 191, 255, 1)',
    'rgba(0, 127, 255, 1)',
    'rgba(0, 63, 255, 1)',
    'rgba(0, 0, 255, 1)',
    'rgba(0, 0, 223, 1)',
    'rgba(0, 0, 191, 1)',
    'rgba(0, 0, 159, 1)',
    'rgba(0, 0, 127, 1)',
    'rgba(63, 0, 91, 1)',
    'rgba(127, 0, 63, 1)',
    'rgba(191, 0, 31, 1)',
    'rgba(255, 0, 0, 1)'
  ]
  heatmap.set('gradient', heatmap.get('gradient') ? null : gradient);
};

/*Muda o tamanho do raio de cobertura do ponto no mapa*/
function changeRadius() {
  heatmap.set('radius', heatmap.get('radius') ? null : 20);
};

/*Muda opacidade*/
function changeOpacity() {
  heatmap.set('opacity', heatmap.get('opacity') ? null : 0.2);
}

google.maps.event.addDomListener(window, 'load', initialize);