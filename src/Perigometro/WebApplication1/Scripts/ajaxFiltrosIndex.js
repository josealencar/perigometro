$('[name=Ano]').first().attr('name', 'AnoInicial');
$('[name=Ano]').first().attr('name', 'AnoFinal');
function FiltraDadosIndex() {
    var formulario = $('form[name=filtrosIndex]');
    var li = formulario.parent().parent().parent();
    console.log(formulario);
    console.log(li);
    $.ajax({
        url: '/Acidentes/BuscarDadosFiltrados',
        type: 'POST',
        dataType: 'JSON',
        data: formulario.serialize()
    }).done(function (res) {
        console.log(res);
        li.attr('class', 'dropdown yamm-fw');
        heatmap.setMap(heatmap.getMap() ? null : null);
        populaHeatmapData(res);
    }).fail(function (res) {
        console.log(res);
    })
}