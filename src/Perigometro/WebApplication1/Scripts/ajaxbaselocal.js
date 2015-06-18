function buscarDados() {
    $.ajax({
        url: '/Acidentes/BuscarInicial',
        type: 'GET'
    }).done(function (res) { populaHeatmapData(res); }).fail(function (res) { console.log(res);});
};

function populaHeatmapData(dados) {
    heatmapData = [];
    $.each(dados.Dados, function (i, dado) {
        heatmapData.push(new google.maps.LatLng(dado.Latitude, dado.Longitude));
    })
    adicionaHeatmap();
};