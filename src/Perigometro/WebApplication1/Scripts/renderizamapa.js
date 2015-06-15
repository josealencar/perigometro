var heatmapData = [];
var mapOptions;
var map;
var array;
var heatmap;

var data = [{
    resource_id: '6f72ea19-d111-49c8-8ce5-57ca8bdc8195', //2000
    limit: 18153
}, {
    resource_id: '0a1710fd-2608-4b74-8a32-d7b3640a7c67', //2001
    limit: 21138
}, {
    resource_id: '52d095d2-f1b8-49d1-abbf-42aa13449980', //2002
    limit: 22145
}, {
    resource_id: 'b52410b9-5f63-4b93-b94b-2caf9dec0569', //2003
    limit: 20936
}, {
    resource_id: '448d3955-164d-4754-830c-169ca1beee65', //2004
    limit: 20662
}, {
    resource_id: 'b316d4e4-6235-4268-b233-b9ed9e8c08ce', //2005
    limit: 20758
}, {
    resource_id: '3a621653-5ff6-441b-8b54-d0d7f8f3d330', //2006
    limit: 20333
}, {
    resource_id: '2fe10125-b0e1-4e41-be26-232b59229339', //2007
    limit: 22244
}, {
    resource_id: 'fb184504-5336-4652-8772-309772692a8b', //2008
    limit: 22289
}, {
    resource_id: '84a91914-df48-4a2a-b390-b39ef4c055c9', //2009
    limit: 16300
}, {
    resource_id: '798d6958-cf41-4d41-8970-95a993b0cf83', //2010
    limit: 25474
}, {
    resource_id: 'f830a7d0-3d43-4d0a-b712-877f97813ed7', //2011
    limit: 23579
}, {
    resource_id: 'd9add4bb-00a7-4205-bd36-7bd89439a09a', //2012
    limit: 20202
}, {
    resource_id: 'a027ffcb-dcbe-46df-b6b7-b8ba69ee1559', //2013
    limit: 11200
}];

function initialize(){
    mapOptions = {
        center: new google.maps.LatLng(-30.034647, -51.217658),
        zoom: 13,
        mapTypeId: google.maps.MapTypeId.MAP
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

    /*dispara os ajax*/
    $.when(
        $.each(data, function (i, dt) {
            $.ajax({
                url: 'http://datapoa.com.br/api/action/datastore_search',
                data: dt,
                dataType: 'jsonp'
            }).done(
                  function (result) {
                      console.log(result);
                      //alert('Total results found: ' + result.result.total);
                      var resultado = result.result.records;
                      //console.log(resultado);
                      array = $.extend({}, resultado);
                      //console.log(array);
                      $.each(array, function (i, res) {
                          heatmapData.push(new google.maps.LatLng(res.LATITUDE, res.LONGITUDE))
                      });
                  }
              )
        })).then(function (x) {
            adicionaHeatmap();
        });
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