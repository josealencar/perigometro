function buscaEstatisticas(ano1, ano2)
{
    var anos = [];
    anos.push(ano1);
    anos.push(ano2);
    json = JSON.stringify({'Anos': anos});
    $.ajax({
        url: '/Dashboard/Estatisticas',
        data: json,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
    }).done(function (res) {
        console.log(res);
    })
}