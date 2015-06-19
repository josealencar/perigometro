var myPieChart;

function inicializaPie() {
    var ctx = document.getElementById("myChart").getContext("2d");
    var dataGrafico = [
        {
            value: 21,
            color: "#F7464A",
            highlight: "#FF5A5E",
            label: "SUL"
        },
        {
            value: 20,
            color: "#46BFBD",
            highlight: "#5AD3D1",
            label: "NORTE"
        },
        {
            value: 18,
            color: "#FDB45C",
            highlight: "#FFC870",
            label: "LESTE"
        },
        {
            value: 4,
            color: "#27ae60",
            highlight: "#2ecc71",
            label: "CENTRO"
        }
    ];
    myPieChart = new Chart(ctx).Pie(dataGrafico, { animateScale: true });
}

function inicializaBar() {
    $('#dia').html("Sexta-feira");
    $('#tempo').html("Tempo Bom");
    $('#regiao').html("Zona Leste");
    $('#seguro').html("Bicicleta");
    $('#inseguro').html("Automóvel");
};
function toTitleCase(str) {
    return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
}
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

        var mortes = res.Mortes;
        var clima = res.Clima;
        var regiao = res.Regiao;
        var dia = res.Dia;
        var seguro = res.Seguro;
        var inseguro = res.Inseguro;

        mortes.forEach(function (item, i) {
            myPieChart.segments[i].value = item.Qtde;
            myPieChart.segments[i].label = item.Regiao;
        });


        $('ul[class=loader]').attr('hidden', 'hidden');

        $('#dia').html(toTitleCase(dia[0].Dia));
        $('#tempo').html("Tempo " + toTitleCase(clima[0].Clima));
        $('#regiao').html("Zona " + toTitleCase(regiao[0].Regiao));
        $('#seguro').html(seguro.Nome);
        $('#inseguro').html(inseguro.Nome);


        myPieChart.update();
    })
}