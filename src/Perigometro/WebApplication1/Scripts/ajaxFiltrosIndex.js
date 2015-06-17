function FiltraDadosIndex() {
    var formulario = $('form[name=filtrosIndex]');
    $.ajax({
        url: '/Acidentes/BuscarDadosFiltrados',
        type: 'POST'
    }).done(function (res) {
        console.log(res);
    }).fail(function (res) {
        console.log(res);
    })
}