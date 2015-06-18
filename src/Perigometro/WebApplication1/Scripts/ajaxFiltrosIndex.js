$('[name=Ano]').first().attr('name', 'AnoInicial');
$('[name=Ano]').first().attr('name', 'AnoFinal');
function FiltraDadosIndex() {
    var formulario = $('form[name=filtrosIndex]');
    var anoInicial = formulario.find('select[name=AnoInicial]').val();
    var anoFinal = formulario.find('select[name=AnoFinal]').val();
    var clima = formulario.find('select[name=Clima]').val();
    var regiao = formulario.find('select[name=Regiao]').val();
    var semana = formulario.find('select[name=Semana]').val();
    var turno = formulario.find('select[name=Turno]').val();
    var tipo = formulario.find('select[name=Tipo]').val();
    var veiculo = formulario.find('select[name=Veiculo]').val();
    var fatal = formulario.find('input[name=Fatal]').is(':checked');

    if (anoInicial === "" && anoFinal === "" && clima === "" && regiao === "" && semana === "" && turno === ""
        && tipo === "" && veiculo === "" && fatal === false) {
        if (confirm("Você não selecionou nenhum filtro, por padrão quando não existe nenhum filtro selecionado é retornado para a tela todos os registros do banco, se você deseja que seja retornado todos os registros favor clicar em Ok, caso contrário clique em Cancelar e selecione algum filtro. (Obs.: Caso clique em Ok pode levar algum tempo para carregar todos os dados no mapa)")) {
            console.log("Buscando todos os dados.");
        } else {
            console.log("Busca cancelada.");
            return;
        }
    }

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