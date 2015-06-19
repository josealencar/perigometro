var myPieChart;
var myBarChart;

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
    var ctx = document.getElementById("numeros").getContext("2d");

    var dataGrafico = {
        labels: ["Dia da Semana - SEXTA", "Condição Climática - BOM", "Região - LESTE", "Transporte mais Seguro - Bicileta", "Transporte mais Inseguro - Automóvel"],
        datasets: [
            {
                label: "Estatisticas",
                fillColor: "rgba(231, 76, 60,1.0)",
                strokeColor: "rgba(231, 76, 60,1.0)",
                highlightFill: "rgba(192, 57, 43,1.0)",
                highlightStroke: "rgba(192, 57, 43,1.0)",
                data: [2007, 9487, 3447, 109, 9796]
            }
        ]
    };

    myBarChart = new Chart(ctx).Bar(dataGrafico, { barShowStroke: false });
};

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

        myBarChart.datasets[0].bars[0].value = dia[0].Qtde;
        myBarChart.datasets[0].bars[1].value = clima[0].Qtde;
        myBarChart.datasets[0].bars[2].value = regiao[0].Qtde;
        myBarChart.datasets[0].bars[3].value = seguro.Qtde;
        myBarChart.datasets[0].bars[4].value = inseguro.Qtde;

        myBarChart.datasets[0].bars[0].label = "Dia da Semana - " + dia[0].Dia;
        myBarChart.scale.xLabels[0] = "Dia da Semana - " + dia[0].Dia;
        myBarChart.datasets[0].bars[1].label = "Condição Climática - " + clima[0].Clima;
        myBarChart.scale.xLabels[1] = "Condição Climática - " + clima[0].Clima;
        myBarChart.datasets[0].bars[2].label = "Região - " + regiao[0].Regiao;
        myBarChart.scale.xLabels[2] = "Região - " + regiao[0].Regiao;
        myBarChart.datasets[0].bars[3].label = "Transporte mais Seguro - " + seguro.Nome;
        myBarChart.scale.xLabels[3] = "Transporte mais Seguro - " + seguro.Nome;
        myBarChart.datasets[0].bars[4].label = "Transporte mais Inseguro - " + inseguro.Nome;
        myBarChart.scale.xLabels[4] = "Transporte mais Inseguro - " + inseguro.Nome;


        myBarChart.update();
        myPieChart.update();
    })
}