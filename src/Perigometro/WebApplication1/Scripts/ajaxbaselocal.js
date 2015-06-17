function buscarDados() {
    $.ajax({
        url: '/Acidentes/BuscarTodos',
        type: 'GET'
    }).done(function (res) { console.log(res); populaHeatmapData(res); });
};

function populaHeatmapData(dados) {
    heatmapData = [];
    $.each(dados.Dados, function (i, dado) {
        heatmapData.push(new google.maps.LatLng(dado.Latitude, dado.Longitude));
    })
    adicionaHeatmap();
};